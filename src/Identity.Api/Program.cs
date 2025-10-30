using Identity.Api;
using Identity.Api.Abstractions;
using Identity.Api.Services;
using Identity.Application;
using Identity.Application.Abstractions;
using Identity.Application.Models.Options;
using Identity.Infrastructure;
using Identity.Infrastructure.Integrations.Email;
using Identity.Infrastructure.Persistence;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddHttpContextAccessor();

builder.Services.Configure<SmtpOptions>(builder.Configuration.GetSection("Smtp"));
builder.Services.AddApplicationServices();
builder.Services.AddInfrastructureServices(builder.Configuration);

builder.Services.Configure<JwtOptions>(builder.Configuration.GetSection("Jwt"));

// Set max file size
builder.Services.Configure<FormOptions>(options =>
{
    options.MultipartBodyLengthLimit = 2 * 1024 * 1024; // 2 MB
});

builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = "JwtBearer";
        options.DefaultChallengeScheme = "JwtBearer";
    })
    .AddJwtBearer("JwtBearer", options =>
    {
        var jwtOptions = builder.Configuration.GetSection("Jwt").Get<JwtOptions>()
            ?? throw new InvalidOperationException("JWT settings are not configured");

        options.TokenValidationParameters = new()
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(
                System.Text.Encoding.UTF8.GetBytes(jwtOptions.Secret))
        };
    });


builder.Services.AddScoped<IAccessTokenService, AccessTokenService>();
builder.Services.AddScoped<IRefreshTokenService, RefreshTokenService>();
builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();

builder.Services.AddScoped<IConfirmationLinkGenerator, ConfirmationLinkGenerator>();
builder.Services.AddSingleton<IConfirmationEmailBuilder, ConfirmationEmailBuilder>();

builder.Services.AddScoped<IPasswordResetLinkGenerator, PasswordResetLinkGenerator>();
builder.Services.AddSingleton<IPasswordResetEmailBuilder, PasswordResetEmailBuilder>();

if (builder.Environment.IsDevelopment())
{
    builder.Services.AddScoped<IEmailConfirmationService, LogConfirmationService>();
    builder.Services.AddScoped<IPasswordResetEmailService, LogPasswordResetService>();
}
else
{
    builder.Services.AddScoped<IEmailConfirmationService, EmailConfirmationService>();
    builder.Services.AddScoped<IPasswordResetEmailService, PasswordResetEmailService>();
}

builder.Services.AddRequestHandlers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    
    using var scope = app.Services.CreateScope();
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

    await context.Database.MigrateAsync();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
