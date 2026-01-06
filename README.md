# H3-BioOpg (Cinema App)

Full-stack cinema app:
- Angular 16+ frontend
- ASP.NET Core 8 Web API with EF Core 8 and PostgreSQL
- Uploads microservice (Node/Express + Multer) for poster images

Everything runs via Docker Compose + a Makefile.

## Quick start (Docker)

Prereqs: Docker Desktop

```bash
make up          # start db, api, upload server, and Angular dev server
make seed        # run EF Core migrations + seed data (idempotent)
make open        # macOS convenience: opens Web, Swagger, pgAdmin
```

URLs:
- Website: http://localhost:4200
- API: http://localhost:5099 (Swagger: http://localhost:5099/swagger)
- Uploads: http://localhost:3001
- pgAdmin: http://localhost:5050

Common tasks:
```bash
make ps          # show services
make logs        # tail logs
make rebuild-api # rebuild API image and restart it
make down        # stop services
make clean       # stop & remove volumes (resets DB and uploaded files)
```

## Project layout
- Angular app: `src/`
- API solution: `Backend/` (EF Core, repositories/services/controllers)
- Upload server: `upload-server.js` (serves `uploads/`)
- Compose & Make: `docker-compose.yml`, `Makefile`

## Backend details
- Database: Postgres 16
- EF Core migrations applied on API start; seed controlled by env flags the Makefile passes:
  - `SEED_DB=true`, `SEED_ONLY=true` used by `make seed`
- Seeders populate movies, auditoriums, ticket types, screenings, users, bookings.
- API endpoints (prefix `api/`):
  - `GET/POST/PUT/DELETE /api/movies`
  - `GET/POST/PUT/DELETE /api/screenings`
  - `GET/POST/PUT/DELETE /api/auditoriums`
  - `GET/POST/PUT/DELETE /api/users`
  - `GET /api/users/byEmail?email=...`
  - `GET/POST/PUT/DELETE /api/bookings`

Note: Authentication is currently mock on the frontend (unsigned JWT stored in `localStorage`). The API does not issue or validate JWTs yet.

## Frontend details
- Dev server runs on http://localhost:4200 with a proxy (`proxy.conf.json`) to backend services:
  - `/api` → API (http://api:5099 inside Compose → http://localhost:5099)
  - `/upload`, `/uploads` → Upload server (http://localhost:3001)
  - `/hackapi` → API root (reserved for lab endpoints)
- Routes:
  - `/` browse movies; `/movies/:id` details
  - `/screenings/:id/book` booking screen
  - `/login`, `/register`
  - `/admin/*` for Movies, Screenings, Auditoriums, Users (guarded by role Admin)

## Upload server
- Runs on port 3001
- Static files: `GET /uploads/*`
- Poster upload: `POST /upload/poster` (multipart form-data field `file`, ≤5MB, JPEG/PNG)
- Response: `{ "url": "/uploads/posters/<filename>" }`

```bash
# Angular
npm install
npm start

# API
dotnet restore Backend/BiografWeb.sln
dotnet build Backend/BiografWeb.sln -c Debug
dotnet run --project Backend/BiografWeb.Api/BiografWeb.Api.csproj
```