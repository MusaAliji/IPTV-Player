# Publishing shared-types Package

## Complete Guide to Publishing and Using Published Package

---

## Option 1: Publish to npm Public Registry

### Step 1: Prepare the Package

**1.1. Update package.json with proper metadata:**

```bash
cd /home/user/IPTV-Player/shared-types
```

Edit `package.json`:

```json
{
  "name": "@your-username/iptv-shared-types",
  "version": "1.0.0",
  "description": "Shared TypeScript types, constants, and utilities for IPTV Player",
  "type": "module",
  "main": "dist/index.js",
  "module": "dist/index.js",
  "types": "dist/index.d.ts",
  "exports": {
    ".": {
      "import": "./dist/index.js",
      "types": "./dist/index.d.ts"
    }
  },
  "files": [
    "dist",
    "README.md",
    "LICENSE"
  ],
  "repository": {
    "type": "git",
    "url": "https://github.com/your-username/IPTV-Player.git",
    "directory": "shared-types"
  },
  "keywords": [
    "iptv",
    "types",
    "typescript",
    "shared",
    "streaming"
  ],
  "author": "Your Name <your.email@example.com>",
  "license": "MIT",
  "scripts": {
    "build": "tsc",
    "prepublishOnly": "npm run build",
    "test": "jest",
    "test:watch": "jest --watch",
    "test:coverage": "jest --coverage"
  },
  "devDependencies": {
    "@types/jest": "^29.5.0",
    "@types/node": "^20.0.0",
    "jest": "^29.5.0",
    "ts-jest": "^29.1.0",
    "typescript": "^5.0.0"
  }
}
```

**Important Changes:**
- Change `@iptv-player/shared-types` to `@your-username/iptv-shared-types`
- Add `"files"` field to specify what to publish
- Add `"prepublishOnly"` script to build before publishing
- Add repository, author, and proper metadata

**1.2. Create .npmignore file:**

```bash
cat > .npmignore << 'EOF'
# Source files
src/
tests/
*.test.ts

# Config files
tsconfig.json
jest.config.js
.gitignore

# Development
node_modules/
.vscode/
.idea/

# Build artifacts
*.tsbuildinfo
npm-debug.log*
yarn-debug.log*
yarn-error.log*
EOF
```

**1.3. Build the package:**

```bash
npm run build
```

**1.4. Test the package locally:**

```bash
npm pack
# This creates a .tgz file you can inspect
```

---

### Step 2: Publish to npm

**2.1. Create npm account (if you don't have one):**

Visit: https://www.npmjs.com/signup

**2.2. Login to npm:**

```bash
npm login
```

Enter your npm credentials.

**2.3. Publish the package:**

For scoped public package (free):
```bash
npm publish --access public
```

For unscoped package (requires paid account):
```bash
npm publish
```

**2.4. Verify publication:**

Visit: `https://www.npmjs.com/package/@your-username/iptv-shared-types`

---

### Step 3: Update Web Project to Use Published Package

**3.1. Remove local dependency:**

```bash
cd /home/user/IPTV-Player/web
npm uninstall @iptv-player/shared-types
```

**3.2. Install published package:**

```bash
npm install @your-username/iptv-shared-types
```

**3.3. Update imports (if package name changed):**

Replace all imports in your code:

```bash
# Find all files with the old import
grep -r "@iptv-player/shared-types" src/

# Replace (use sed or manually update):
find src/ -type f -name "*.ts" -o -name "*.tsx" | \
  xargs sed -i 's/@iptv-player\/shared-types/@your-username\/iptv-shared-types/g'
```

**Or create a type alias in package.json:**

You can keep using the old name by using npm aliases:

```bash
npm install @iptv-player/shared-types@npm:@your-username/iptv-shared-types
```

This way, no code changes are needed!

---

## Option 2: Publish to GitHub Packages (Private)

### Step 1: Configure for GitHub Packages

**1.1. Update package.json:**

```json
{
  "name": "@MusaAliji/iptv-shared-types",
  "version": "1.0.0",
  "repository": {
    "type": "git",
    "url": "https://github.com/MusaAliji/IPTV-Player.git"
  },
  "publishConfig": {
    "registry": "https://npm.pkg.github.com"
  }
}
```

**1.2. Create .npmrc in shared-types directory:**

```bash
cat > .npmrc << 'EOF'
@MusaAliji:registry=https://npm.pkg.github.com
EOF
```

**1.3. Authenticate with GitHub:**

Create a Personal Access Token (PAT) with `write:packages` permission:
- Go to GitHub Settings → Developer settings → Personal access tokens
- Generate new token with `write:packages` scope

```bash
npm login --scope=@MusaAliji --registry=https://npm.pkg.github.com
# Username: your-github-username
# Password: your-personal-access-token
# Email: your-email
```

**1.4. Publish:**

```bash
npm publish
```

### Step 2: Use in Web Project

**2.1. Create .npmrc in web directory:**

```bash
cd /home/user/IPTV-Player/web
cat > .npmrc << 'EOF'
@MusaAliji:registry=https://npm.pkg.github.com
//npm.pkg.github.com/:_authToken=${GITHUB_TOKEN}
EOF
```

**2.2. Set environment variable:**

```bash
export GITHUB_TOKEN=your-personal-access-token
```

Or create `.env.local`:

```bash
GITHUB_TOKEN=your-personal-access-token
```

**2.3. Install the package:**

```bash
npm uninstall @iptv-player/shared-types
npm install @MusaAliji/iptv-shared-types
```

---

## Option 3: Use Verdaccio (Private npm Registry)

### Step 1: Setup Verdaccio

```bash
# Install globally
npm install -g verdaccio

# Run verdaccio
verdaccio
# Runs on http://localhost:4873
```

### Step 2: Publish to Verdaccio

```bash
cd /home/user/IPTV-Player/shared-types

# Add user
npm adduser --registry http://localhost:4873

# Publish
npm publish --registry http://localhost:4873
```

### Step 3: Use in Web Project

```bash
cd /home/user/IPTV-Player/web

# Install from Verdaccio
npm install @iptv-player/shared-types --registry http://localhost:4873
```

Or create `.npmrc`:

```
registry=http://localhost:4873
```

---

## Automated Publishing Script

Create `shared-types/publish.sh`:

```bash
#!/bin/bash

set -e

echo "🔨 Building package..."
npm run build

echo "✅ Running tests..."
npm test

echo "📦 Packing package..."
npm pack --dry-run

echo "🚀 Publishing to npm..."
npm publish --access public

echo "✨ Done! Package published successfully"
echo "Install with: npm install @your-username/iptv-shared-types"
```

Make it executable:

```bash
chmod +x publish.sh
```

Use it:

```bash
./publish.sh
```

---

## Version Management

### Publishing Updates

**1. Update version:**

```bash
cd /home/user/IPTV-Player/shared-types

# Patch version (1.0.0 → 1.0.1)
npm version patch

# Minor version (1.0.0 → 1.1.0)
npm version minor

# Major version (1.0.0 → 2.0.0)
npm version major
```

**2. Publish:**

```bash
npm publish --access public
```

**3. Update in web project:**

```bash
cd /home/user/IPTV-Player/web

# Update to latest
npm update @your-username/iptv-shared-types

# Or install specific version
npm install @your-username/iptv-shared-types@1.0.1
```

---

## Keeping Both Options (Local + Published)

You can support both local development and published package:

### In shared-types package.json:

```json
{
  "name": "@your-username/iptv-shared-types",
  "version": "1.0.0"
}
```

### In web package.json:

**For local development:**
```json
{
  "dependencies": {
    "@iptv-player/shared-types": "file:../shared-types"
  }
}
```

**For production:**
```json
{
  "dependencies": {
    "@iptv-player/shared-types": "npm:@your-username/iptv-shared-types@^1.0.0"
  }
}
```

### Use npm script to switch:

```json
{
  "scripts": {
    "use-local": "npm install @iptv-player/shared-types@file:../shared-types",
    "use-published": "npm install @iptv-player/shared-types@npm:@your-username/iptv-shared-types@latest"
  }
}
```

---

## Continuous Integration (CI/CD)

### GitHub Actions Workflow

Create `.github/workflows/publish-shared-types.yml`:

```yaml
name: Publish shared-types

on:
  push:
    tags:
      - 'shared-types-v*'

jobs:
  publish:
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v3

      - uses: actions/setup-node@v3
        with:
          node-version: '18'
          registry-url: 'https://registry.npmjs.org'

      - name: Install dependencies
        working-directory: ./shared-types
        run: npm ci

      - name: Build
        working-directory: ./shared-types
        run: npm run build

      - name: Test
        working-directory: ./shared-types
        run: npm test

      - name: Publish to npm
        working-directory: ./shared-types
        run: npm publish --access public
        env:
          NODE_AUTH_TOKEN: ${{ secrets.NPM_TOKEN }}
```

**To publish:**

```bash
cd /home/user/IPTV-Player/shared-types
git tag shared-types-v1.0.0
git push origin shared-types-v1.0.0
```

---

## Recommended Approach

For your use case, I recommend:

### Development Phase (Now):
✅ Keep using `"@iptv-player/shared-types": "file:../shared-types"`
- Fast iteration
- No need to publish for every change
- Easier debugging

### Production/Deployment:
✅ Publish to npm public registry as `@your-username/iptv-shared-types`
- Version control
- Easy distribution
- Can be used in multiple projects

### Best of Both:
Use npm alias to keep your import names consistent:

```bash
npm install @iptv-player/shared-types@npm:@your-username/iptv-shared-types@latest
```

This way:
- Code stays the same (still imports from `@iptv-player/shared-types`)
- You get the published package benefits
- Easy to switch back to local for development

---

## Quick Start Commands

### Publish to npm (Recommended):

```bash
# 1. Update package name in shared-types/package.json
# Change to: "@your-username/iptv-shared-types"

# 2. Login to npm
cd /home/user/IPTV-Player/shared-types
npm login

# 3. Build and publish
npm run build
npm publish --access public

# 4. Install in web project
cd /home/user/IPTV-Player/web
npm install @iptv-player/shared-types@npm:@your-username/iptv-shared-types@latest
```

No code changes needed with the alias approach!

---

Let me know which approach you'd like to use, and I can help you implement it!
