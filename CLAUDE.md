# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

This is an ASP.NET Core 8.0 Identity API service that provides JWT-based authentication with refresh token support and email confirmation. The project uses:
- **ASP.NET Core Identity** for user management
- **Entity Framework Core** with PostgreSQL for data persistence
- **JWT Bearer tokens** with refresh tokens for authentication
- **MailKit** for email confirmation functionality
- **Swagger/OpenAPI** for API documentation

## Solution Structure

The solution contains two projects:
- **Identity.Api** - Main API project with controllers, services, and persistence
- **Identity.Domain** - Domain layer with entity definitions (currently minimal usage)

## Architecture

### Key Architectural Patterns

**Request Handler Pattern**: The codebase uses a custom request/response handler pattern rather than traditional service layers:
- Controllers are thin and delegate to request handlers via `IRequestHandler<TRequest, TResponse>`
- Each endpoint has its own folder containing: `Controller`, `Request`, `Response`, `RequestHandler`, and `Exceptions`
- Request handlers are automatically registered via reflection in [DependencyInjection.cs](src/Identity.Api/DependencyInjection.cs)
- Example: Login functionality lives in `Controllers/Login/` with `LoginController`, `LoginRequest`, `LoginResponse`, and `LoginRequestHandler`

**Database Access**:
- `ApplicationDbContext` inherits from `IdentityDbContext<ApplicationUser>` and implements `IApplicationDbContext`
- Custom entity: `UserRefreshToken` stores hashed refresh tokens with expiration and revocation support
- All `IEntityTypeConfiguration` implementations are auto-applied from assembly

**Authentication Architecture**:
- JWT access tokens (default: 15 minute lifetime) via `IAccessTokenService`
- Refresh tokens (7 day lifetime) via `IRefreshTokenService` - tokens are hashed before storage using SHA256
- Email confirmation required for login ([Program.cs:31](src/Identity.Api/Program.cs#L31))
- In development: email confirmations are logged via `LogConfirmationService`
- In production: emails sent via SMTP using `EmailConfirmationService` and MailKit

**Service Abstractions**: All major functionality is defined via interfaces in `Abstractions/`:
- `IAccessTokenService` - JWT generation
- `IRefreshTokenService` - Refresh token generation and hashing
- `IEmailSender` - SMTP email delivery
- `IEmailConfirmationService` - Orchestrates confirmation email sending
- `IConfirmationLinkGenerator` - Generates confirmation URLs
- `IConfirmationEmailBuilder` - Builds HTML email content

### API Endpoints

All endpoints are organized by feature in `Controllers/`:
- **POST /api/register** - User registration (sends confirmation email)
- **POST /api/login** - Login with email/password (returns access + refresh tokens)
- **POST /api/refresh** - Refresh access token using refresh token
- **POST /api/revoke** - Revoke a refresh token
- **GET /api/confirm-email** - Confirm email via token link

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

Password requirements ([Program.cs:29-30](src/Identity.Api/Program.cs#L29-L30)):
- Minimum 6 characters
- Non-alphanumeric characters NOT required
- Email confirmation required for sign-in ([Program.cs:31](src/Identity.Api/Program.cs#L31))

### Auto-Migration

In development environment, database migrations run automatically on startup ([Program.cs:85-88](src/Identity.Api/Program.cs#L85-L88)).

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

**Error Handling**:
- Custom exceptions per feature (e.g., `UnauthorizedException`, `EmailNotConfirmedException`, `InvalidRefreshTokenException`)
- Exceptions organized in feature-specific `Exceptions/` folders

## Adding New Endpoints

To add a new endpoint following the existing pattern:
1. Create folder in `Controllers/` (e.g., `Controllers/NewFeature/`)
2. Add `NewFeatureController.cs` with route and HTTP method
3. Add `NewFeatureRequest.cs` and `NewFeatureResponse.cs` DTOs
4. Add `NewFeatureRequestHandler.cs` implementing `IRequestHandler<NewFeatureRequest, NewFeatureResponse>`
5. Add any custom exceptions in `Exceptions/` subfolder
6. Request handler will be auto-registered by `AddRequestHandlers()` in [DependencyInjection.cs](src/Identity.Api/DependencyInjection.cs)
