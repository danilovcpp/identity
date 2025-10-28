# Avatar Upload Feature

## Overview

This feature allows authenticated users to upload, update, and delete their profile avatars. Avatar images are stored in MinIO object storage.

## Architecture

### Components

1. **Domain Layer** ([ApplicationUser.cs](src/Identity.Domain/Entities/ApplicationUser.cs))
   - Added `AvatarUrl` property to store the avatar URL

2. **Application Layer** ([Avatar/](src/Identity.Application/Avatar/))
   - `IFileStorageService` - Abstraction for file storage operations
   - `UploadAvatarRequest` / `UploadAvatarRequestHandler` - Handles avatar upload
   - `DeleteAvatarRequest` / `DeleteAvatarRequestHandler` - Handles avatar deletion
   - Validation: file type (JPEG, PNG, GIF, WebP) and size (max 5 MB)

3. **Infrastructure Layer** ([Storage/](src/Identity.Infrastructure/Storage/))
   - `MinioFileStorageService` - MinIO implementation of `IFileStorageService`
   - `MinioSettings` - Configuration model for MinIO connection
   - Auto-creates bucket with public read access on first upload

4. **API Layer** ([AvatarController.cs](src/Identity.Api/Controllers/AvatarController.cs))
   - `POST /api/avatar/upload` - Upload avatar endpoint
   - `DELETE /api/avatar` - Delete avatar endpoint

## Setup

### 1. Start MinIO using Docker Compose

```bash
docker-compose up -d minio
```

This starts:
- **MinIO Server** on `http://localhost:9000`
- **MinIO Console** on `http://localhost:9001`

Login credentials:
- Username: `minioadmin`
- Password: `minioadmin`

### 2. Configuration

MinIO settings in [appsettings.Development.json](src/Identity.Api/appsettings.Development.json):

```json
{
  "Minio": {
    "Endpoint": "localhost:9000",
    "AccessKey": "minioadmin",
    "SecretKey": "minioadmin",
    "BucketName": "avatars",
    "UseSSL": false
  }
}
```

### 3. Apply Database Migration

The `AvatarUrl` field has been added to the `AspNetUsers` table:

```bash
dotnet ef database update --project src/Identity.Infrastructure --startup-project src/Identity.Api
```

Or simply run the application - migrations auto-apply in development mode.

## API Usage

### Upload Avatar

**Endpoint:** `POST /api/avatar/upload`

**Authentication:** Required (JWT Bearer token)

**Content-Type:** `multipart/form-data`

**Request:**
```http
POST /api/avatar/upload HTTP/1.1
Host: localhost:5000
Authorization: Bearer {your_jwt_token}
Content-Type: multipart/form-data; boundary=----WebKitFormBoundary

------WebKitFormBoundary
Content-Disposition: form-data; name="file"; filename="avatar.jpg"
Content-Type: image/jpeg

{binary data}
------WebKitFormBoundary--
```

**Response (200 OK):**
```json
{
  "avatarUrl": "http://localhost:9000/avatars/avatars/user-id_guid.jpg"
}
```

**Error Responses:**
- `400 Bad Request` - No file uploaded
- `400 Bad Request` - Invalid file type (must be JPEG/PNG/GIF/WebP)
- `400 Bad Request` - File too large (max 5 MB)
- `401 Unauthorized` - Missing or invalid JWT token
- `404 Not Found` - User not found

### Delete Avatar

**Endpoint:** `DELETE /api/avatar`

**Authentication:** Required (JWT Bearer token)

**Request:**
```http
DELETE /api/avatar HTTP/1.1
Host: localhost:5000
Authorization: Bearer {your_jwt_token}
```

**Response (204 No Content)**

**Error Responses:**
- `401 Unauthorized` - Missing or invalid JWT token
- `404 Not Found` - User not found

## File Validation

### Allowed File Types
- `image/jpeg` (JPEG/JPG)
- `image/png` (PNG)
- `image/gif` (GIF)
- `image/webp` (WebP)

### File Size Limit
- Maximum: **5 MB**

### Storage Path Format
Files are stored with the following naming convention:
```
avatars/{userId}_{guid}.{extension}
```

Example: `avatars/123e4567-e89b-12d3-a456-426614174000_a1b2c3d4.jpg`

## Testing with cURL

### Upload Avatar
```bash
curl -X POST http://localhost:5000/api/avatar/upload \
  -H "Authorization: Bearer YOUR_JWT_TOKEN" \
  -F "file=@/path/to/avatar.jpg"
```

### Delete Avatar
```bash
curl -X DELETE http://localhost:5000/api/avatar \
  -H "Authorization: Bearer YOUR_JWT_TOKEN"
```

## Testing with Swagger

1. Run the application: `dotnet run --project src/Identity.Api`
2. Navigate to Swagger UI: `http://localhost:5000/swagger`
3. Authenticate:
   - Click "Authorize" button
   - Enter: `Bearer {your_jwt_token}`
   - Click "Authorize"
4. Test endpoints:
   - Expand `POST /api/avatar/upload`
   - Click "Try it out"
   - Select a file
   - Click "Execute"

## MinIO Console Access

Access the MinIO web console at `http://localhost:9001`:

1. Login with credentials: `minioadmin` / `minioadmin`
2. Navigate to "Buckets" to see the `avatars` bucket
3. Browse uploaded files
4. View bucket policies and access settings

## How It Works

### Upload Flow

1. User sends authenticated request with image file
2. `AvatarController` extracts user ID from JWT claims
3. Request dispatched to `UploadAvatarRequestHandler` via MediatR
4. Handler validates:
   - File type (must be image/jpeg, image/png, image/gif, or image/webp)
   - File size (must be ≤ 5 MB)
5. Old avatar is deleted from MinIO (if exists)
6. New file uploaded to MinIO with unique filename
7. Avatar URL generated and saved to database
8. URL returned in response

### Delete Flow

1. User sends authenticated DELETE request
2. `AvatarController` extracts user ID from JWT claims
3. Request dispatched to `DeleteAvatarRequestHandler`
4. Handler deletes file from MinIO
5. `AvatarUrl` field set to `null` in database

### Bucket Auto-Creation

On first upload, `MinioFileStorageService` automatically:
1. Checks if bucket exists
2. Creates bucket if it doesn't exist
3. Sets public read policy on the bucket:
   ```json
   {
     "Version": "2012-10-17",
     "Statement": [
       {
         "Effect": "Allow",
         "Principal": {"AWS": "*"},
         "Action": ["s3:GetObject"],
         "Resource": ["arn:aws:s3:::avatars/*"]
       }
     ]
   }
   ```

This allows avatar URLs to be publicly accessible without authentication.

## Security Considerations

1. **File Type Validation**: Only image files are accepted
2. **File Size Limit**: Prevents large file uploads (DoS protection)
3. **Authentication Required**: Only authenticated users can upload/delete
4. **User Isolation**: Users can only modify their own avatars
5. **Unique Filenames**: GUID prevents filename collisions
6. **Old File Cleanup**: Previous avatar is deleted on update

## Troubleshooting

### MinIO Connection Error
- Ensure MinIO is running: `docker ps | grep minio`
- Check endpoint configuration in appsettings
- Verify network connectivity: `curl http://localhost:9000`

### File Upload Fails
- Check file size (must be ≤ 5 MB)
- Verify file type (JPEG/PNG/GIF/WebP only)
- Ensure user is authenticated (valid JWT token)

### Avatar URL Not Accessible
- Verify bucket policy allows public read access
- Check MinIO console bucket settings
- Ensure correct URL format: `http://localhost:9000/avatars/{filename}`

## Production Considerations

For production deployment:

1. **Use HTTPS**: Set `Minio:UseSSL` to `true`
2. **Change Credentials**: Update `AccessKey` and `SecretKey`
3. **CDN Integration**: Consider placing CDN in front of MinIO
4. **Backup Strategy**: Implement regular backups of MinIO data
5. **Monitoring**: Monitor storage usage and API performance
6. **Image Optimization**: Consider adding image compression/resizing
7. **Content Moderation**: Implement automated content moderation for uploaded images
