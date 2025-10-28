# Identity API Specification v1.0

## Base URL
```
Development: http://localhost:5000
Production: https://your-domain.com
```

## Authentication
Most endpoints use JWT Bearer token authentication. Include the access token in the Authorization header:
```
Authorization: Bearer {accessToken}
```

## Content Type
- Default: `application/json`
- Avatar upload: `multipart/form-data`

---

## Endpoints

### 1. Authentication

#### 1.1 Register
Creates a new user account and sends email confirmation.

**Endpoint:** `POST /api/register`

**Authentication:** None

**Request Body:**
```json
{
  "email": "user@example.com",
  "password": "Password123"
}
```

**Response:** `200 OK`
```json
{
  "userId": "550e8400-e29b-41d4-a716-446655440000",
  "message": "User registered successfully. Please check your email to confirm your account."
}
```

**Error Responses:**
- `400 Bad Request` - Validation errors (e.g., invalid email, weak password)
- `409 Conflict` - Email already exists

---

#### 1.2 Login
Authenticates a user and returns access + refresh tokens.

**Endpoint:** `POST /api/login`

**Authentication:** None

**Request Body:**
```json
{
  "email": "user@example.com",
  "password": "Password123"
}
```

**Response:** `200 OK`
```json
{
  "accessToken": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "refreshToken": "a1b2c3d4e5f6...",
  "expiresIn": 900
}
```

**Response Fields:**
- `accessToken` - JWT access token (default: 15 min lifetime)
- `refreshToken` - Refresh token (default: 7 days lifetime)
- `expiresIn` - Access token expiration in seconds

**Error Responses:**
- `400 Bad Request` - Validation errors
- `401 Unauthorized` - Invalid credentials
- `403 Forbidden` - Email not confirmed

---

#### 1.3 Refresh Token
Refreshes an expired access token using a refresh token.

**Endpoint:** `POST /api/refresh`

**Authentication:** None

**Request Body:**
```json
{
  "refreshToken": "a1b2c3d4e5f6..."
}
```

**Response:** `200 OK`
```json
{
  "accessToken": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "refreshToken": "g7h8i9j0k1l2...",
  "expiresIn": 900
}
```

**Error Responses:**
- `400 Bad Request` - Missing refresh token
- `401 Unauthorized` - Invalid, expired, or revoked refresh token

---

#### 1.4 Revoke Token
Revokes a refresh token, preventing further use.

**Endpoint:** `POST /api/revoke`

**Authentication:** None

**Request Body:**
```json
{
  "refreshToken": "a1b2c3d4e5f6..."
}
```

**Response:** `200 OK`
```json
{
  "message": "Refresh token revoked successfully"
}
```

**Error Responses:**
- `400 Bad Request` - Missing refresh token
- `404 Not Found` - Token not found

---

### 2. Email Confirmation

#### 2.1 Confirm Email
Confirms a user's email address using the token sent via email.

**Endpoint:** `GET /api/confirm-email`

**Authentication:** None

**Query Parameters:**
- `userId` (required) - User ID from confirmation link
- `token` (required) - Confirmation token from email

**Example:**
```
GET /api/confirm-email?userId=550e8400-e29b-41d4-a716-446655440000&token=CfDJ8N...
```

**Response:** `200 OK`
```json
{
  "message": "Email confirmed successfully"
}
```

**Error Responses:**
- `400 Bad Request` - Invalid token or user ID
- `404 Not Found` - User not found

---

### 3. Password Management

#### 3.1 Forgot Password
Initiates password reset process and sends reset link via email.

**Endpoint:** `POST /api/forgot-password`

**Authentication:** None

**Request Body:**
```json
{
  "email": "user@example.com"
}
```

**Response:** `200 OK`
```json
{
  "message": "If an account with that email exists, a password reset link has been sent."
}
```

**Note:** Always returns success to prevent email enumeration.

---

#### 3.2 Reset Password
Resets user password using token from email.

**Endpoint:** `POST /api/reset-password`

**Authentication:** None

**Request Body:**
```json
{
  "email": "user@example.com",
  "token": "CfDJ8N...",
  "newPassword": "NewPassword123"
}
```

**Response:** `200 OK`
```json
{
  "message": "Password reset successfully"
}
```

**Error Responses:**
- `400 Bad Request` - Invalid token, validation errors
- `404 Not Found` - User not found

---

#### 3.3 Change Password
Changes password for authenticated user (requires current password).

**Endpoint:** `POST /api/change-password`

**Authentication:** Required (Bearer token)

**Request Body:**
```json
{
  "currentPassword": "OldPassword123",
  "newPassword": "NewPassword123"
}
```

**Response:** `200 OK`
```json
{
  "message": "Password changed successfully"
}
```

**Error Responses:**
- `400 Bad Request` - Validation errors, incorrect current password
- `401 Unauthorized` - Missing or invalid access token

---

### 4. User Profile

#### 4.1 Get User Profile (Public)
Retrieves public profile information for any user.

**Endpoint:** `GET /api/profile/{userId}`

**Authentication:** None

**Path Parameters:**
- `userId` - User ID (GUID)

**Example:**
```
GET /api/profile/550e8400-e29b-41d4-a716-446655440000
```

**Response:** `200 OK`
```json
{
  "id": "550e8400-e29b-41d4-a716-446655440000",
  "email": "user@example.com",
  "userName": "johndoe",
  "avatarUrl": "http://localhost:9000/avatars/550e8400_abc123.jpg",
  "emailConfirmed": true,
  "createdAt": "2025-01-15T10:30:00Z"
}
```

**Error Responses:**
- `404 Not Found` - User not found

---

#### 4.2 Get My Profile
Retrieves authenticated user's profile.

**Endpoint:** `GET /api/profile/me`

**Authentication:** Required (Bearer token)

**Response:** `200 OK`
```json
{
  "id": "550e8400-e29b-41d4-a716-446655440000",
  "email": "user@example.com",
  "userName": "johndoe",
  "avatarUrl": "http://localhost:9000/avatars/550e8400_abc123.jpg",
  "emailConfirmed": true,
  "createdAt": "2025-01-15T10:30:00Z"
}
```

**Error Responses:**
- `401 Unauthorized` - Missing or invalid access token

---

### 5. Avatar Management

#### 5.1 Upload Avatar
Uploads or updates user avatar image.

**Endpoint:** `POST /api/avatar/upload`

**Authentication:** Required (Bearer token)

**Content-Type:** `multipart/form-data`

**Form Data:**
- `file` (required) - Image file (JPEG, PNG, GIF, WebP)
  - Max size: 5 MB
  - Field name: `file`

**Example (using FormData):**
```javascript
const formData = new FormData();
formData.append('file', fileInput.files[0]);
```

**Response:** `200 OK`
```json
{
  "avatarUrl": "http://localhost:9000/avatars/550e8400_abc123.jpg",
  "message": "Avatar uploaded successfully"
}
```

**Error Responses:**
- `400 Bad Request` - Invalid file type, file too large, or no file provided
- `401 Unauthorized` - Missing or invalid access token

**Supported File Types:**
- `image/jpeg`
- `image/png`
- `image/gif`
- `image/webp`

---

#### 5.2 Delete Avatar
Deletes user's avatar.

**Endpoint:** `DELETE /api/avatar`

**Authentication:** Required (Bearer token)

**Response:** `204 No Content`

**Error Responses:**
- `401 Unauthorized` - Missing or invalid access token
- `404 Not Found` - No avatar to delete

---

## Error Response Format

All error responses follow this format:

```json
{
  "type": "https://tools.ietf.org/html/rfc7231#section-6.5.1",
  "title": "One or more validation errors occurred.",
  "status": 400,
  "errors": {
    "Email": ["The Email field is required."],
    "Password": ["Password must be at least 6 characters."]
  }
}
```

For non-validation errors:
```json
{
  "message": "Error description"
}
```

---

## Common HTTP Status Codes

- `200 OK` - Request succeeded
- `204 No Content` - Request succeeded, no response body
- `400 Bad Request` - Validation errors or invalid input
- `401 Unauthorized` - Authentication required or failed
- `403 Forbidden` - Authenticated but not authorized (e.g., email not confirmed)
- `404 Not Found` - Resource not found
- `409 Conflict` - Resource already exists
- `500 Internal Server Error` - Server error

---

## Security Notes

1. **Password Requirements:**
   - Minimum 6 characters
   - Non-alphanumeric characters not required

2. **Token Security:**
   - Access tokens expire in 15 minutes (default)
   - Refresh tokens expire in 7 days (default)
   - Refresh tokens are hashed (SHA256) before storage
   - Always store tokens securely (httpOnly cookies recommended)

3. **Email Confirmation:**
   - Required before login
   - Confirmation link sent via email on registration

4. **CORS:**
   - Configure allowed origins in production
   - Include credentials for authenticated requests

5. **Rate Limiting:**
   - Implement rate limiting on sensitive endpoints (login, register, forgot-password)

---

## JWT Claims

Access tokens contain the following claims:

```json
{
  "sub": "550e8400-e29b-41d4-a716-446655440000",
  "email": "user@example.com",
  "jti": "unique-token-id",
  "exp": 1234567890,
  "iss": "IdentityApi",
  "aud": "IdentityApiUsers"
}
```

- `sub` - User ID (use this for authentication)
- `email` - User email
- `jti` - Token ID
- `exp` - Expiration timestamp
- `iss` - Issuer
- `aud` - Audience

---

## API Flow Examples

### Registration & Login Flow
1. `POST /api/register` - Register user
2. User receives confirmation email
3. `GET /api/confirm-email?userId=...&token=...` - Confirm email
4. `POST /api/login` - Login and receive tokens
5. Use `accessToken` for authenticated requests

### Token Refresh Flow
1. Store `accessToken` and `refreshToken` from login
2. Include `accessToken` in Authorization header
3. When `accessToken` expires (401 error)
4. `POST /api/refresh` with `refreshToken`
5. Update stored tokens
6. Retry failed request with new `accessToken`

### Password Reset Flow
1. `POST /api/forgot-password` - Request reset
2. User receives reset email
3. `POST /api/reset-password` - Reset with token
4. `POST /api/login` - Login with new password

### Avatar Upload Flow
1. Login and obtain `accessToken`
2. `POST /api/avatar/upload` with file in FormData
3. Receive `avatarUrl` in response
4. Display avatar using returned URL
5. `DELETE /api/avatar` - Remove avatar (optional)
