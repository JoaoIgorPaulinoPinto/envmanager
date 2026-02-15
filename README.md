# envmanager

A .NET API for managing project environment variables with user auth, project scoping, and team invitations.

## Features
- JWT authentication with refresh token flow
- User registration and listing
- Project creation and retrieval
- Update project variables, name, and description
- Project invitations with accept/decline
- MongoDB persistence

## Tech Stack
- .NET 10 (ASP.NET Core)
- MongoDB
- JWT Bearer auth
- Scalar/OpenAPI for API docs (development only)

## Quickstart
1. Install prerequisites
   - .NET 10 SDK
   - MongoDB (local or remote)
2. Configure settings
   - Edit `appsettings.json` with your MongoDB connection and JWT key
3. Run the API

```bash
# from the repo root

dotnet restore
dotnet run
```

The API will start and expose controllers under the routes listed below.

## Configuration
`appsettings.json`:
- `ConnectionStrings:MongoDb` MongoDB connection string
- `ConnectionStrings:DataBaseName` Database name
- `Jwt:Key` Secret key used to sign JWTs (replace the default)
- `Jwt:Issuer` Issuer/audience for JWT validation

## API Routes
Base routes (all JSON):
- `POST /auth` Login
- `POST /auth/refresh` Refresh access token
- `GET /user` List users (auth required)
- `GET /user/{id}` Get user by id (auth required)
- `POST /user` Create user
- `POST /project` Create project (auth required)
- `GET /project` List projects (auth required)
- `GET /project/{id}` Get project by id (auth required)
- `PUT /project/variables` Update project variables (auth required)
- `PUT /project/update` Update project name/description (auth required)
- `POST /invite` Create project invite (auth required)
- `POST /invite/answer` Accept/decline invite (auth required)

For example requests, see `envmanager.http`.

## Example Requests
The full set of requests is in `envmanager.http`. Here is a minimal flow:

```http
### Create user
POST {{envmanager_HostAddress}}/user
Content-Type: application/json
{
  "user_name": "Jane Doe",
  "email": "jane@example.com",
  "password": "123456"
}

### Login
POST {{envmanager_HostAddress}}/auth
Content-Type: application/json
{
  "email": "jane@example.com",
  "password": "123456"
}
```

## Development Notes
- OpenAPI/Scalar docs are only enabled in `Development`.
- Refresh tokens are issued via HTTP-only cookies.

## Project Structure
- `src/app/controllers` HTTP controllers
- `src/services` Use cases and interfaces
- `src/data` Data layer and repositories
- `src/infra` Infrastructure (DB)
- `src/middleware` Middleware and exception handling
