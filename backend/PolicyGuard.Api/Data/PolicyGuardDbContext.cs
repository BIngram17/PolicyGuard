using Microsoft.EntityFrameworkCore;
using PolicyGuard.Api.Models;

namespace PolicyGuard.Api.Data;

public class PolicyGuardDbContext : DbContext
{
    public PolicyGuardDbContext(DbContextOptions<PolicyGuardDbContext> options)
        : base(options)
    {
    }

    public DbSet<AppUser> AppUsers => Set<AppUser>();
    public DbSet<ComplianceChecklist> ComplianceChecklists => Set<ComplianceChecklist>();
    public DbSet<ChecklistItem> ChecklistItems => Set<ChecklistItem>();
    public DbSet<PolicyReview> PolicyReviews => Set<PolicyReview>();
    public DbSet<ReviewResult> ReviewResults => Set<ReviewResult>();
    public DbSet<AuditLog> AuditLogs => Set<AuditLog>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        const string adminPasswordHash = "UG9saWN5R3VhcmRBZG1pbg==.O2psDnM1aFO70lk3DjLHlPHjGODbJgm1mBbNnhbuuAI=";
        const string reviewerPasswordHash = "UG9saWN5R3VhcmRSZXZpZQ==.7it8fGTFjEGTtQUBlKXihfEYQDiArcf0z0WvwivmcZI=";
        const string auditorPasswordHash = "UG9saWN5R3VhcmRBdWRpdA==.amhEDPqJtIkpaHIi5g3vdxcwMxWeV3Gqh5f7nRcrO08=";

        modelBuilder.Entity<AppUser>()
            .HasIndex(user => user.Email)
            .IsUnique();

        modelBuilder.Entity<AppUser>()
            .Property(user => user.FullName)
            .HasMaxLength(150);

        modelBuilder.Entity<AppUser>()
            .Property(user => user.Email)
            .HasMaxLength(200);

        modelBuilder.Entity<AppUser>()
            .Property(user => user.Role)
            .HasMaxLength(50);

        modelBuilder.Entity<ComplianceChecklist>()
            .HasMany(c => c.Items)
            .WithOne(i => i.ComplianceChecklist)
            .HasForeignKey(i => i.ComplianceChecklistId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<PolicyReview>()
            .HasMany(r => r.Results)
            .WithOne(result => result.PolicyReview)
            .HasForeignKey(result => result.PolicyReviewId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<ReviewResult>()
            .HasOne(result => result.ChecklistItem)
            .WithMany()
            .HasForeignKey(result => result.ChecklistItemId)
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<AuditLog>()
            .Property(log => log.Action)
            .HasMaxLength(100);

        modelBuilder.Entity<AuditLog>()
            .Property(log => log.EntityType)
            .HasMaxLength(100);

        modelBuilder.Entity<AuditLog>()
            .Property(log => log.PerformedBy)
            .HasMaxLength(150);

        modelBuilder.Entity<AppUser>().HasData(
            new AppUser
            {
                Id = 1,
                FullName = "Admin User",
                Email = "admin@policyguard.local",
                PasswordHash = adminPasswordHash,
                Role = "Admin",
                IsActive = true,
                CreatedAt = new DateTime(2026, 1, 1)
            },
            new AppUser
            {
                Id = 2,
                FullName = "Reviewer User",
                Email = "reviewer@policyguard.local",
                PasswordHash = reviewerPasswordHash,
                Role = "Reviewer",
                IsActive = true,
                CreatedAt = new DateTime(2026, 1, 1)
            },
            new AppUser
            {
                Id = 3,
                FullName = "Auditor User",
                Email = "auditor@policyguard.local",
                PasswordHash = auditorPasswordHash,
                Role = "Auditor",
                IsActive = true,
                CreatedAt = new DateTime(2026, 1, 1)
            }
        );

        modelBuilder.Entity<ComplianceChecklist>().HasData(
            new ComplianceChecklist
            {
                Id = 1,
                Name = "IT Policy Checklist",
                Description = "Reviews IT policies for required governance, security, access control, and maintenance sections.",
                Category = "Information Technology",
                CreatedAt = new DateTime(2026, 1, 1)
            },
            new ComplianceChecklist
            {
                Id = 2,
                Name = "SDLC Documentation Checklist",
                Description = "Reviews software project documentation for SDLC readiness, testing, deployment, and approval coverage.",
                Category = "Software Development",
                CreatedAt = new DateTime(2026, 1, 1)
            },
            new ComplianceChecklist
            {
                Id = 3,
                Name = "Change Management Checklist",
                Description = "Reviews change management documentation for risk, impact, approval, rollback, and post-change review items.",
                Category = "Change Management",
                CreatedAt = new DateTime(2026, 1, 1)
            }
        );

        modelBuilder.Entity<ChecklistItem>().HasData(
            new ChecklistItem { Id = 1, ComplianceChecklistId = 1, Requirement = "Purpose Statement", Description = "The policy should clearly explain its purpose, objective, or intent.", Keywords = "purpose, objective, intent, goal", Weight = 10, CreatedAt = new DateTime(2026, 1, 1) },
            new ChecklistItem { Id = 2, ComplianceChecklistId = 1, Requirement = "Scope", Description = "The policy should define who and what the policy applies to.", Keywords = "scope, applies to, applicable, coverage", Weight = 10, CreatedAt = new DateTime(2026, 1, 1) },
            new ChecklistItem { Id = 3, ComplianceChecklistId = 1, Requirement = "Roles and Responsibilities", Description = "The policy should identify responsible parties and their duties.", Keywords = "roles, responsibilities, accountable, owner, duties", Weight = 10, CreatedAt = new DateTime(2026, 1, 1) },
            new ChecklistItem { Id = 4, ComplianceChecklistId = 1, Requirement = "Security Considerations", Description = "The policy should address security expectations or safeguards.", Keywords = "security, safeguard, protection, vulnerability, risk", Weight = 10, CreatedAt = new DateTime(2026, 1, 1) },
            new ChecklistItem { Id = 5, ComplianceChecklistId = 1, Requirement = "Access Control", Description = "The policy should explain access control, permissions, or authorization requirements.", Keywords = "access control, permission, authorization, authentication, least privilege", Weight = 10, CreatedAt = new DateTime(2026, 1, 1) },
            new ChecklistItem { Id = 6, ComplianceChecklistId = 1, Requirement = "Data Handling", Description = "The policy should describe how sensitive, confidential, or operational data is handled.", Keywords = "data handling, confidential, sensitive data, privacy, retention", Weight = 10, CreatedAt = new DateTime(2026, 1, 1) },
            new ChecklistItem { Id = 7, ComplianceChecklistId = 1, Requirement = "Review Schedule", Description = "The policy should state when it will be reviewed or updated.", Keywords = "review schedule, annual review, reviewed, updated, maintenance", Weight = 10, CreatedAt = new DateTime(2026, 1, 1) },
            new ChecklistItem { Id = 8, ComplianceChecklistId = 1, Requirement = "Approval Process", Description = "The policy should identify approval requirements or approving authority.", Keywords = "approval, approved by, signoff, authorized by, approver", Weight = 10, CreatedAt = new DateTime(2026, 1, 1) },
            new ChecklistItem { Id = 9, ComplianceChecklistId = 1, Requirement = "Enforcement Statement", Description = "The policy should explain how compliance will be enforced.", Keywords = "enforcement, violation, noncompliance, disciplinary, corrective action", Weight = 10, CreatedAt = new DateTime(2026, 1, 1) },
            new ChecklistItem { Id = 10, ComplianceChecklistId = 1, Requirement = "Revision History", Description = "The policy should include revision history or version tracking.", Keywords = "revision history, version, change log, last updated, document history", Weight = 10, CreatedAt = new DateTime(2026, 1, 1) },

            new ChecklistItem { Id = 11, ComplianceChecklistId = 2, Requirement = "Project Purpose", Description = "The documentation should explain the project purpose and business need.", Keywords = "project purpose, business need, objective, problem statement", Weight = 10, CreatedAt = new DateTime(2026, 1, 1) },
            new ChecklistItem { Id = 12, ComplianceChecklistId = 2, Requirement = "Business Requirements", Description = "The documentation should define business requirements.", Keywords = "business requirement, business requirements, stakeholder need, requirement", Weight = 10, CreatedAt = new DateTime(2026, 1, 1) },
            new ChecklistItem { Id = 13, ComplianceChecklistId = 2, Requirement = "Functional Requirements", Description = "The documentation should define functional requirements.", Keywords = "functional requirement, user story, system shall, feature", Weight = 10, CreatedAt = new DateTime(2026, 1, 1) },
            new ChecklistItem { Id = 14, ComplianceChecklistId = 2, Requirement = "Technical Requirements", Description = "The documentation should define technical requirements or constraints.", Keywords = "technical requirement, architecture, system design, database, API", Weight = 10, CreatedAt = new DateTime(2026, 1, 1) },
            new ChecklistItem { Id = 15, ComplianceChecklistId = 2, Requirement = "Testing Plan", Description = "The documentation should include a testing plan.", Keywords = "testing plan, test case, quality assurance, QA, validation", Weight = 10, CreatedAt = new DateTime(2026, 1, 1) },
            new ChecklistItem { Id = 16, ComplianceChecklistId = 2, Requirement = "UAT Process", Description = "The documentation should describe User Acceptance Testing.", Keywords = "UAT, user acceptance testing, acceptance criteria, business validation", Weight = 10, CreatedAt = new DateTime(2026, 1, 1) },
            new ChecklistItem { Id = 17, ComplianceChecklistId = 2, Requirement = "Deployment Plan", Description = "The documentation should include deployment planning.", Keywords = "deployment plan, release plan, implementation plan, go live", Weight = 10, CreatedAt = new DateTime(2026, 1, 1) },
            new ChecklistItem { Id = 18, ComplianceChecklistId = 2, Requirement = "Maintenance Plan", Description = "The documentation should describe ongoing support or maintenance.", Keywords = "maintenance plan, support plan, operations, monitoring", Weight = 10, CreatedAt = new DateTime(2026, 1, 1) },
            new ChecklistItem { Id = 19, ComplianceChecklistId = 2, Requirement = "Risk Assessment", Description = "The documentation should identify risks and mitigation plans.", Keywords = "risk assessment, risk, mitigation, impact, likelihood", Weight = 10, CreatedAt = new DateTime(2026, 1, 1) },
            new ChecklistItem { Id = 20, ComplianceChecklistId = 2, Requirement = "Approval and Signoff", Description = "The documentation should include approval or signoff expectations.", Keywords = "approval, signoff, approved by, stakeholder approval", Weight = 10, CreatedAt = new DateTime(2026, 1, 1) },

            new ChecklistItem { Id = 21, ComplianceChecklistId = 3, Requirement = "Change Description", Description = "The change request should clearly describe the proposed change.", Keywords = "change description, proposed change, request summary, modification", Weight = 10, CreatedAt = new DateTime(2026, 1, 1) },
            new ChecklistItem { Id = 22, ComplianceChecklistId = 3, Requirement = "Business Justification", Description = "The change request should explain why the change is needed.", Keywords = "business justification, justification, business need, reason", Weight = 10, CreatedAt = new DateTime(2026, 1, 1) },
            new ChecklistItem { Id = 23, ComplianceChecklistId = 3, Requirement = "Impact Analysis", Description = "The change request should explain expected impact.", Keywords = "impact analysis, impact, affected users, affected systems", Weight = 10, CreatedAt = new DateTime(2026, 1, 1) },
            new ChecklistItem { Id = 24, ComplianceChecklistId = 3, Requirement = "Risk Level", Description = "The change request should define risk level or risk rating.", Keywords = "risk level, risk rating, high risk, medium risk, low risk", Weight = 10, CreatedAt = new DateTime(2026, 1, 1) },
            new ChecklistItem { Id = 25, ComplianceChecklistId = 3, Requirement = "Rollback Plan", Description = "The change request should include a rollback or backout plan.", Keywords = "rollback plan, backout plan, restore, revert", Weight = 10, CreatedAt = new DateTime(2026, 1, 1) },
            new ChecklistItem { Id = 26, ComplianceChecklistId = 3, Requirement = "Testing Plan", Description = "The change request should include testing before or after implementation.", Keywords = "testing plan, test, validation, verification", Weight = 10, CreatedAt = new DateTime(2026, 1, 1) },
            new ChecklistItem { Id = 27, ComplianceChecklistId = 3, Requirement = "Implementation Schedule", Description = "The change request should define when the change will be implemented.", Keywords = "implementation schedule, implementation date, timeline, maintenance window", Weight = 10, CreatedAt = new DateTime(2026, 1, 1) },
            new ChecklistItem { Id = 28, ComplianceChecklistId = 3, Requirement = "Approval Process", Description = "The change request should define approval requirements.", Keywords = "approval, approved by, change advisory board, CAB, signoff", Weight = 10, CreatedAt = new DateTime(2026, 1, 1) },
            new ChecklistItem { Id = 29, ComplianceChecklistId = 3, Requirement = "Communication Plan", Description = "The change request should explain who will be notified and how.", Keywords = "communication plan, notify, notification, stakeholders, users", Weight = 10, CreatedAt = new DateTime(2026, 1, 1) },
            new ChecklistItem { Id = 30, ComplianceChecklistId = 3, Requirement = "Post-Change Review", Description = "The change request should include a post-change review or validation step.", Keywords = "post-change review, review, lessons learned, validation, follow-up", Weight = 10, CreatedAt = new DateTime(2026, 1, 1) }
        );
    }
}