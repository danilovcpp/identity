# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

This is an ASP.NET Core 8.0 Identity API service that provides JWT-based authentication with refresh token support and email confirmation. The project uses:
- **ASP.NET Core Identity** for user management
- **Entity Framework Core** with PostgreSQL for data persistence
- **JWT Bearer tokens** with refresh tokens for authentication
- **MediatR** (CQRS pattern) for request handling
- **MailKit** for email confirmation functionality
- **Swagger/OpenAPI** for API documentation

## Solution Structure

The solution follows Clean Architecture principles with four projects:
- **Identity.Domain** - Domain entities and core business objects
- **Identity.Application** - Application logic with MediatR handlers, abstractions, and behaviors
- **Identity.Infrastructure** - External concerns (database, email, persistence)
- **Identity.Api** - Presentation layer (controllers, service implementations)

**Dependency Flow**: Api → Application → Domain ← Infrastructure

## Architecture

### Key Architectural Patterns

**MediatR CQRS Pattern**: The codebase uses MediatR for request/response handling:
- Controllers inject `ISender` and dispatch requests via `sender.Send(request)`
- Request handlers are in Identity.Application implementing `IRequestHandler<TRequest, TResponse>`
- Requests are records implementing `IRequest<TResponse>` from MediatR
- Responses are simple record types
- Example: [LoginController.cs](src/Identity.Api/Controllers/LoginController.cs) dispatches [LoginRequest](src/Identity.Application/Login/LoginRequest.cs) to [LoginRequestHandler](src/Identity.Application/Login/LoginRequestHandler.cs)

**MediatR Pipeline Behaviors** (registered in [Identity.Application/DependencyInjection.cs](src/Identity.Application/DependencyInjection.cs)):
1. `UnhandledExceptionBehaviour` - Global exception handling
2. `AuthorizationBehaviour` - Authorization checks
3. `ValidationBehaviour` - FluentValidation integration
4. `PerformanceBehaviour` - Performance monitoring/logging

**Database Access**:
- `ApplicationDbContext` (in Infrastructure) inherits from `IdentityDbContext<ApplicationUser>` and implements `IApplicationDbContext`
- `IApplicationDbContext` interface is in Application layer (abstractions)
- Custom entity: `UserRefreshToken` stores hashed refresh tokens with expiration and revocation support
- All `IEntityTypeConfiguration` implementations are auto-applied from assembly

**Authentication Architecture**:
- JWT access tokens (default: 15 minute lifetime) via `IAccessTokenService`
- Refresh tokens (7 day lifetime) via `IRefreshTokenService` - tokens are hashed before storage using SHA256
- Email confirmation required for login (configured in [Identity.Infrastructure/DependencyInjection.cs](src/Identity.Infrastructure/DependencyInjection.cs))
- In development: email confirmations are logged via `LogConfirmationService`
- In production: emails sent via SMTP using `EmailConfirmationService` and MailKit

**Service Abstractions**: Interfaces defined in [Identity.Application/Abstractions/](src/Identity.Application/Abstractions/):
- `IApplicationDbContext` - Database context abstraction
- `IAccessTokenService` - JWT generation
- `IRefreshTokenService` - Refresh token generation and hashing
- `IEmailSender` - SMTP email delivery
- `IEmailConfirmationService` - Orchestrates confirmation email sending
- `IConfirmationLinkGenerator` - Generates confirmation URLs
- `IConfirmationEmailBuilder` - Builds HTML email content
- `IPasswordResetEmailService` - Orchestrates password reset email sending
- `IPasswordResetLinkGenerator` - Generates password reset URLs
- `IPasswordResetEmailBuilder` - Builds HTML email content for password reset

### API Endpoints

All endpoints are defined in [Identity.Api/Controllers/](src/Identity.Api/Controllers/):
- **POST /api/register** - User registration (sends confirmation email)
- **POST /api/login** - Login with email/password (returns access + refresh tokens)
- **POST /api/refresh** - Refresh access token using refresh token
- **POST /api/revoke** - Revoke a refresh token
- **POST /api/confirm-email** - Confirm email via token link
- **POST /api/forgot-password** - Request password reset (sends reset email)
- **POST /api/reset-password** - Reset password using token

## Development Commands

### Building and Running
```bash
# Build the solution
dotnet build Identity.sln

# Run the API (from solution root)
dotnet run --project src/Identity.Api/Identity.Api.csproj

# Run in watch mode for development
dotnet watch --project src/Identity.Api/Identity.Api.csproj
```

### Database Migrations
```bash
# Add a new migration (run from solution root)
dotnet ef migrations add MigrationName --project src/Identity.Api/Identity.Api.csproj

# Update database manually (not needed in development - auto-migrates)
dotnet ef database update --project src/Identity.Api/Identity.Api.csproj

# Remove last migration
dotnet ef migrations remove --project src/Identity.Api/Identity.Api.csproj
```

### Docker
```bash
# Build Docker image (from solution root)
docker build -f src/Identity.Api/Dockerfile -t identity-api .

# Run container
docker run -p 8080:8080 -p 8081:8081 identity-api
```

## Configuration

### Required Settings in appsettings.Development.json

**Database**:
- `ConnectionStrings:IdentityDatabase` - PostgreSQL connection string
- Default: `server=localhost;port=5432;user id=postgres;password=postgres;database=identity`

**JWT**:
- `Jwt:Secret` - Signing key (must be at least 256 bits)
- `Jwt:AccessTokenLifetimeMinutes` - Access token expiration (default: 15)
- `Jwt:RefreshTokenLifetimeDays` - Refresh token expiration (default: 7)
- Note: Issuer and Audience are configured but NOT validated ([Program.cs:48-49](src/Identity.Api/Program.cs#L48-L49))

**SMTP** (required for production email confirmation):
- `Smtp:Host` - SMTP server (default: smtp.gmail.com)
- `Smtp:Port` - SMTP port (default: 587)
- `Smtp:UseSsl` - Use SSL/TLS (default: false)
- `Smtp:Username` - SMTP username
- `Smtp:Password` - SMTP password
- `Smtp:FromEmail` - Sender email address
- `Smtp:FromName` - Sender display name

### Identity Configuration

Password requirements (configured in [Identity.Infrastructure/DependencyInjection.cs](src/Identity.Infrastructure/DependencyInjection.cs)):
- Minimum 6 characters
- Non-alphanumeric characters NOT required
- Email confirmation required for sign-in

### Auto-Migration

In development environment, database migrations run automatically on startup ([Program.cs:74](src/Identity.Api/Program.cs#L74)).

## Important Implementation Details

**Refresh Token Security**:
- Tokens are generated as cryptographically secure random strings
- Only SHA256 hashes are stored in database, never plaintext tokens
- Tokens track expiration, revocation, and creation timestamps
- `UserRefreshToken` entity has computed properties: `IsRevoked`, `IsExpired`, `IsActive`

**Email Confirmation Flow**:
1. User registers → confirmation token generated by Identity framework
2. Confirmation link built with token and user ID
3. In dev: link logged to console; In prod: email sent via SMTP
4. User clicks link → `POST /api/confirm-email` with userId and token
5. Login endpoint checks email confirmation before allowing access

**Password Reset Flow**:
1. User requests reset → `POST /api/forgot-password` with email
2. Password reset token generated by Identity framework
3. Reset link built with token and email (URL-encoded)
4. In dev: link logged to console; In prod: email sent via SMTP
5. User clicks link → `POST /api/reset-password` with email, token, and new password
6. Token validated and password updated
7. Security: Returns success for non-existent users to prevent email enumeration

**Error Handling**:
- Custom exceptions per feature (e.g., `UnauthorizedException`, `EmailNotConfirmedException`, `InvalidRefreshTokenException`)
- Exceptions organized in feature-specific `Exceptions/` folders within Identity.Application
- Common exceptions in [Identity.Application/Common/Exceptions/](src/Identity.Application/Common/Exceptions/)

## Adding New Endpoints

To add a new endpoint following the MediatR pattern:

### 1. In Identity.Application (Application Layer)
Create folder `Identity.Application/NewFeature/` with:
- `NewFeatureRequest.cs` - Record implementing `IRequest<NewFeatureResponse>`
- `NewFeatureResponse.cs` - Response DTO record
- `NewFeatureRequestHandler.cs` - Implements `IRequestHandler<NewFeatureRequest, NewFeatureResponse>`
- `Exceptions/` subfolder for feature-specific exceptions (if needed)

**Example Request:**
```csharp
public record NewFeatureRequest(string Param) : IRequest<NewFeatureResponse>;
```

**Example Handler:**
```csharp
public class NewFeatureRequestHandler : IRequestHandler<NewFeatureRequest, NewFeatureResponse>
{
    public async Task<NewFeatureResponse> Handle(NewFeatureRequest request, CancellationToken cancellationToken)
    {
        // Implementation
        return new NewFeatureResponse();
    }
}
```

### 2. In Identity.Api (Presentation Layer)
Create `Identity.Api/Controllers/NewFeatureController.cs`:
```csharp
[ApiController]
[Route("api/new-feature")]
public class NewFeatureController : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> NewFeature(
        [FromServices] ISender sender,
        [FromBody] NewFeatureRequest request)
    {
        var response = await sender.Send(request);
        return Ok(response);
    }
}
```

### 3. Registration
- MediatR handlers are auto-registered by scanning the Identity.Application assembly
- No manual registration needed in DependencyInjection.cs
