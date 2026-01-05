# Dev Setup

## Prerequisites
- Node 18+

## Install
```bash
npm install
```

## Start file-backed API (json-server)
```bash
npm run api
```
Serves REST at http://localhost:3000, persisting to `db.json`.

## Start Angular app with proxy
```bash
npm start
```
Open http://localhost:4200.

Proxy config (`proxy.conf.json`):
- `/api`     → `http://localhost:3000` (json-server, CRUD)
- `/upload`  → `http://localhost:3001` (uploads POST)
- `/uploads` → `http://localhost:3001` (uploads GET)
- `/hackapi` → `http://localhost:5099` (future ASP.NET backend for SQL lab)

## Start upload server (for posters)
```bash
npm run upload
```
Runs on http://localhost:3001. Accepts JPG/PNG ≤ 5MB at `POST /upload/poster`. Returns `{ url }`.

## Notes
- Admin routes (guarded): `/admin/movies`, `/admin/screenings`, `/admin/auditoriums`, `/admin/users`
- Movies: create/edit including poster upload (uses the upload server)
- Users: full CRUD; passwords salted + SHA256 client-side (login verifies the same way)
- Search: mock Elastic (simple client-side filter)
- SQL Injection lab: `/hack/sql` (posts raw SQL to `/hackapi/hack/sql` → requires future backend)

## Swap to real backend later
- Replace json-server with ASP.NET Core Web API
- Keep Angular services; retain `/api/*` route shape
- Add endpoint `POST /hack/sql` (on port 5099 per proxy) to enable SQL lab
- Optional: replace mock JWT with real signed JWT; interceptor already sends `Authorization: Bearer <jwt>`

## Temporary Admin (frontend-only)
To access Admin without known credentials during the frontend phase, set a mock JWT in DevTools console:
```js
const h = btoa(JSON.stringify({alg:'none',typ:'JWT'})).replace(/\+/g,'-').replace(/\//g,'_').replace(/=+$/,'');
const p = btoa(JSON.stringify({sub:'dev-admin',role:'Admin',exp:Math.floor(Date.now()/1000)+3600}))
  .replace(/\+/g,'-').replace(/\//g,'_').replace(/=+$/,'');
localStorage.setItem('auth_token', `${h}.${p}.`);
location.reload();
```
