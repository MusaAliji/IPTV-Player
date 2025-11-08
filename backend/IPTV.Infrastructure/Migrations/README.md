# Database Migrations

This folder contains Entity Framework Core migrations and SQL scripts for the IPTV database.

## Option 1: Using Entity Framework Migrations (Recommended)

This is the preferred method as it keeps your database schema in sync with your code.

### Apply the migration:

```bash
cd /path/to/backend/IPTV.API
dotnet ef database update
```

This will:
- Create the `IPTVDb` database if it doesn't exist
- Create all tables: Channels, Contents, Users, EPGPrograms
- Set up relationships and indexes

### Verify the migration:

```bash
# List all migrations
dotnet ef migrations list

# View migration details
dotnet ef migrations script
```

### Rollback (if needed):

```bash
# Remove the last migration
dotnet ef database update 0

# Or remove the migration files and update
dotnet ef migrations remove
```

## Option 2: Using SQL Script (Manual)

If you prefer to create the database manually or can't use EF migrations:

1. **Open SQL Server Management Studio (SSMS)** or **Azure Data Studio**

2. **Connect to your SQL Server instance**

3. **Run the SQL script:**
   - Open file: `InitialCreate.sql`
   - Execute the script
   - This will create the database and all tables

## Database Schema

### Tables Created:

1. **Channels**
   - Live TV channel information
   - Fields: Name, StreamUrl, ChannelNumber, Category, Language, etc.

2. **Contents**
   - VOD content (movies, series)
   - Fields: Title, Description, StreamUrl, Type, Duration, Genre, Rating, etc.

3. **Users**
   - User accounts and authentication
   - Fields: Username, Email, PasswordHash, Role, etc.
   - Unique constraints on Username and Email

4. **EPGPrograms**
   - Electronic Program Guide data
   - Fields: Title, Description, StartTime, EndTime, ChannelId, etc.
   - Foreign key to Channels table

### Relationships:

- EPGPrograms → Channels (Many-to-One with CASCADE delete)

## Connection String

Default connection string in `appsettings.json`:
```
Server=localhost;Database=IPTVDb;Trusted_Connection=True;TrustServerCertificate=True
```

### For SQL Server LocalDB:
```
Server=(localdb)\\mssqllocaldb;Database=IPTVDb;Trusted_Connection=True
```

### For SQL Server with credentials:
```
Server=localhost;Database=IPTVDb;User Id=your_user;Password=your_password;TrustServerCertificate=True
```

## Troubleshooting

### "Unable to create migrations"
Make sure you're in the IPTV.API directory:
```bash
cd IPTV.API
dotnet ef migrations add MigrationName --project ../IPTV.Infrastructure
```

### "Login failed for user"
- Check your connection string
- Verify SQL Server is running
- For Windows Authentication, make sure Integrated Security is enabled

### "Database already exists"
The migration is safe to run multiple times. It will skip existing tables.

## Next Steps

After creating the database:
1. Run the API: `dotnet run --project IPTV.API`
2. Test endpoints: https://localhost:5001/swagger
3. Add seed data (Phase 2)
