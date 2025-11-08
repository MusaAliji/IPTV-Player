#!/bin/bash

# Script to publish shared-types package to npm
# Usage: ./publish.sh [patch|minor|major]

set -e

# Colors for output
GREEN='\033[0;32m'
BLUE='\033[0;34m'
RED='\033[0;31m'
YELLOW='\033[1;33m'
NC='\033[0m' # No Color

echo -e "${BLUE}📦 Publishing shared-types package${NC}"
echo ""

# Check if logged in to npm
if ! npm whoami &> /dev/null; then
    echo -e "${RED}❌ Not logged in to npm${NC}"
    echo "Please run: npm login"
    exit 1
fi

# Determine version bump type
VERSION_TYPE=${1:-patch}

if [[ ! "$VERSION_TYPE" =~ ^(patch|minor|major)$ ]]; then
    echo -e "${RED}❌ Invalid version type: $VERSION_TYPE${NC}"
    echo "Usage: ./publish.sh [patch|minor|major]"
    exit 1
fi

echo -e "${YELLOW}🔍 Current version:${NC}"
npm version --no-git-tag-version | grep shared-types || true

# Clean build directory
echo ""
echo -e "${BLUE}🧹 Cleaning build directory...${NC}"
rm -rf dist

# Build the package
echo -e "${BLUE}🔨 Building package...${NC}"
npm run build

if [ ! -d "dist" ] || [ -z "$(ls -A dist)" ]; then
    echo -e "${RED}❌ Build failed - dist directory is empty${NC}"
    exit 1
fi

echo -e "${GREEN}✅ Build successful${NC}"

# Run tests (optional - comment out if no tests)
if [ -f "package.json" ] && grep -q '"test"' package.json; then
    echo ""
    echo -e "${BLUE}🧪 Running tests...${NC}"
    npm test || {
        echo -e "${YELLOW}⚠️  Tests failed, continuing anyway...${NC}"
    }
fi

# Show what will be published
echo ""
echo -e "${BLUE}📋 Files that will be published:${NC}"
npm pack --dry-run 2>&1 | grep -v "npm notice"

# Bump version
echo ""
echo -e "${BLUE}📈 Bumping ${VERSION_TYPE} version...${NC}"
npm version $VERSION_TYPE --no-git-tag-version

NEW_VERSION=$(node -p "require('./package.json').version")
echo -e "${GREEN}New version: $NEW_VERSION${NC}"

# Confirm before publishing
echo ""
echo -e "${YELLOW}⚠️  Ready to publish version $NEW_VERSION to npm${NC}"
read -p "Continue? (y/N) " -n 1 -r
echo
if [[ ! $REPLY =~ ^[Yy]$ ]]; then
    echo -e "${RED}❌ Publishing cancelled${NC}"
    exit 1
fi

# Publish to npm
echo ""
echo -e "${BLUE}🚀 Publishing to npm...${NC}"
npm publish --access public

echo ""
echo -e "${GREEN}✨ Successfully published version $NEW_VERSION!${NC}"
echo ""
echo -e "${BLUE}📝 Next steps:${NC}"
echo "1. Update web project:"
echo "   cd ../web"
echo "   npm update @iptv-player/shared-types"
echo ""
echo "2. Or install specific version:"
echo "   npm install @iptv-player/shared-types@$NEW_VERSION"
echo ""
echo -e "${BLUE}🌐 View on npm:${NC}"
PACKAGE_NAME=$(node -p "require('./package.json').name")
echo "   https://www.npmjs.com/package/$PACKAGE_NAME"
