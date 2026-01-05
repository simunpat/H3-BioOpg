# H3-BioOpg

Angular frontend for a cinema app. Uses:
- Mock REST API via json-server (`/api` → http://localhost:3000, backed by `db.json`)
- Upload server for posters on http://localhost:3001
- Mock unsigned JWT (header.payload.) stored in `localStorage`
- SQL Injection Lab page (frontend wired, backend to be added later)

## Getting started

1) Install
```bash
npm install
```

2) Start mock API (json-server)
```bash
npm run api
# serves http://localhost:3000 from db.json
```

3) Start upload server (for poster images)
```bash
npm run upload
# serves http://localhost:3001
```

4) Start Angular dev server (with proxy)
```bash
npm start
# http://localhost:4200, proxies:
#   /api     -> http://localhost:3000
#   /upload  -> http://localhost:3001
#   /uploads -> http://localhost:3001
#   /hackapi -> http://localhost:5099 (future backend)
```

## Features (current)
- Movies browse and details
- Admin: Movies, Screenings, Auditoriums, Users (CRUD)
- Register/Login with client-side salted SHA256 verification
- Mock JWT: Authorization header carries a JWT-shaped string (alg "none")
- Search: simple local filter (mock Elastic)
- SQL Injection Lab page at `/hack/sql` (posts raw SQL to `/hackapi/hack/sql`; backend to be implemented)

## Admin access in frontend-only mode
Until a real backend exists, you can create a temporary admin token in the browser console:
```js
// Create a mock unsigned JWT with role Admin (valid for 1 hour)
const h = btoa(JSON.stringify({alg:'none',typ:'JWT'})).replace(/\+/g,'-').replace(/\//g,'_').replace(/=+$/,'');
const p = btoa(JSON.stringify({sub:'dev-admin',role:'Admin',exp:Math.floor(Date.now()/1000)+3600}))
  .replace(/\+/g,'-').replace(/\//g,'_').replace(/=+$/,'');
localStorage.setItem('auth_token', `${h}.${p}.`);
location.reload();
```
After this, open Admin links from the header.

## Build
```bash
npm run build
```

## Notes
- Swap json-server for ASP.NET + MSSQL later (keep `/api/*` routes)
- Implement the SQL endpoint at `POST http://localhost:5099/hack/sql` to enable the `/hack/sql` page
