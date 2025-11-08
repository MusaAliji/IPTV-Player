# Environment Configuration Guide

## Overview

The web application uses environment variables to configure the backend API URL for different environments (development, production).

## Environment Files

### `.env.development`
Used when running `npm run dev`
```bash
VITE_API_URL=https://localhost:58564
```

### `.env.production`
Used when running `npm run build`
```bash
VITE_API_URL=https://localhost:58564
```

### `.env.example`
Template file for reference (not used directly)

## How It Works

### Development Mode
When you run `npm run dev`:
1. Vite loads variables from `.env.development`
2. The proxy in `vite.config.ts` forwards `/api/*` requests to `VITE_API_URL`
3. Your app makes requests to `/api/...` which are proxied to the backend
4. Self-signed SSL certificates are accepted (`secure: false`)

Example:
```
Browser request: http://localhost:3000/api/content
Proxied to: https://localhost:58564/api/content
```

### Production Build
When you run `npm run build`:
1. Vite loads variables from `.env.production`
2. The API service uses the full `VITE_API_URL` directly
3. No proxy is used - requests go directly to the backend

Example:
```
Browser request: https://localhost:58564/api/content
Direct connection to backend
```

## Changing the Backend URL

### Method 1: Edit Environment Files (Recommended)

**For Development:**
Edit `.env.development`:
```bash
VITE_API_URL=https://your-backend-url:port
```

**For Production:**
Edit `.env.production`:
```bash
VITE_API_URL=https://your-production-api.com
```

### Method 2: Local Override (Not Committed)

Create a `.env.local` file (gitignored):
```bash
VITE_API_URL=https://192.168.1.100:58564
```

This overrides both development and production settings.

## Common Scenarios

### 1. Different Backend for Development vs Production
**.env.development:**
```bash
VITE_API_URL=https://localhost:58564
```

**.env.production:**
```bash
VITE_API_URL=https://api.production.com
```

### 2. Using Local Backend on Different Port
**.env.development:**
```bash
VITE_API_URL=https://localhost:8080
```

### 3. Backend on Different Machine (Same Network)
**.env.development:**
```bash
VITE_API_URL=https://192.168.1.50:58564
```

### 4. Remote Development Server
**.env.development:**
```bash
VITE_API_URL=https://dev-api.yourcompany.com
```

## SSL/HTTPS Configuration

The application is configured to work with HTTPS backends, including those with self-signed certificates:

- **Development Proxy:** `secure: false` allows self-signed certificates
- **Production:** Browser's built-in SSL validation applies

## Testing Your Configuration

### 1. Check Environment Variables
Add this to any component temporarily:
```typescript
console.log('API URL:', import.meta.env.VITE_API_URL);
console.log('Mode:', import.meta.env.MODE);
console.log('Prod:', import.meta.env.PROD);
```

### 2. Test Development Server
```bash
npm run dev
```
Check browser console for API URL and test requests.

### 3. Test Production Build
```bash
npm run build
npm run preview
```
Check that API requests go to the correct production URL.

### 4. Verify Proxy is Working
Open browser DevTools → Network tab
Look for requests to `/api/*` - they should be proxied to your backend URL.

## Troubleshooting

### Issue: "Failed to fetch" or CORS errors
**Solution:**
1. Verify backend is running at the configured URL
2. Check backend CORS settings allow requests from `http://localhost:3000`
3. Verify SSL certificate is valid or proxy has `secure: false`

### Issue: Environment variables not updating
**Solution:**
1. Stop the dev server (`Ctrl+C`)
2. Restart: `npm run dev`
3. Vite only loads environment variables on startup

### Issue: 404 errors for API endpoints
**Solution:**
1. Check the API endpoint paths in `shared-types/src/constants/api.constants.ts`
2. Verify your backend API structure matches the expected paths
3. Check if backend requires `/api/v1` prefix or different base path

### Issue: Self-signed certificate errors
**Solution:**
The proxy is configured with `secure: false` which should accept self-signed certificates.
If issues persist:
1. Check browser console for specific SSL errors
2. Try accessing `https://localhost:58564` directly in browser
3. Accept the certificate warning if prompted

## Environment Variables Reference

| Variable | Description | Default |
|----------|-------------|---------|
| `VITE_API_URL` | Backend API URL | `https://localhost:58564` |
| `MODE` | Build mode (automatic) | `development` or `production` |
| `PROD` | Production flag (automatic) | `false` in dev, `true` in build |

## Files Modified

- `web/.env.development` - Development environment variables
- `web/.env.production` - Production environment variables
- `web/.env.example` - Example template
- `web/vite.config.ts` - Vite configuration with environment loading
- `web/src/services/api.ts` - API service with environment-based URL
- `web/src/vite-env.d.ts` - TypeScript definitions for env variables
- `web/.gitignore` - Ignores local environment overrides

## Security Notes

1. ✅ `.env.development` and `.env.production` are committed to git
2. ✅ `.env.local` is gitignored - use for local testing
3. ⚠️ Don't put secrets in environment files - only URLs
4. ⚠️ Backend API keys should be in backend config, not frontend

## Next Steps

1. Start the dev server: `npm run dev`
2. Open browser to: `http://localhost:3000`
3. Check Network tab to verify API requests
4. Adjust `.env.development` if needed

For production deployment, update `.env.production` with your production API URL before building.
