# Testing Guide - Avatar Upload Feature

## Quick Test Scenario

### Step 1: Start Infrastructure

```bash
# Start PostgreSQL and MinIO
docker-compose up -d

# Verify services are running
docker ps
```

Expected output:
- `identity-postgres` running on port 5432
- `identity-minio` running on ports 9000, 9001

### Step 2: Run the Application

```bash
dotnet run --project src/Identity.Api/Identity.Api.csproj
```

Application will be available at:
- HTTP: http://localhost:5000
- Swagger: http://localhost:5000/swagger

### Step 3: Register a Test User

**Request:**
```bash
curl -X POST http://localhost:5000/api/register \
  -H "Content-Type: application/json" \
  -d '{
    "email": "test@example.com",
    "password": "Test123",
    "firstName": "Test",
    "lastName": "User"
  }'
```

**Response:**
```json
{
  "message": "User registered successfully. Please check your email to confirm your account."
}
```

Check console logs for confirmation link (in development mode).

### Step 4: Confirm Email

Copy the confirmation link from console logs and execute:

```bash
curl -X POST "http://localhost:5000/api/confirm-email?userId={userId}&token={token}"
```

**Response:**
```json
{
  "message": "Email confirmed successfully."
}
```

### Step 5: Login

```bash
curl -X POST http://localhost:5000/api/login \
  -H "Content-Type: application/json" \
  -d '{
    "email": "test@example.com",
    "password": "Test123"
  }'
```

**Response:**
```json
{
  "accessToken": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "refreshToken": "randomstring...",
  "expiresAt": "2024-01-01T12:30:00Z"
}
```

**Save the `accessToken` for next steps!**

### Step 6: Upload Avatar

Prepare a test image file (e.g., `avatar.jpg`) and upload:

```bash
curl -X POST http://localhost:5000/api/avatar/upload \
  -H "Authorization: Bearer YOUR_ACCESS_TOKEN_HERE" \
  -F "file=@avatar.jpg"
```

**Response:**
```json
{
  "avatarUrl": "http://localhost:9000/avatars/avatars/userId_guid.jpg"
}
```

### Step 7: Verify Avatar in MinIO

1. Open MinIO Console: http://localhost:9001
2. Login with credentials:
   - Username: `minioadmin`
   - Password: `minioadmin`
3. Navigate to **Buckets** → **avatars**
4. You should see your uploaded file

### Step 8: Access Avatar URL

Open the avatar URL from Step 6 response in your browser. The image should be publicly accessible.

```bash
# Or test with curl
curl -I http://localhost:9000/avatars/avatars/userId_guid.jpg
```

Expected: HTTP 200 OK

### Step 9: Update Avatar

Upload a different image:

```bash
curl -X POST http://localhost:5000/api/avatar/upload \
  -H "Authorization: Bearer YOUR_ACCESS_TOKEN_HERE" \
  -F "file=@new-avatar.png"
```

The old avatar will be automatically deleted from MinIO.

### Step 10: Delete Avatar

```bash
curl -X DELETE http://localhost:5000/api/avatar \
  -H "Authorization: Bearer YOUR_ACCESS_TOKEN_HERE"
```

**Response:** HTTP 204 No Content

Verify in MinIO Console that the file has been deleted.

## Testing with Swagger UI

### 1. Open Swagger
Navigate to: http://localhost:5000/swagger

### 2. Register User
- Expand `POST /api/register`
- Click **Try it out**
- Fill in the request body
- Click **Execute**

### 3. Confirm Email
Check console logs for confirmation link, then:
- Expand `POST /api/confirm-email`
- Click **Try it out**
- Enter `userId` and `token` from link
- Click **Execute**

### 4. Login
- Expand `POST /api/login`
- Click **Try it out**
- Enter credentials
- Click **Execute**
- **Copy the `accessToken` from response**

### 5. Authorize in Swagger
- Click the **Authorize** button (🔓 icon at top right)
- Enter: `Bearer YOUR_ACCESS_TOKEN`
- Click **Authorize**
- Click **Close**

### 6. Upload Avatar
- Expand `POST /api/avatar/upload`
- Click **Try it out**
- Click **Choose File** and select an image
- Click **Execute**
- Copy the `avatarUrl` from response

### 7. Open Avatar URL
Open the URL from Step 6 in a new browser tab to view the image.

### 8. Delete Avatar
- Expand `DELETE /api/avatar`
- Click **Try it out**
- Click **Execute**

## Error Testing Scenarios

### Test 1: Upload Without Authentication
```bash
curl -X POST http://localhost:5000/api/avatar/upload \
  -F "file=@avatar.jpg"
```

Expected: **401 Unauthorized**

### Test 2: Upload Invalid File Type
```bash
curl -X POST http://localhost:5000/api/avatar/upload \
  -H "Authorization: Bearer YOUR_TOKEN" \
  -F "file=@document.pdf"
```

Expected: **400 Bad Request** - "Invalid file type. Only image files (JPEG, PNG, GIF, WebP) are allowed."

### Test 3: Upload Large File (>5MB)
Create a large file:
```bash
# Create a 6MB file for testing
dd if=/dev/zero of=large.jpg bs=1M count=6
```

Upload:
```bash
curl -X POST http://localhost:5000/api/avatar/upload \
  -H "Authorization: Bearer YOUR_TOKEN" \
  -F "file=@large.jpg"
```

Expected: **400 Bad Request** - "File size exceeds the maximum allowed size of 5 MB."

### Test 4: Upload Without File
```bash
curl -X POST http://localhost:5000/api/avatar/upload \
  -H "Authorization: Bearer YOUR_TOKEN"
```

Expected: **400 Bad Request** - "No file uploaded."

### Test 5: Delete When No Avatar Exists
```bash
curl -X DELETE http://localhost:5000/api/avatar \
  -H "Authorization: Bearer YOUR_TOKEN"
```

Expected: **204 No Content** (idempotent operation)

## Database Verification

Connect to PostgreSQL and verify avatar URL is stored:

```bash
docker exec -it identity-postgres psql -U postgres -d identity

# In PostgreSQL shell:
SELECT "Id", "Email", "AvatarUrl" FROM "AspNetUsers";
```

Expected output:
```
                  Id                  |       Email        |                   AvatarUrl
--------------------------------------+--------------------+-----------------------------------------------
 123e4567-e89b-12d3-a456-426614174000 | test@example.com   | http://localhost:9000/avatars/avatars/123...
```

## Performance Testing

### Upload 10 Avatars Sequentially
```bash
for i in {1..10}; do
  echo "Upload #$i"
  curl -X POST http://localhost:5000/api/avatar/upload \
    -H "Authorization: Bearer YOUR_TOKEN" \
    -F "file=@avatar.jpg" \
    -w "\nTime: %{time_total}s\n\n"
done
```

Monitor:
- Response times
- MinIO Console (only 1 file should remain - old ones deleted)
- Database (AvatarUrl updated each time)

## Cleanup

### Stop Services
```bash
docker-compose down
```

### Remove Volumes (⚠️ Deletes all data)
```bash
docker-compose down -v
```

### Remove Test Files
```bash
rm large.jpg  # if created for testing
```

## Troubleshooting

### MinIO Not Starting
```bash
docker logs identity-minio
```

### PostgreSQL Connection Error
```bash
docker logs identity-postgres
```

### Avatar Upload Fails
1. Check MinIO is running: `docker ps | grep minio`
2. Verify MinIO endpoint: http://localhost:9000
3. Check application logs for detailed error
4. Verify JWT token is valid (not expired)

### Avatar Not Publicly Accessible
1. Check MinIO Console → Buckets → avatars → Access Policy
2. Should show "Custom" policy allowing public read
3. Verify bucket policy in MinIO Console

## Next Steps

After testing the avatar feature:
1. Review implementation in [AVATAR_FEATURE.md](AVATAR_FEATURE.md)
2. Check architecture in [CLAUDE.md](CLAUDE.md)
3. Explore MinIO Console for advanced bucket management
4. Consider adding image resizing/optimization in production
