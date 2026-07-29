# PolicyGuard Deployment Guide

This guide documents the production deployment path for PolicyGuard using Azure App Service, Azure SQL Database, Azure Static Web Apps, and GitHub Actions.

## Target Architecture

- **Frontend:** React/Vite app deployed to Azure Static Web Apps.
- **Backend:** ASP.NET Core Web API deployed to Azure App Service.
- **Database:** Azure SQL Database.
- **Automation:** GitHub Actions runs build/test checks on pull requests and deploys after successful pushes to `main`.

## Required Azure Resources

Create these resources in Azure:

1. Azure SQL Database
2. Azure App Service for the ASP.NET Core API
3. Azure Static Web Apps resource for the React frontend

Use a consistent naming pattern so the project looks professional:

```text
policyguard-api
policyguard-db
policyguard-web
policyguard-rg
```

## Backend App Settings

In the Azure App Service configuration blade, add these app settings:

```text
ASPNETCORE_ENVIRONMENT=Production
Jwt__Key=<long-random-production-secret>
Jwt__Issuer=PolicyGuard
Jwt__Audience=PolicyGuardClient
Jwt__ExpirationMinutes=480
Cors__AllowedOrigins__0=https://<your-static-web-app-url>
Swagger__Enabled=false
PortfolioDemo__Enabled=true
PortfolioDemo__Email=portfolio-reviewer@your-domain.example
PortfolioDemo__FullName=Portfolio Reviewer
PortfolioDemo__Password=<unique-random-password-at-least-12-characters>
PortfolioDemo__AccessToken=<unique-random-url-safe-token>
```

Add the SQL connection string under **Connection strings** using the name `DefaultConnection`:

```text
Server=tcp:<your-sql-server>.database.windows.net,1433;Initial Catalog=<your-db>;Persist Security Info=False;User ID=<db-user>;Password=<db-password>;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;
```

Never commit production database credentials or JWT secrets to GitHub.

The API creates or updates the portfolio demo account during startup. Its role is
always forced to `Reviewer`, which allows recruiters to exercise the core workflow
without user administration or destructive access. Changing
`PortfolioDemo__Password` and restarting the App Service rotates the password.
`PortfolioDemo__AccessToken` enables a resume link that exchanges an opaque token
for a Reviewer session without exposing the account password.

## Frontend Environment Variable

Set this GitHub repository variable:

```text
VITE_API_BASE_URL=https://<your-api-app-name>.azurewebsites.net/api
VITE_DEMO_EMAIL=portfolio-reviewer@your-domain.example
```

The frontend reads this value at build time through Vite.
`VITE_DEMO_EMAIL` is safe to display and enables the recruiter access card on the
login screen. Never create a `VITE_` variable for the demo password because Vite
embeds those values in the public browser bundle.

## GitHub Repository Variables

Go to:

```text
GitHub repo → Settings → Secrets and variables → Actions → Variables
```

Add:

```text
AZURE_WEBAPP_NAME=<your-api-app-service-name>
VITE_API_BASE_URL=https://<your-api-app-service-name>.azurewebsites.net/api
VITE_DEMO_EMAIL=portfolio-reviewer@your-domain.example
```

## GitHub Repository Secrets

Go to:

```text
GitHub repo → Settings → Secrets and variables → Actions → Secrets
```

Add:

```text
AZURE_WEBAPP_PUBLISH_PROFILE=<publish-profile-xml-from-azure-app-service>
AZURE_STATIC_WEB_APPS_API_TOKEN=<deployment-token-from-azure-static-web-apps>
```

The CI/CD workflow is written to skip deployment when these values are missing. That means pull requests and local setup can still run build/test checks without cloud credentials.

## Database Migration

Run the initial EF Core migration against Azure SQL before treating the deployment as production-ready.

From the backend project directory:

```bash
cd backend/PolicyGuard.Api

dotnet ef database update
```

Make sure your local environment is temporarily pointed at the Azure SQL connection string before running the command, or run the migration from a controlled deployment environment.

## Health Check

After the API is deployed, verify the app is alive:

```bash
curl https://<your-api-app-name>.azurewebsites.net/health
```

Expected response:

```json
{
  "service": "PolicyGuard.Api",
  "status": "Healthy",
  "environment": "Production",
  "timestampUtc": "..."
}
```

## Deployment Flow

1. Open a pull request into `main`.
2. GitHub Actions runs backend build, xUnit tests, frontend build, and Docker image validation.
3. Merge the pull request.
4. GitHub Actions deploys the API and frontend from `main` if Azure secrets and variables are configured.
5. Verify `/health` and open the frontend URL.

## Sharing Portfolio Access

Send the live URL and demo email in your application or portfolio. Send the
password separately through a time-limited 1Password, Bitwarden, or Proton Pass
secret link. If that is not practical, send the password in a separate direct
message rather than in the same email as the URL.

Use one shared Reviewer account for short recruiting campaigns, rotate its
password between campaigns, and never share the Admin account. To revoke access
immediately, set `PortfolioDemo__Enabled=false` and deactivate the account in the
database, or rotate `PortfolioDemo__Password` and restart the API.

For a one-click resume experience, use this link format:

```text
https://<your-static-web-app-url>/#demo=<PortfolioDemo__AccessToken>
```

The browser removes the fragment from the address bar before submitting it to
`/api/Auth/demo-login`. The regular login page remains visible with prepared,
read-only fields, and the visitor presses **Sign In to Live Demo**. Treat the
complete link as a credential: share it only where intended and rotate
`PortfolioDemo__AccessToken` to revoke old resume links.

## Docker Build

The API can also be built as a container from the repository root:

```bash
docker build -f backend/PolicyGuard.Api/Dockerfile -t policyguard-api:local .
```

Run it locally with production-like environment variables:

```bash
docker run --rm -p 8080:8080 \
  -e ASPNETCORE_ENVIRONMENT=Production \
  -e Jwt__Key="replace-with-a-long-local-test-secret" \
  -e ConnectionStrings__DefaultConnection="<your-connection-string>" \
  -e Cors__AllowedOrigins__0="http://localhost:5173" \
  policyguard-api:local
```

Then check:

```bash
curl http://localhost:8080/health
```
