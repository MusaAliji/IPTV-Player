# How to Publish to npm - Step by Step

## Prerequisites

1. **Create npm account** (if you don't have one):
   - Go to: https://www.npmjs.com/signup
   - Create a free account
   - Verify your email

## Publishing Steps

### Step 1: Login to npm

Open your terminal and run:

```bash
cd /home/user/IPTV-Player/shared-types
npm login
```

You'll be prompted for:
- **Username**: Your npm username
- **Password**: Your npm password
- **Email**: Your email (must match npm account)
- **One-time password**: If you have 2FA enabled

### Step 2: Test What Will Be Published

Before publishing, see what files will be included:

```bash
npm pack --dry-run
```

This shows you exactly what will be published to npm.

### Step 3: Build the Package

```bash
npm run build
```

This compiles TypeScript to JavaScript in the `dist/` folder.

### Step 4: Publish to npm

**For scoped package (recommended - FREE):**

```bash
npm publish --access public
```

**Note:** Since the package name is `@iptv-player/shared-types` (starts with @), you MUST use `--access public` for free publishing. Without it, npm will try to publish as private (which requires a paid account).

### Step 5: Verify Publication

After publishing, verify it's live:

```bash
# Check on npm website
https://www.npmjs.com/package/@iptv-player/shared-types

# Or use CLI
npm view @iptv-player/shared-types
```

---

## Common Errors and Solutions

### Error: "You must be logged in to publish packages"

**Solution:**
```bash
npm login
npm whoami  # Verify you're logged in
```

### Error: "402 Payment Required"

**Cause:** Trying to publish scoped package as private without paid account

**Solution:** Add `--access public` flag:
```bash
npm publish --access public
```

### Error: "Package name too similar to existing package"

**Solution:** Change the package name in `package.json`:
```json
{
  "name": "@your-username/iptv-shared-types"
}
```
Replace `your-username` with your npm username.

### Error: "You do not have permission to publish"

**Solution:**
1. Check you're logged in: `npm whoami`
2. Make sure package name doesn't conflict with existing packages
3. If using scoped name, ensure scope matches your username or org

### Error: "Cannot publish over previously published version"

**Solution:** Bump the version:
```bash
npm version patch  # 1.0.0 -> 1.0.1
npm publish --access public
```

---

## After Publishing

### Install in Web Project

Once published, install it in your web project:

```bash
cd /home/user/IPTV-Player/web

# Option 1: Keep import names the same (RECOMMENDED)
npm uninstall @iptv-player/shared-types
npm install @iptv-player/shared-types

# Option 2: Or use the published package directly
npm install @iptv-player/shared-types@latest
```

### Future Updates

When you make changes to shared-types:

```bash
cd /home/user/IPTV-Player/shared-types

# 1. Make your code changes
# 2. Bump version
npm version patch  # or minor, or major

# 3. Publish
npm publish --access public

# 4. Update in web project
cd /home/user/IPTV-Player/web
npm update @iptv-player/shared-types
```

---

## Automated Publishing (Alternative)

You can also use the provided script:

```bash
cd /home/user/IPTV-Player/shared-types
./publish.sh patch
```

This automatically:
- Builds the package
- Bumps version
- Publishes to npm
- Shows confirmation

---

## Quick Reference

```bash
# Login (one time)
npm login

# Build
npm run build

# Publish
npm publish --access public

# Update version
npm version patch    # Bug fixes: 1.0.0 → 1.0.1
npm version minor    # New features: 1.0.0 → 1.1.0
npm version major    # Breaking changes: 1.0.0 → 2.0.0

# Check what's published
npm view @iptv-player/shared-types
npm view @iptv-player/shared-types version
npm view @iptv-player/shared-types versions
```

---

## Important Notes

1. **Package Name**: Currently `@iptv-player/shared-types`
   - The `@` makes it a "scoped" package
   - Scoped packages can be published for FREE to npm
   - You MUST use `--access public` when publishing

2. **What Gets Published**: Only these files/folders:
   - `dist/` folder (compiled JavaScript)
   - `README.md`
   - `LICENSE`
   - `package.json`

   Everything else is excluded via `.npmignore`

3. **Version Numbers**: Follow semantic versioning:
   - **1.0.x** - Patch (bug fixes)
   - **1.x.0** - Minor (new features, backward compatible)
   - **x.0.0** - Major (breaking changes)

4. **Publishing is Permanent**: You cannot unpublish after 24 hours (npm policy)

---

## Need Help?

- npm Documentation: https://docs.npmjs.com/
- Publishing Scoped Packages: https://docs.npmjs.com/creating-and-publishing-scoped-public-packages
- Support: https://www.npmjs.com/support
