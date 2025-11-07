# Initialize Git Repository and Push to GitHub

## Step 1: Initialize Git Repository

Open PowerShell in your project root and run:

```powershell
cd D:\Projects\IPTVPlayer\Claude-project

# Initialize git repository
git init

# Configure git (replace with your info)
git config user.name "Your Name"
git config user.email "your.email@example.com"

# Add all files
git add .

# Create initial commit
git commit -m "Initial commit: IPTV Player project documentation and MCP starter files"
```

## Step 2: Create GitHub Repository

1. Go to https://github.com/new
2. Repository name: `IPTV-Player`
3. Description: `Multi-platform IPTV Player with MCP integration - React Native, React, TypeScript, and .NET`
4. Choose Public or Private
5. **DO NOT** initialize with README (we already have one)
6. Click "Create repository"

## Step 3: Connect to GitHub and Push

GitHub will show you commands. Use these:

```powershell
# Add the remote repository (replace YOUR_USERNAME with your GitHub username)
git remote add origin https://github.com/YOUR_USERNAME/IPTV-Player.git

# Rename branch to main (if needed)
git branch -M main

# Push to GitHub
git push -u origin main
```

## Alternative: Using GitHub CLI

If you have GitHub CLI installed:

```powershell
# Login to GitHub
gh auth login

# Create repository and push
gh repo create IPTV-Player --public --source=. --remote=origin --push

# Or for private repo
gh repo create IPTV-Player --private --source=. --remote=origin --push
```

## Step 4: Verify

1. Go to `https://github.com/YOUR_USERNAME/IPTV-Player`
2. You should see all your files

## Troubleshooting

### "fatal: not a git repository"
- Make sure you're in the correct directory
- Run `git init` first

### "remote origin already exists"
```powershell
git remote remove origin
git remote add origin https://github.com/YOUR_USERNAME/IPTV-Player.git
```

### Authentication issues
- Use Personal Access Token instead of password
- Or setup SSH keys: https://docs.github.com/en/authentication/connecting-to-github-with-ssh

## Keeping Repository Updated

As you develop, commit regularly:

```powershell
# Stage changes
git add .

# Commit with message
git commit -m "Phase 1: Completed MCP server setup"

# Push to GitHub
git push
```

## .gitignore Already Included

The starter-files/.gitignore will prevent these from being committed:
- node_modules/
- dist/
- bin/
- obj/
- *.log
- .env files
- Build artifacts

---

**Important:** Replace `YOUR_USERNAME` with your actual GitHub username in all commands!
