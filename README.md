# Payments Matching Tool

Full-stack payment reconciliation app for comparing internal system payments with payment provider payments from two CSV files.

The app lets a user upload both files, run the matching process, review matched and unmatched records, and resolve differences by accepting either the system or provider side.

## Tech Stack

| Layer | Technology |
| --- | --- |
| Frontend | Angular 17, Angular Material |
| Backend | .NET 8 Web API, Entity Framework Core 8 |
| Database | SQL Server LocalDB, Express, or full SQL Server |
| API Docs | Swagger / OpenAPI |

## Project Structure

```text
PaymentsMatchingTool/
|-- backend/
|   `-- PaymentsMatching.API/
|       |-- Controllers/
|       |-- Data/
|       |-- DTOs/
|       |-- Entities/
|       |-- Middleware/
|       |-- Models/
|       |-- Repositories/
|       |-- Services/
|       |-- Program.cs
|       `-- appsettings.json
|-- frontend/
|   `-- payments-matching-app/
|       |-- src/
|       |-- angular.json
|       `-- package.json
|-- database/
|   |-- create-table.sql
|   `-- migration-commands.md
`-- samples/
    |-- system.csv
    `-- provider.csv
```

## Prerequisites

Install these before running the app:

| Tool | Version |
| --- | --- |
| .NET SDK | 8.0 or newer |
| Node.js | 18 or newer |
| SQL Server | LocalDB, Express, or full SQL Server |
| EF Core CLI | 8.x, only needed for migrations |

Optional EF CLI install:

```bash
dotnet tool install -g dotnet-ef
```

## Database Setup

The default connection string is in `backend/PaymentsMatching.API/appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=PaymentsMatchingDb;Trusted_Connection=True;TrustServerCertificate=True"
  }
}
```

Use one of these setup options.

### Option A: Manual SQL

Open `database/create-table.sql` in SQL Server Management Studio or Azure Data Studio and run it against `PaymentsMatchingDb`.

If the database does not exist yet, uncomment these lines at the top of the script first:

```sql
CREATE DATABASE PaymentsMatchingDb;
GO
USE PaymentsMatchingDb;
GO
```

### Option B: EF Core Migrations

From `backend/PaymentsMatching.API`:

```bash
dotnet ef migrations add InitialCreate
dotnet ef database update
```

## Run The Backend

From `backend/PaymentsMatching.API`:

```bash
dotnet restore
dotnet run
```

The launch profile is configured for:

```text
HTTP:  http://localhost:5000
HTTPS: https://localhost:5001
Swagger: http://localhost:5000/swagger
```

## Run The Frontend

From `frontend/payments-matching-app`:

```bash
npm install
npm start
```

Open:

```text
http://localhost:4200
```

The Angular app calls the API at `http://localhost:5000`.

## Quick Test

1. Start SQL Server.
2. Create the `MatchResults` table using `database/create-table.sql`.
3. Start the backend with `dotnet run`.
4. Start the frontend with `npm start`.
5. Open `http://localhost:4200`.
6. Upload `samples/system.csv` and `samples/provider.csv`.
7. Click `Run Match`.

Expected summary for the sample files:

| Status | Count |
| --- | ---: |
| Total | 5 |
| Matched | 2 |
| Only System | 1 |
| Only Provider | 1 |
| Amount Mismatch | 1 |

## CSV Format

Both files must be UTF-8 CSV files with this header:

```csv
orderId,amount,currency
```

Example:

```csv
orderId,amount,currency
ORD-1,100,INR
ORD-2,200,INR
ORD-3,150,USD
```

## Matching Rules

| Status | Rule |
| --- | --- |
| `MATCHED` | Same `orderId` and `currency` exist in both files with the same amount |
| `AMOUNTMISMATCH` | Same `orderId` and `currency` exist in both files with different amounts |
| `ONLYSYSTEM` | Record exists only in the system file |
| `ONLYPROVIDER` | Record exists only in the provider file |

The matching key is `orderId + currency`.

## API Endpoints

| Method | Endpoint | Description |
| --- | --- | --- |
| `POST` | `/api/matching/run` | Upload CSV files and run matching |
| `GET` | `/api/matching/results?filter=all` | Get match results |
| `GET` | `/api/matching/summary` | Get summary counts |
| `PUT` | `/api/matching/{id}/resolve` | Resolve a non-matched record |

Resolve request body:

```json
{ "resolutionSide": "SYSTEM" }
```

or:

```json
{ "resolutionSide": "PROVIDER" }
```

## Troubleshooting

| Problem | Fix |
| --- | --- |
| Frontend cannot call API | Confirm the backend is running on `http://localhost:5000` |
| Database connection fails | Confirm SQL Server is running and update `appsettings.json` if needed |
| `dotnet ef` is not found | Run `dotnet tool install -g dotnet-ef` |
| Upload returns `400 Bad Request` | Select both CSV files before running the match |
| Empty or failed results load | Create the database table before using the app |

## Verification Performed

These commands were run successfully:

```bash
dotnet build
npm run build
```

The Angular build currently reports warnings for bundle/style budgets and one Angular Material content projection diagnostic, but it still completes successfully.
