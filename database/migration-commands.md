# EF Core Migration Commands

Run these from the `backend/PaymentsMatching.API` folder.

## Add initial migration

```bash
dotnet ef migrations add InitialCreate
```

## Apply migration to database

```bash
dotnet ef database update
```

## Drop and recreate database (dev only)

```bash
dotnet ef database drop --force
dotnet ef database update
```

## Alternative: Use the SQL script directly

If you prefer not to use EF migrations, run `create-table.sql` against your
SQL Server database using SSMS or Azure Data Studio.
