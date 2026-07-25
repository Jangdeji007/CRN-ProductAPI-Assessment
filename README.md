# CRN Product API

.NET 8 Clean Architecture Product API with JWT authentication and refresh-token rotation.

## Authentication flow (high level)

1. **Register** (`POST /api/auth/register`) or use the seeded admin (`admin@crn.local` / `Admin@123`).
2. **Login** (`POST /api/auth/login`) returns a short-lived **access token** (JWT, ~15 min) and a **refresh token** (~7 days). The refresh token is stored hashed in the database.
3. Call protected endpoints with `Authorization: Bearer {accessToken}`.
4. When the access token expires, call **Refresh** (`POST /api/auth/refresh`) with the refresh token. The API **rotates** tokens: the old refresh token is revoked and a new access + refresh pair is issued.
5. If a **revoked** refresh token is reused (possible theft), all sessions for that user are revoked.
6. **Logout** (`POST /api/auth/logout`) revokes the current refresh token.

### Roles (many-to-many)

Tables: `Role`, `UserRole` (join), `User`.

One user can have multiple roles; each role can be assigned to many users.

| Role   | Access |
|--------|--------|
| Admin  | Create / Update / Delete products (seeded on `admin@crn.local`) |
| Member | Default role for new registrations; product GETs are public |

JWT access tokens include one role claim per assigned role.

## Quick start

1. Set `ConnectionStrings:DefaultConnection` and `Jwt:Key` (min 32 chars) in `src/API/appsettings.Development.json`.
2. Run the API — migrations and admin seed run on startup.
3. Open Swagger, login, authorize with the access token, call product endpoints.
