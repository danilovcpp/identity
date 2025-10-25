using System.Security.Claims;
using System.Text;
using Identity.Api.Infrastructure;
using Identity.Api.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Identity.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly IConfiguration _config;

    public AuthController(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        IConfiguration config)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _config = config;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterDto dto)
    {
        var user = new ApplicationUser { UserName = dto.Email, Email = dto.Email };
        var result = await _userManager.CreateAsync(user, dto.Password);

        if (!result.Succeeded)
            return BadRequest(result.Errors);

        return Ok();
    }

    // [HttpPost("login")]
    // public async Task<IActionResult> Login(LoginDto dto)
    // {
    //     var user = await _userManager.FindByEmailAsync(dto.Email);
    //     if (user == null) return Unauthorized();
    //
    //     var result = await _signInManager.CheckPasswordSignInAsync(user, dto.Password, false);
    //     if (!result.Succeeded) return Unauthorized();
    //
    //     // Создаем JWT токен
    //     var tokenHandler = new JwtSecurityTokenHandler();
    //     var key = Encoding.UTF8.GetBytes(_config["Jwt:Key"]);
    //     var tokenDescriptor = new SecurityTokenDescriptor
    //     {
    //         Subject = new ClaimsIdentity(new[]
    //         {
    //             new Claim(ClaimTypes.NameIdentifier, user.Id),
    //             new Claim(ClaimTypes.Name, user.UserName)
    //         }),
    //         Expires = DateTime.UtcNow.AddHours(12),
    //         SigningCredentials = new SigningCredentials(
    //             new SymmetricSecurityKey(key),
    //             SecurityAlgorithms.HmacSha256Signature)
    //     };
    //
    //     var token = tokenHandler.CreateToken(tokenDescriptor);
    //     return Ok(new { token = tokenHandler.WriteToken(token) });
    // }
}

public record RegisterDto(string Email, string Password);
public record LoginDto(string Email, string Password);