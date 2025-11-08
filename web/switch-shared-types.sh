#!/bin/bash

# Script to switch between local and published shared-types package
# Usage: ./switch-shared-types.sh [local|published]

set -e

GREEN='\033[0;32m'
BLUE='\033[0;34m'
RED='\033[0;31m'
YELLOW='\033[1;33m'
NC='\033[0m'

MODE=${1:-}

if [ -z "$MODE" ]; then
    echo -e "${BLUE}Current shared-types configuration:${NC}"
    if [ -f "package.json" ]; then
        node -p "JSON.stringify(require('./package.json').dependencies['@iptv-player/shared-types'], null, 2)"
    fi
    echo ""
    echo "Usage: ./switch-shared-types.sh [local|published]"
    echo ""
    echo "Options:"
    echo "  local      - Use local file:../shared-types"
    echo "  published  - Use published npm package"
    exit 0
fi

case $MODE in
    local)
        echo -e "${BLUE}🔗 Switching to local shared-types...${NC}"

        # Check if local package exists
        if [ ! -d "../shared-types" ]; then
            echo -e "${RED}❌ Local shared-types not found at ../shared-types${NC}"
            exit 1
        fi

        # Build local package
        echo -e "${BLUE}🔨 Building local package...${NC}"
        (cd ../shared-types && npm run build)

        # Uninstall current
        npm uninstall @iptv-player/shared-types 2>/dev/null || true

        # Install local
        npm install ../shared-types

        echo -e "${GREEN}✅ Switched to local shared-types${NC}"
        echo -e "${YELLOW}Changes to ../shared-types will require rebuild${NC}"
        ;;

    published)
        echo -e "${BLUE}📦 Switching to published shared-types...${NC}"

        # Ask for package name
        read -p "Enter published package name (e.g., @username/iptv-shared-types): " PACKAGE_NAME

        if [ -z "$PACKAGE_NAME" ]; then
            echo -e "${RED}❌ Package name cannot be empty${NC}"
            exit 1
        fi

        # Uninstall current
        npm uninstall @iptv-player/shared-types 2>/dev/null || true

        # Install published with alias
        echo -e "${BLUE}📥 Installing $PACKAGE_NAME...${NC}"
        npm install @iptv-player/shared-types@npm:$PACKAGE_NAME@latest

        echo -e "${GREEN}✅ Switched to published package: $PACKAGE_NAME${NC}"
        echo -e "${YELLOW}Update with: npm update @iptv-player/shared-types${NC}"
        ;;

    *)
        echo -e "${RED}❌ Invalid mode: $MODE${NC}"
        echo "Usage: ./switch-shared-types.sh [local|published]"
        exit 1
        ;;
esac

echo ""
echo -e "${BLUE}Current configuration:${NC}"
node -p "'@iptv-player/shared-types: ' + JSON.stringify(require('./package.json').dependencies['@iptv-player/shared-types'])"
