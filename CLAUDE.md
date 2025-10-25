# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

This is an ASP.NET Core 8.0 Identity API service that provides JWT-based authentication. The project uses:
- **ASP.NET Core Identity** for user management
- **Entity Framework Core** with PostgreSQL for data persistence
- **JWT Bearer tokens** for authentication
- **Swagger/OpenAPI** for API documentation

## Architecture

### Project Structure
- `Identity.Api` - Main API project containing:
  - `Controllers/` - API endpoints (AuthController for registration/login)
  - `Models/` - Domain models (ApplicationUser extends IdentityUser)
  - `Persistence/` - Database context and migrations
  - `Abstractions/` - Interface definitions (IApplicationDbContext)

### Key Architectural Patterns
- **Repository Pattern**: IApplicationDbContext abstraction provides a clean separation between the domain and data access layers
- **ASP.NET Core Identity**: Built on top of IdentityDbContext with ApplicationUser as the custom user model
- **JWT Authentication**: Configured with symmetric key signing (HS256), tokens expire after 12 hours
- **Auto-Migration**: In development, database migrations run automatically on startup ([Program.cs:57](src/Identity.Api/Program.cs#L57))

### Authentication Flow
1. User registers via `/api/auth/register` endpoint
2. User logs in via `/api/auth/login` with email/password
3. On successful login, a JWT token is generated with user claims (NameIdentifier, Name)
4. Token must be included in Authorization header for protected endpoints

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

# Update database manually
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

### Required Settings
The application requires the following configuration in `appsettings.Development.json`:
- `ConnectionStrings:IdentityDatabase` - PostgreSQL connection string
- `Jwt:Key` - Secret key for JWT signing (must be at least 256 bits)

### Default Configuration
- PostgreSQL: `localhost:5432`, database: `identity`, user: `postgres`, password: `postgres`
- JWT Key: `a-string-secret-at-least-256-bits-long` (development only)
- Password Requirements: Minimum 6 characters, non-alphanumeric not required ([Program.cs:20-21](src/Identity.Api/Program.cs#L20-L21))

## Database

### Provider
PostgreSQL via Npgsql.EntityFrameworkCore.PostgreSQL

### Context
ApplicationDbContext inherits from IdentityDbContext<ApplicationUser> and implements IApplicationDbContext. It automatically applies all IEntityTypeConfiguration implementations from the assembly.

### Migrations
Located in `src/Identity.Api/Persistence/Migrations/`. In development mode, migrations are applied automatically on application startup.

## API Endpoints

### Authentication
- `POST /api/auth/register` - Register a new user (email, password)
- `POST /api/auth/login` - Login and receive JWT token (email, password)

### Development
API is accessible at `http://localhost:5096` (default development port).
Swagger UI is available at `/swagger` in development environment.

## Notes
- No test projects currently exist in the solution
- JWT validation is configured without issuer/audience validation ([Program.cs:35-36](src/Identity.Api/Program.cs#L35-L36))
- Authentication and authorization middleware order is critical: UseAuthentication() must come before UseAuthorization()
