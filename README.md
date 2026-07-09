# PolicyGuard Compliance Analyzer

PolicyGuard is a full-stack compliance review application built with ASP.NET Core, SQL Server, Entity Framework Core, JWT authentication, role-based access control, and React. The application allows users to create compliance checklist templates, analyze policy/procedure text against those checklists, review compliance results, generate exportable reports, and track system activity through an audit trail.

This project demonstrates full-stack software development, secure API design, CRUD operations, authentication, audit logging, SQL Server integration, automated unit testing, Docker readiness, and GitHub Actions CI/CD.

---

## Enterprise Upgrade Highlights

- xUnit test project for backend business logic and security-sensitive password behavior
- GitHub Actions CI pipeline for backend restore/build/test, frontend build, and Docker image validation
- Azure-ready deployment workflow for the API and frontend
- Environment-driven production configuration
- Production-safe JWT, CORS, and connection string handling
- API `/health` endpoint for cloud health checks
- Dockerfile for containerized API deployment
- Deployment guide in `docs/DEPLOYMENT.md`

---

## Features

- JWT-based authentication
- Role-based access control
- Admin, Reviewer, and Auditor user roles
- Compliance checklist creation, viewing, editing, and deletion
- Policy/procedure analysis against checklist requirements
- Saved policy review archive
- Search, filter, and sort controls for saved reviews
- Detailed review results with compliance scoring
- Exportable compliance report view
- Downloadable text report
- Audit trail for login, review, checklist, delete, and export events
- Audit search, filters, sorting, and refresh tracking
- Swagger API documentation in development
- SQL Server persistence with Entity Framework Core

---

## Tech Stack

### Backend

- C#
- ASP.NET Core Web API
- Entity Framework Core
- SQL Server / Azure SQL
- JWT Authentication
- xUnit
- Swagger / OpenAPI
- Docker

### Frontend

- React
- Vite
- JavaScript
- Axios
- CSS

### DevOps

- GitHub Actions
- Azure App Service
- Azure Static Web Apps
- Docker

---

## Screenshots

### Dashboard

![Dashboard](screenshots/dashboard.png)

### New Policy Review

![New Policy Review](screenshots/new-review.png)

### Saved Reviews

![Saved Reviews](screenshots/saved-reviews.png)

### Review Results

![Review Results](screenshots/review-results.png)

### Compliance Report View

![Compliance Report View](screenshots/report-view.png)

### Checklist Manager

![Checklist Manager](screenshots/checklist-manager.png)

### Checklist Template Detail

![Checklist Template Detail](screenshots/checklist-detail.png)

### Edit Checklist Template

![Edit Checklist Template](screenshots/edit-checklist.png)

### Audit Trail

![Audit Trail](screenshots/audit-trail.png)

### Swagger API

![Swagger API](screenshots/swagger-api.png)

---

## Application Roles

### Admin

Admins can manage the full application, including:

- Creating policy reviews
- Viewing saved reviews
- Deleting reviews
- Creating checklist templates
- Editing checklist templates
- Deleting unused checklist templates
- Viewing audit logs
- Exporting reports

### Reviewer

Reviewers can:

- Create policy reviews
- View saved reviews
- View checklist templates
- View review results
- Generate reports

Reviewers cannot manage checklist templates or delete reviews.

### Auditor

Auditors can:

- View saved reviews
- View checklist templates
- View audit logs
- Review compliance activity

Auditors cannot create policy reviews or modify checklist templates.

---

## API Overview

PolicyGuard exposes REST API endpoints for authentication, checklist management, policy review analysis, dashboard summaries, and audit logging.

Main API areas:

- `/api/Auth`
- `/api/Checklists`
- `/api/PolicyReviews`
- `/api/Dashboard`
- `/api/AuditLogs`
- `/health`

The API is documented through Swagger in development.

---

## Local Development Setup

### Prerequisites

Install the following before running the project:

- .NET 9 SDK
- Node.js 22+
- SQL Server Express
- Docker Desktop, optional but recommended
- Visual Studio Code or Visual Studio

### Backend Setup

Navigate to the backend project:

```bash
cd backend/PolicyGuard.Api
```

Restore dependencies:

```bash
dotnet restore
```

Apply EF Core migrations:

```bash
dotnet ef database update
```

Run the API:

```bash
dotnet run
```

The API runs locally using `appsettings.Development.json`.

### Frontend Setup

Navigate to the frontend project:

```bash
cd frontend/policyguard-client
```

Install dependencies:

```bash
npm install
```

Run the React app:

```bash
npm run dev
```

By default, the frontend calls:

```text
http://localhost:5069/api
```

For deployed environments, set:

```text
VITE_API_BASE_URL=https://your-policyguard-api.azurewebsites.net/api
```

---

## Running Tests

Run backend xUnit tests:

```bash
dotnet test backend/PolicyGuard.Api.Tests/PolicyGuard.Api.Tests.csproj
```

The test suite currently covers:

- Policy analyzer pass/needs-review/missing logic
- Keyword trimming and case-insensitive matching
- Weighted compliance score calculation
- Score rounding
- Zero-weight checklist behavior
- Password hashing format
- Password verification success/failure paths
- Invalid stored password hash handling
- Unique password salts

---

## CI/CD

This repository includes a GitHub Actions workflow at:

```text
.github/workflows/ci-cd.yml
```

The pipeline runs on pull requests and pushes to `main`.

It performs:

- Backend restore
- Backend release build
- xUnit test execution
- Test result artifact upload
- API Docker image build validation
- Frontend dependency install
- Frontend production build
- Azure API deployment when Azure secrets are configured
- Azure Static Web Apps frontend deployment when Azure secrets are configured

See `docs/DEPLOYMENT.md` for cloud setup instructions.

---

## Docker

Build the API image from the repository root:

```bash
docker build -f backend/PolicyGuard.Api/Dockerfile -t policyguard-api:local .
```

Run the API container:

```bash
docker run --rm -p 8080:8080 \
  -e ASPNETCORE_ENVIRONMENT=Production \
  -e Jwt__Key="replace-with-a-long-local-test-secret" \
  -e ConnectionStrings__DefaultConnection="<your-connection-string>" \
  -e Cors__AllowedOrigins__0="http://localhost:5173" \
  policyguard-api:local
```

Check the health endpoint:

```bash
curl http://localhost:8080/health
```

---

## Deployment

The recommended cloud deployment is:

- API: Azure App Service
- Frontend: Azure Static Web Apps
- Database: Azure SQL Database
- Automation: GitHub Actions

Required GitHub repository variables:

```text
AZURE_WEBAPP_NAME
VITE_API_BASE_URL
```

Required GitHub repository secrets:

```text
AZURE_WEBAPP_PUBLISH_PROFILE
AZURE_STATIC_WEB_APPS_API_TOKEN
```

Required Azure App Service settings:

```text
ASPNETCORE_ENVIRONMENT=Production
Jwt__Key=<production-secret>
Jwt__Issuer=PolicyGuard
Jwt__Audience=PolicyGuardClient
Jwt__ExpirationMinutes=480
Cors__AllowedOrigins__0=https://<frontend-url>
Swagger__Enabled=false
```

See `docs/DEPLOYMENT.md` for the full deployment checklist.

---

## Example Workflow

1. Admin logs in.
2. Admin creates or edits a compliance checklist template.
3. Reviewer creates a new policy review using one of the checklist templates.
4. PolicyGuard analyzes the document text against checklist requirements.
5. The user reviews the compliance score and detailed findings.
6. The user opens the report view and exports the report.
7. The audit trail records the login, review creation, report export, and other important events.

---

## Resume-Ready Talking Points

- Built a full-stack compliance analyzer with ASP.NET Core, React, SQL Server, JWT authentication, RBAC, and audit logging.
- Added xUnit unit tests for backend business logic, weighted scoring behavior, password hashing, password verification, and invalid hash handling.
- Built a GitHub Actions CI/CD pipeline that runs backend tests, validates frontend production builds, builds a Docker image, and deploys to Azure when cloud secrets are configured.
- Refactored production configuration to use environment variables for JWT secrets, SQL connection strings, CORS origins, and frontend API endpoints.
