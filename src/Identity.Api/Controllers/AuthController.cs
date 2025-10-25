using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using Identity.Api.Abstractions;
using Identity.Api.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Identity.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly JwtSettings _jwtSettings;
    private readonly IEmailSender _emailSender;
    private readonly IApplicationDbContext _dbContext;

    public AuthController(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        IOptions<JwtSettings> jwtSettings,
        IEmailSender emailSender,
        IApplicationDbContext dbContext)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _jwtSettings = jwtSettings.Value;
        _emailSender = emailSender;
        _dbContext = dbContext;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterDto dto)
    {
        var user = new ApplicationUser { UserName = dto.Email, Email = dto.Email };
        var result = await _userManager.CreateAsync(user, dto.Password);

        if (!result.Succeeded)
            return BadRequest(result.Errors);

        var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
        var encodedToken = HttpUtility.UrlEncode(token);
        var confirmationLink = $"{Request.Scheme}://{Request.Host}/api/auth/confirm-email?userId={user.Id}&token={encodedToken}";

        await _emailSender.SendEmailAsync(
            user.Email,
            "Подтверждение регистрации",
            $@"<h2>Добро пожаловать!</h2>
               <p>Пожалуйста, подтвердите ваш email, перейдя по ссылке:</p>
               <p><a href='{confirmationLink}'>Подтвердить email</a></p>
               <p>Или скопируйте ссылку в браузер:</p>
               <p>{confirmationLink}</p>");

        return Ok(new { message = "Регистрация успешна. Проверьте email для подтверждения." });
    }

    [HttpGet("confirm-email")]
    public async Task<IActionResult> ConfirmEmail([FromQuery] string userId, [FromQuery] string token)
    {
        if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(token))
            return BadRequest(new { message = "UserId и token обязательны" });

        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
            return BadRequest(new { message = "Пользователь не найден" });

        var result = await _userManager.ConfirmEmailAsync(user, token);
        if (!result.Succeeded)
            return BadRequest(new { message = "Ошибка подтверждения email", errors = result.Errors });

        return Ok(new { message = "Email успешно подтвержден. Теперь вы можете войти в систему." });
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginDto dto)
    {
        var user = await _userManager.FindByEmailAsync(dto.Email);
        if (user == null)
            return Unauthorized(new { message = "Неверный email или пароль" });

        if (!await _userManager.IsEmailConfirmedAsync(user))
            return Unauthorized(new { message = "Email не подтвержден. Проверьте вашу почту." });

        var result = await _signInManager.CheckPasswordSignInAsync(user, dto.Password, false);
        if (!result.Succeeded)
            return Unauthorized(new { message = "Неверный email или пароль" });

        var accessToken = GenerateAccessToken(user);
        var refreshToken = GenerateRefreshToken();
        var tokenHash = HashToken(refreshToken);

        // Save refresh token hash to database
        var userRefreshToken = new UserRefreshToken
        {
            UserId = user.Id,
            TokenHash = tokenHash,
            ExpiresAt = DateTime.UtcNow.AddDays(7), // Refresh token valid for 7 days
            CreatedAt = DateTime.UtcNow
        };

        _dbContext.UserRefreshTokens.Add(userRefreshToken);
        await _dbContext.SaveChangesAsync(CancellationToken.None);

        return Ok(new
        {
            accessToken = accessToken,
            refreshToken = refreshToken,
            expiresIn = _jwtSettings.ExpirationHours * 3600 // in seconds
        });
    }

    [HttpPost("refresh")]
    public async Task<IActionResult> RefreshToken(RefreshTokenDto dto)
    {
        var tokenHash = HashToken(dto.RefreshToken);

        var userRefreshToken = await _dbContext.UserRefreshTokens
            .Include(rt => rt.User)
            .FirstOrDefaultAsync(rt => rt.TokenHash == tokenHash);

        if (userRefreshToken == null || !userRefreshToken.IsActive)
            return Unauthorized(new { message = "Недействительный или истекший refresh token" });

        var accessToken = GenerateAccessToken(userRefreshToken.User);

        return Ok(new
        {
            accessToken = accessToken,
            refreshToken = dto.RefreshToken, // Return the same refresh token
            expiresIn = _jwtSettings.ExpirationHours * 3600 // in seconds
        });
    }

    [HttpPost("revoke")]
    public async Task<IActionResult> RevokeToken(RefreshTokenDto dto)
    {
        var tokenHash = HashToken(dto.RefreshToken);

        var userRefreshToken = await _dbContext.UserRefreshTokens
            .FirstOrDefaultAsync(rt => rt.TokenHash == tokenHash);

        if (userRefreshToken == null)
            return BadRequest(new { message = "Refresh token не найден" });

        if (userRefreshToken.IsRevoked)
            return BadRequest(new { message = "Refresh token уже отозван" });

        userRefreshToken.RevokedAt = DateTime.UtcNow;
        await _dbContext.SaveChangesAsync(CancellationToken.None);

        return Ok(new { message = "Refresh token успешно отозван" });
    }

    private string GenerateAccessToken(ApplicationUser user)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.UTF8.GetBytes(_jwtSettings.Key);
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Name, user.UserName!)
            }),
            Expires = DateTime.UtcNow.AddHours(_jwtSettings.ExpirationHours),
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(key),
                SecurityAlgorithms.HmacSha256Signature)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }

    private static string GenerateRefreshToken()
    {
        var randomNumber = new byte[64];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);
        return Convert.ToBase64String(randomNumber);
    }

    private static string HashToken(string token)
    {
        using var sha256 = SHA256.Create();
        var tokenBytes = Encoding.UTF8.GetBytes(token);
        var hashBytes = sha256.ComputeHash(tokenBytes);
        return Convert.ToHexString(hashBytes);
    }
}

public record RegisterDto(string Email, string Password);
public record LoginDto(string Email, string Password);
public record RefreshTokenDto(string RefreshToken);