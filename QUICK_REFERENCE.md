# IPTV Player - Quick Reference

## 📁 Project Structure Overview

```
D:\Projects\IPTVPlayer\
├── Claude-project/              # This documentation folder
│   ├── README.md               # Complete architecture guide
│   ├── DEVELOPMENT_ROADMAP.md  # Step-by-step development plan
│   ├── SETUP_INSTRUCTIONS.md   # Quick start guide
│   ├── QUICK_REFERENCE.md      # This file
│   └── starter-files/          # Template files to copy
│
├── backend/                     # .NET Backend (to be created)
│   ├── IPTV.API/               # Main API project
│   ├── IPTV.Core/              # Business logic
│   ├── IPTV.Infrastructure/    # Data access
│   └── IPTV.MCP/               # MCP server
│
├── mobile/                      # React Native mobile (to be created)
├── tv/                         # React Native TV (to be created)
├── web/                        # React web (to be created)
└── shared/                     # Shared code (to be created)
```

## 🚀 Getting Started

### Phase 1: MCP Server Setup (Start Here!)

1. **Copy starter files**
   ```bash
   # Copy files from starter-files/backend/IPTV.MCP/
   # to your backend/IPTV.MCP/ folder
   ```

2. **Install dependencies**
   ```bash
   cd backend\IPTV.MCP
   npm install
   ```

3. **Build and test**
   ```bash
   npm run build
   npm start
   ```

4. **Configure Claude Desktop**
   - Edit `%APPDATA%\Claude\claude_desktop_config.json`
   - Add your MCP server config
   - Restart Claude Desktop

## 🔧 Development Commands

### MCP Server
```bash
cd backend\IPTV.MCP
npm run build          # Compile TypeScript
npm run dev            # Watch mode
npm start              # Run server
```

### .NET Backend
```bash
cd backend\IPTV.API
dotnet run             # Run API
dotnet build           # Build project
dotnet test            # Run tests
```

### Database
```bash
dotnet ef migrations add MigrationName
dotnet ef database update
dotnet ef database drop     # Careful!
```

## 💡 Tips

1. **Work sequentially** - Complete Phase 1 before moving to Phase 2
2. **Test as you go** - Verify each component works before proceeding
3. **Commit often** - Use Git after each completed step
4. **Read the docs** - Check DEVELOPMENT_ROADMAP.md for details
5. **Ask for help** - Use Claude to clarify any confusion!

---

*Keep this file handy as a quick reference during development!*
