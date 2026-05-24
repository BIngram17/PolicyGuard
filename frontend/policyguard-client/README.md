# PolicyGuard Compliance Analyzer

PolicyGuard is a full-stack compliance review application built with ASP.NET Core, SQL Server, Entity Framework Core, JWT authentication, role-based access control, and React. The application allows users to create compliance checklist templates, analyze policy/procedure text against those checklists, review compliance results, generate exportable reports, and track system activity through an audit trail.

This project was built as a portfolio application to demonstrate full-stack software development, secure API design, CRUD operations, authentication, audit logging, SQL Server integration, and a polished React user interface.

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
- Swagger API documentation
- SQL Server persistence with Entity Framework Core

---

## Tech Stack

### Backend

- C#
- ASP.NET Core Web API
- Entity Framework Core
- SQL Server Express
- JWT Authentication
- Swagger / OpenAPI

### Frontend

- React
- Vite
- JavaScript
- Axios
- CSS

### Tools

- Visual Studio Code
- SQL Server Express
- Swagger UI
- Git / GitHub

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

The API is documented through Swagger.

![Swagger API](screenshots/swagger-api.png)

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

## Local Development Setup

### Prerequisites

Install the following before running the project:

- .NET 9 SDK
- Node.js
- SQL Server Express
- Visual Studio Code or Visual Studio

---

## Backend Setup

Navigate to the backend project:

```bash
cd backend/PolicyGuard.Api