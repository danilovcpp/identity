# Identity API

ASP.NET Core 8.0 Identity API service with JWT authentication, email confirmation, and avatar management.

## Features

- User registration and login with JWT authentication
- Refresh token support with secure hashing
- Email confirmation flow
- Password reset functionality
- **Avatar upload/delete with MinIO storage**
- Clean Architecture with MediatR (CQRS pattern)
- PostgreSQL database with Entity Framework Core
- Swagger/OpenAPI documentation

## Quick Start

### Prerequisites

- .NET 8.0 SDK
- Docker and Docker Compose (for PostgreSQL and MinIO)

### 1. Start Infrastructure Services

```bash
docker-compose up -d
```

This starts:
- PostgreSQL on `localhost:5432`
- MinIO on `localhost:9000` (Console: `localhost:9001`)

### 2. Run the Application

```bash
dotnet run --project src/Identity.Api/Identity.Api.csproj
```

The API will be available at:
- HTTP: `http://localhost:5000`
- HTTPS: `https://localhost:5001`
- Swagger UI: `http://localhost:5000/swagger`

## API Endpoints

### Authentication
- `POST /api/register` - Register new user
- `POST /api/login` - Login with credentials
- `POST /api/refresh` - Refresh access token
- `POST /api/revoke` - Revoke refresh token
- `POST /api/confirm-email` - Confirm email address
- `POST /api/forgot-password` - Request password reset
- `POST /api/reset-password` - Reset password

### Avatar Management
- `POST /api/avatar/upload` - Upload user avatar (authenticated)
- `DELETE /api/avatar` - Delete user avatar (authenticated)

## Documentation

- [CLAUDE.md](CLAUDE.md) - Detailed project documentation and architecture guide
- [AVATAR_FEATURE.md](AVATAR_FEATURE.md) - Avatar upload feature documentation

## Configuration

See [appsettings.Development.json](src/Identity.Api/appsettings.Development.json) for default configuration.

Key settings:
- **Database**: PostgreSQL connection string
- **JWT**: Token settings (secret, lifetime)
- **SMTP**: Email server configuration
- **MinIO**: Object storage for avatars

## Project Structure

```
src/
├── Identity.Api/           # Presentation layer (controllers)
├── Identity.Application/   # Application logic (MediatR handlers)
├── Identity.Domain/        # Domain entities
└── Identity.Infrastructure/ # External services (database, email, storage)
```

## Development

```bash
# Build solution
dotnet build Identity.sln

# Run with hot reload
dotnet watch --project src/Identity.Api/Identity.Api.csproj

# Add migration
dotnet ef migrations add MigrationName --project src/Identity.Infrastructure --startup-project src/Identity.Api

# Update database
dotnet ef database update --project src/Identity.Infrastructure --startup-project src/Identity.Api
```

## License

MIT