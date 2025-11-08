#!/bin/bash

# Script to prepare package.json for publishing
# This updates the package name and adds necessary fields

set -e

GREEN='\033[0;32m'
BLUE='\033[0;34m'
YELLOW='\033[1;33m'
NC='\033[0m'

echo -e "${BLUE}📦 Preparing shared-types for publishing${NC}"
echo ""

# Get current package name
CURRENT_NAME=$(node -p "require('./package.json').name")
echo -e "Current package name: ${YELLOW}$CURRENT_NAME${NC}"
echo ""

# Ask for npm username
read -p "Enter your npm username (or organization): " NPM_USERNAME

if [ -z "$NPM_USERNAME" ]; then
    echo "Username cannot be empty"
    exit 1
fi

NEW_NAME="@$NPM_USERNAME/iptv-shared-types"

echo ""
echo -e "New package name will be: ${GREEN}$NEW_NAME${NC}"
read -p "Continue? (y/N) " -n 1 -r
echo

if [[ ! $REPLY =~ ^[Yy]$ ]]; then
    echo "Cancelled"
    exit 1
fi

# Backup package.json
cp package.json package.json.backup

# Update package.json using Node
node << EOF
const fs = require('fs');
const pkg = JSON.parse(fs.readFileSync('package.json', 'utf8'));

pkg.name = '$NEW_NAME';

// Add files field if not present
if (!pkg.files) {
    pkg.files = ['dist', 'README.md', 'LICENSE'];
}

// Add prepublishOnly script
if (!pkg.scripts.prepublishOnly) {
    pkg.scripts.prepublishOnly = 'npm run build';
}

// Add repository if not present
if (!pkg.repository) {
    pkg.repository = {
        type: 'git',
        url: 'https://github.com/$NPM_USERNAME/IPTV-Player.git',
        directory: 'shared-types'
    };
}

fs.writeFileSync('package.json', JSON.stringify(pkg, null, 2) + '\n');
EOF

echo ""
echo -e "${GREEN}✅ package.json updated${NC}"
echo ""
echo -e "${BLUE}Next steps:${NC}"
echo "1. Review package.json and update author, repository, etc."
echo "2. Create .npmignore file (if needed)"
echo "3. Login to npm: npm login"
echo "4. Publish: ./publish.sh"
echo ""
echo -e "${YELLOW}Backup saved as: package.json.backup${NC}"
