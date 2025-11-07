# IPTV MCP Server - Phase 1 Setup Instructions

## Quick Start Guide

Follow these steps to get your MCP server up and running.

### Step 1: Copy Files

Copy all files from the `starter-files/backend/IPTV.MCP/` directory to your actual project:

```bash
# From your project root
D:\Projects\IPTVPlayer\
```

Create the directory structure:
```bash
mkdir backend\IPTV.MCP
```

Copy these files:
- `server.ts`
- `package.json`
- `tsconfig.json`

### Step 2: Install Dependencies

```bash
cd backend\IPTV.MCP
npm install
```

This will install:
- `@modelcontextprotocol/sdk` - MCP framework
- `axios` - HTTP client
- `zod` - Schema validation
- `typescript` - TypeScript compiler
- `@types/node` - Node.js type definitions

### Step 3: Build the Server

```bash
npm run build
```

You should see a `dist/` folder created with `server.js` inside.

### Step 4: Test the Server

Open a terminal and run:

```bash
npm start
```

You should see: `IPTV MCP Server running on stdio`

Press `Ctrl+C` to stop.

### Step 5: Configure Claude Desktop

1. Open your Claude Desktop configuration file:
   - Windows: `%APPDATA%\Claude\claude_desktop_config.json`
   - Create it if it doesn't exist

2. Add this configuration:

```json
{
  "mcpServers": {
    "iptv": {
      "command": "node",
      "args": [
        "D:\\Projects\\IPTVPlayer\\backend\\IPTV.MCP\\dist\\server.js"
      ]
    }
  }
}
```

3. Restart Claude Desktop

### Step 6: Test in Claude

1. Open Claude Desktop
2. Look for the 🔨 hammer icon in the input area
3. You should see "iptv" in the list of servers
4. Try using the "hello" tool - it should respond with "Hello from IPTV MCP Server! 🎬"

## Troubleshooting

### "Cannot find module" error
- Make sure you ran `npm install`
- Check that `node_modules/` folder exists

### "tsc not found" error
- Run `npm install -D typescript`
- Or use `npx tsc` instead of `tsc`

### Server not showing in Claude
- Check the path in `claude_desktop_config.json`
- Make sure to use double backslashes `\\` on Windows
- Restart Claude Desktop after config changes

### Server crashes on start
- Check the console for error messages
- Make sure `dist/server.js` exists
- Try rebuilding: `npm run build`

## What's Next?

Once your MCP server is working, you'll:

1. **Add real tools** - Replace the "hello" tool with actual content management tools
2. **Connect to backend API** - Add HTTP client to call your .NET API
3. **Implement schemas** - Add Zod schemas for input validation
4. **Add error handling** - Proper error messages and logging

Continue with **Phase 2** in the DEVELOPMENT_ROADMAP.md!

## File Structure

Your backend folder should look like this:

```
backend/
└── IPTV.MCP/
    ├── server.ts          # Main MCP server code
    ├── package.json       # Dependencies
    ├── tsconfig.json      # TypeScript config
    ├── node_modules/      # Installed packages (after npm install)
    └── dist/              # Compiled JavaScript (after npm run build)
        └── server.js
```

## Development Commands

- `npm run build` - Compile TypeScript to JavaScript
- `npm run dev` - Watch mode (recompiles on changes)
- `npm start` - Run the compiled server

## Need Help?

If you encounter issues:
1. Check the troubleshooting section above
2. Verify your Node.js version: `node --version` (should be 18+)
3. Check npm version: `npm --version`
4. Review the error messages carefully

Happy coding! 🚀
