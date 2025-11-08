# Quick Start: Publishing shared-types

## 🚀 Fast Track (Recommended)

### First Time Setup (One-time only)

```bash
# 1. Go to shared-types directory
cd /home/user/IPTV-Player/shared-types

# 2. Run preparation script
./prepare-publish.sh
# Enter your npm username when prompted

# 3. Review and edit package.json
# Update author, repository URL, etc.

# 4. Login to npm (if not already)
npm login
```

### Publishing Updates

```bash
# Go to shared-types directory
cd /home/user/IPTV-Player/shared-types

# Publish with automatic version bump
./publish.sh patch   # Bug fixes (1.0.0 → 1.0.1)
./publish.sh minor   # New features (1.0.0 → 1.1.0)
./publish.sh major   # Breaking changes (1.0.0 → 2.0.0)
```

### Using in Web Project

```bash
cd /home/user/IPTV-Player/web

# Switch to published package
./switch-shared-types.sh published
# Enter your package name: @username/iptv-shared-types

# Or manually:
npm install @iptv-player/shared-types@npm:@username/iptv-shared-types@latest
```

---

## 📝 Manual Publishing (Without Scripts)

### Prepare Package

```bash
cd /home/user/IPTV-Player/shared-types

# Edit package.json - change name to:
# "@your-username/iptv-shared-types"

# Add to package.json:
"files": ["dist", "README.md"],
"scripts": {
  "prepublishOnly": "npm run build"
}
```

### Publish

```bash
# Login to npm
npm login

# Build
npm run build

# Publish
npm publish --access public
```

### Install in Web

```bash
cd /home/user/IPTV-Player/web

# Using alias (keeps import names the same)
npm install @iptv-player/shared-types@npm:@username/iptv-shared-types@latest

# Or direct install (requires changing all imports)
npm install @username/iptv-shared-types
```

---

## 🔄 Development Workflow

### Local Development (Current Setup)

```bash
# shared-types/package.json
{
  "name": "@iptv-player/shared-types"
}

# web/package.json
{
  "dependencies": {
    "@iptv-player/shared-types": "file:../shared-types"
  }
}

# When you make changes to shared-types:
cd shared-types
npm run build

cd ../web
# Changes are automatically picked up
npm run dev
```

### Switch to Published

```bash
cd web
./switch-shared-types.sh published
# Enter: @username/iptv-shared-types
```

### Switch Back to Local

```bash
cd web
./switch-shared-types.sh local
```

---

## 📦 Package Name Strategies

### Option 1: Scoped Public Package (Recommended)

```json
{
  "name": "@your-username/iptv-shared-types"
}
```

**Pros:**
- Free on npm
- Professional
- No naming conflicts

**Publish:**
```bash
npm publish --access public
```

### Option 2: Unscoped Package

```json
{
  "name": "iptv-shared-types-yourusername"
}
```

**Pros:**
- Simpler name

**Cons:**
- May be taken
- Requires paid npm account for private packages

### Option 3: Keep Same Name (Use Alias)

Keep package named `@iptv-player/shared-types` but install with alias:

```bash
npm install @iptv-player/shared-types@npm:@username/iptv-shared-types
```

**Pros:**
- No code changes needed
- Import statements stay the same

---

## 🔍 Verification

### Check What Will Be Published

```bash
cd shared-types

# See files that will be included
npm pack --dry-run

# Create actual .tgz file to inspect
npm pack
tar -tzf iptv-shared-types-1.0.0.tgz
```

### Verify Published Package

```bash
# View on npm
open https://www.npmjs.com/package/@username/iptv-shared-types

# Check files
npm view @username/iptv-shared-types files

# Check version
npm view @username/iptv-shared-types version

# Download and inspect
npm pack @username/iptv-shared-types
tar -xzf username-iptv-shared-types-1.0.0.tgz
ls package/
```

---

## 🐛 Troubleshooting

### "Package name too similar to existing package"

Change the package name to something more unique:
```json
{
  "name": "@username/iptv-player-shared-types"
}
```

### "You must be logged in to publish packages"

```bash
npm login
# Enter credentials
npm whoami  # Verify login
```

### "You do not have permission to publish"

For scoped packages, add `--access public`:
```bash
npm publish --access public
```

### "Package already exists"

Bump the version:
```bash
npm version patch
npm publish --access public
```

### Web project can't find package after publishing

```bash
# Clear npm cache
npm cache clean --force

# Remove node_modules and reinstall
rm -rf node_modules package-lock.json
npm install
```

### Import errors after switching to published package

If package name changed, update all imports:
```bash
# Find all import statements
grep -r "@iptv-player/shared-types" src/

# Use alias installation to avoid changes:
npm install @iptv-player/shared-types@npm:@newname/iptv-shared-types
```

---

## 📋 Checklist

### Before First Publish

- [ ] Update package.json name
- [ ] Add author information
- [ ] Add repository URL
- [ ] Create .npmignore or set "files" field
- [ ] Build package (`npm run build`)
- [ ] Test package locally (`npm pack`)
- [ ] Login to npm (`npm login`)
- [ ] Review what will be published (`npm pack --dry-run`)

### Every Publish

- [ ] Update version (`npm version patch/minor/major`)
- [ ] Build package (`npm run build`)
- [ ] Run tests (`npm test`)
- [ ] Publish (`npm publish --access public`)
- [ ] Verify on npm website
- [ ] Update web project (`npm update @iptv-player/shared-types`)

---

## 💡 Pro Tips

1. **Use semantic versioning:**
   - `patch`: Bug fixes (1.0.0 → 1.0.1)
   - `minor`: New features (1.0.0 → 1.1.0)
   - `major`: Breaking changes (1.0.0 → 2.0.0)

2. **Add a changelog:**
   Create `CHANGELOG.md` to track changes

3. **Use npm scripts:**
   ```json
   {
     "scripts": {
       "prepublishOnly": "npm run build && npm test",
       "version": "npm run build"
     }
   }
   ```

4. **Test before publishing:**
   ```bash
   npm pack
   cd ../test-project
   npm install ../shared-types/iptv-shared-types-1.0.0.tgz
   ```

5. **Keep local development easy:**
   Use alias installation to keep import names consistent:
   ```bash
   npm install @iptv-player/shared-types@npm:@username/iptv-shared-types
   ```

---

## 🔗 Resources

- **npm Documentation:** https://docs.npmjs.com/
- **Publishing Scoped Packages:** https://docs.npmjs.com/creating-and-publishing-scoped-public-packages
- **Semantic Versioning:** https://semver.org/
- **package.json Fields:** https://docs.npmjs.com/cli/v9/configuring-npm/package-json

---

## Need Help?

See full documentation: `PUBLISHING_GUIDE.md`
