# IPTV Backend - Phase 1 Setup

This directory contains the complete backend implementation for the IPTV Player platform, including:
- MCP Server (Node.js/TypeScript)
- .NET Web API (ASP.NET Core 8.0)
- Entity Framework Core with SQL Server

## Prerequisites

- Node.js 18+ (for MCP server)
- .NET 8.0 SDK
- SQL Server (LocalDB or SQL Server Express)

## Project Structure

```
backend/
├── IPTV.MCP/              # MCP Server for Claude integration
├── IPTV.API/              # ASP.NET Core Web API
├── IPTV.Core/             # Domain entities and interfaces
├── IPTV.Infrastructure/   # Data access and EF Core
└── IPTV.sln               # Visual Studio solution
```

## Setup Instructions

### 1. MCP Server Setup

The MCP server is already built and ready to use.

```bash
cd IPTV.MCP
npm install   # Already done, but run if needed
npm run build # Already done, but run if needed
npm start     # Test the server
```

### 2. .NET Backend Setup

#### Step 1: Restore NuGet packages

```bash
cd /path/to/backend
dotnet restore
```

#### Step 2: Verify the solution builds

```bash
dotnet build
```

#### Step 3: Create database migration

```bash
cd IPTV.API
dotnet ef migrations add InitialCreate --project ../IPTV.Infrastructure --startup-project .
```

#### Step 4: Apply migration to create database

```bash
dotnet ef database update --project ../IPTV.Infrastructure --startup-project .
```

This will create a SQL Server database named `IPTVDb` with all tables.

#### Step 5: Run the API

```bash
dotnet run --project IPTV.API
```

The API will start at:
- HTTPS: https://localhost:5001
- HTTP: http://localhost:5000
- Swagger UI: https://localhost:5001/swagger

### 3. Test the API

Once running, test the catalog endpoint:

```bash
# Using curl
curl https://localhost:5001/api/content

# Using PowerShell
Invoke-WebRequest -Uri "https://localhost:5001/api/content" -SkipCertificateCheck
```

Or open Swagger UI in your browser: https://localhost:5001/swagger

## Database Configuration

The default connection string in `appsettings.json`:
```
Server=localhost;Database=IPTVDb;Trusted_Connection=True;TrustServerCertificate=True
```

To use a different SQL Server instance, update the connection string in:
- `IPTV.API/appsettings.json`

## Available Entities

### Content
- Represents movies, series, and VOD content
- Fields: Title, Description, StreamUrl, Type, Duration, Rating, etc.

### Channel
- Represents live TV channels
- Fields: Name, StreamUrl, ChannelNumber, Category, Language, etc.

### EPGProgram
- Electronic Program Guide data
- Fields: Title, Description, StartTime, EndTime, ChannelId, etc.

### User
- User accounts
- Fields: Username, Email, PasswordHash, Role, etc.

## API Endpoints

### Content Controller
- `GET /api/content` - Get all content (catalog)
- `GET /api/content/{id}` - Get content by ID
- `GET /api/content/type/{type}` - Get content by type (LiveTV, VOD, Series, Movie)

## Troubleshooting

### "Could not find a part of the path" error
Make sure you're in the correct directory when running commands.

### Database connection errors
- Verify SQL Server is running
- Check the connection string in appsettings.json
- For LocalDB, use: `Server=(localdb)\\mssqllocaldb;Database=IPTVDb;Trusted_Connection=True`

### Migration errors
Make sure you're running migrations from the IPTV.API directory:
```bash
cd IPTV.API
dotnet ef migrations add InitialCreate --project ../IPTV.Infrastructure
```

## Next Steps

After completing Phase 1:
1. Add seed data to the database
2. Implement additional controllers (Channel, EPG, User)
3. Add authentication and authorization
4. Continue with Phase 2 from DEVELOPMENT_ROADMAP.md

## Phase 1 Checklist

- [x] MCP server created and tested
- [x] .NET solution structure created
- [x] Entity Framework Core configured
- [x] Core entities defined
- [x] Database context created
- [x] ContentController with GET endpoints
- [x] CORS and Swagger configured
- [ ] Database migration created (run locally)
- [ ] Database created (run locally)
- [ ] API tested with real data

---

**Note:** All code files have been created. You just need to run the .NET commands on your local machine to restore packages, create migrations, and run the API.
