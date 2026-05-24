using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace PolicyGuard.Api.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ComplianceChecklists",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Category = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ComplianceChecklists", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ChecklistItems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ComplianceChecklistId = table.Column<int>(type: "int", nullable: false),
                    Requirement = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Keywords = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Weight = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChecklistItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ChecklistItems_ComplianceChecklists_ComplianceChecklistId",
                        column: x => x.ComplianceChecklistId,
                        principalTable: "ComplianceChecklists",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PolicyReviews",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DocumentText = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ComplianceChecklistId = table.Column<int>(type: "int", nullable: false),
                    OverallScore = table.Column<double>(type: "float", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PolicyReviews", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PolicyReviews_ComplianceChecklists_ComplianceChecklistId",
                        column: x => x.ComplianceChecklistId,
                        principalTable: "ComplianceChecklists",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ReviewResults",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PolicyReviewId = table.Column<int>(type: "int", nullable: false),
                    ChecklistItemId = table.Column<int>(type: "int", nullable: false),
                    ResultStatus = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MatchedText = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Recommendation = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Score = table.Column<double>(type: "float", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReviewResults", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ReviewResults_ChecklistItems_ChecklistItemId",
                        column: x => x.ChecklistItemId,
                        principalTable: "ChecklistItems",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ReviewResults_PolicyReviews_PolicyReviewId",
                        column: x => x.PolicyReviewId,
                        principalTable: "PolicyReviews",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "ComplianceChecklists",
                columns: new[] { "Id", "Category", "CreatedAt", "Description", "Name" },
                values: new object[,]
                {
                    { 1, "Information Technology", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Reviews IT policies for required governance, security, access control, and maintenance sections.", "IT Policy Checklist" },
                    { 2, "Software Development", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Reviews software project documentation for SDLC readiness, testing, deployment, and approval coverage.", "SDLC Documentation Checklist" },
                    { 3, "Change Management", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Reviews change management documentation for risk, impact, approval, rollback, and post-change review items.", "Change Management Checklist" }
                });

            migrationBuilder.InsertData(
                table: "ChecklistItems",
                columns: new[] { "Id", "ComplianceChecklistId", "CreatedAt", "Description", "Keywords", "Requirement", "Weight" },
                values: new object[,]
                {
                    { 1, 1, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "The policy should clearly explain its purpose, objective, or intent.", "purpose, objective, intent, goal", "Purpose Statement", 10 },
                    { 2, 1, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "The policy should define who and what the policy applies to.", "scope, applies to, applicable, coverage", "Scope", 10 },
                    { 3, 1, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "The policy should identify responsible parties and their duties.", "roles, responsibilities, accountable, owner, duties", "Roles and Responsibilities", 10 },
                    { 4, 1, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "The policy should address security expectations or safeguards.", "security, safeguard, protection, vulnerability, risk", "Security Considerations", 10 },
                    { 5, 1, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "The policy should explain access control, permissions, or authorization requirements.", "access control, permission, authorization, authentication, least privilege", "Access Control", 10 },
                    { 6, 1, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "The policy should describe how sensitive, confidential, or operational data is handled.", "data handling, confidential, sensitive data, privacy, retention", "Data Handling", 10 },
                    { 7, 1, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "The policy should state when it will be reviewed or updated.", "review schedule, annual review, reviewed, updated, maintenance", "Review Schedule", 10 },
                    { 8, 1, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "The policy should identify approval requirements or approving authority.", "approval, approved by, signoff, authorized by, approver", "Approval Process", 10 },
                    { 9, 1, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "The policy should explain how compliance will be enforced.", "enforcement, violation, noncompliance, disciplinary, corrective action", "Enforcement Statement", 10 },
                    { 10, 1, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "The policy should include revision history or version tracking.", "revision history, version, change log, last updated, document history", "Revision History", 10 },
                    { 11, 2, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "The documentation should explain the project purpose and business need.", "project purpose, business need, objective, problem statement", "Project Purpose", 10 },
                    { 12, 2, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "The documentation should define business requirements.", "business requirement, business requirements, stakeholder need, requirement", "Business Requirements", 10 },
                    { 13, 2, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "The documentation should define functional requirements.", "functional requirement, user story, system shall, feature", "Functional Requirements", 10 },
                    { 14, 2, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "The documentation should define technical requirements or constraints.", "technical requirement, architecture, system design, database, API", "Technical Requirements", 10 },
                    { 15, 2, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "The documentation should include a testing plan.", "testing plan, test case, quality assurance, QA, validation", "Testing Plan", 10 },
                    { 16, 2, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "The documentation should describe User Acceptance Testing.", "UAT, user acceptance testing, acceptance criteria, business validation", "UAT Process", 10 },
                    { 17, 2, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "The documentation should include deployment planning.", "deployment plan, release plan, implementation plan, go live", "Deployment Plan", 10 },
                    { 18, 2, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "The documentation should describe ongoing support or maintenance.", "maintenance plan, support plan, operations, monitoring", "Maintenance Plan", 10 },
                    { 19, 2, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "The documentation should identify risks and mitigation plans.", "risk assessment, risk, mitigation, impact, likelihood", "Risk Assessment", 10 },
                    { 20, 2, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "The documentation should include approval or signoff expectations.", "approval, signoff, approved by, stakeholder approval", "Approval and Signoff", 10 },
                    { 21, 3, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "The change request should clearly describe the proposed change.", "change description, proposed change, request summary, modification", "Change Description", 10 },
                    { 22, 3, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "The change request should explain why the change is needed.", "business justification, justification, business need, reason", "Business Justification", 10 },
                    { 23, 3, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "The change request should explain expected impact.", "impact analysis, impact, affected users, affected systems", "Impact Analysis", 10 },
                    { 24, 3, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "The change request should define risk level or risk rating.", "risk level, risk rating, high risk, medium risk, low risk", "Risk Level", 10 },
                    { 25, 3, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "The change request should include a rollback or backout plan.", "rollback plan, backout plan, restore, revert", "Rollback Plan", 10 },
                    { 26, 3, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "The change request should include testing before or after implementation.", "testing plan, test, validation, verification", "Testing Plan", 10 },
                    { 27, 3, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "The change request should define when the change will be implemented.", "implementation schedule, implementation date, timeline, maintenance window", "Implementation Schedule", 10 },
                    { 28, 3, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "The change request should define approval requirements.", "approval, approved by, change advisory board, CAB, signoff", "Approval Process", 10 },
                    { 29, 3, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "The change request should explain who will be notified and how.", "communication plan, notify, notification, stakeholders, users", "Communication Plan", 10 },
                    { 30, 3, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "The change request should include a post-change review or validation step.", "post-change review, review, lessons learned, validation, follow-up", "Post-Change Review", 10 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_ChecklistItems_ComplianceChecklistId",
                table: "ChecklistItems",
                column: "ComplianceChecklistId");

            migrationBuilder.CreateIndex(
                name: "IX_PolicyReviews_ComplianceChecklistId",
                table: "PolicyReviews",
                column: "ComplianceChecklistId");

            migrationBuilder.CreateIndex(
                name: "IX_ReviewResults_ChecklistItemId",
                table: "ReviewResults",
                column: "ChecklistItemId");

            migrationBuilder.CreateIndex(
                name: "IX_ReviewResults_PolicyReviewId",
                table: "ReviewResults",
                column: "PolicyReviewId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ReviewResults");

            migrationBuilder.DropTable(
                name: "ChecklistItems");

            migrationBuilder.DropTable(
                name: "PolicyReviews");

            migrationBuilder.DropTable(
                name: "ComplianceChecklists");
        }
    }
}
