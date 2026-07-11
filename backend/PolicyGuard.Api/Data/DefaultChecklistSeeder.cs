using Microsoft.EntityFrameworkCore;
using PolicyGuard.Api.Models;

namespace PolicyGuard.Api.Data;

public static class DefaultChecklistSeeder
{
    public static async Task SeedAsync(PolicyGuardDbContext context)
    {
        // This seeder is intentionally idempotent. It checks checklist names before inserting so
        // redeployments do not create duplicate default templates in Azure SQL or local SQL Server.
        await AddChecklistIfMissingAsync(
            context,
            name: "Data Privacy and Retention Checklist",
            category: "Data Governance",
            description: "Reviews policies for proper handling, classification, retention, disposal, access, and privacy protection of organizational data.",
            items: new[]
            {
                NewItem(
                    "Data Classification",
                    "The policy should define how data is classified by sensitivity or business impact.",
                    "data classification, confidential, restricted, public, sensitive"),
                NewItem(
                    "Data Ownership",
                    "The policy should identify who owns or is responsible for the data.",
                    "data owner, ownership, steward, responsible party, accountable"),
                NewItem(
                    "Collection Purpose",
                    "The policy should explain why data is collected and how it will be used.",
                    "collection purpose, business purpose, data use, purpose limitation"),
                NewItem(
                    "Access Restrictions",
                    "The policy should define who may access data and under what conditions.",
                    "access restriction, authorized users, least privilege, permissions, access control"),
                NewItem(
                    "Retention Period",
                    "The policy should state how long data must be retained.",
                    "retention period, retention schedule, retained, data lifecycle, archive"),
                NewItem(
                    "Disposal Process",
                    "The policy should explain how data is securely deleted, destroyed, or disposed of.",
                    "disposal, destruction, delete, securely erased, data disposal"),
                NewItem(
                    "Privacy Protection",
                    "The policy should address protection of personal, confidential, or regulated information.",
                    "privacy, personal information, PII, confidential information, protected data"),
                NewItem(
                    "Regulatory Compliance",
                    "The policy should reference applicable legal, regulatory, or contractual obligations.",
                    "compliance, regulation, legal requirement, contractual obligation, audit requirement"),
                NewItem(
                    "Breach Notification",
                    "The policy should describe when privacy or data incidents must be reported.",
                    "breach notification, incident report, data breach, notify, escalation"),
                NewItem(
                    "Review Schedule",
                    "The policy should state when the policy will be reviewed or updated.",
                    "review schedule, annual review, policy review, updated, maintenance")
            });

        await AddChecklistIfMissingAsync(
            context,
            name: "Incident Response Checklist",
            category: "Security Operations",
            description: "Reviews incident response policies for detection, escalation, containment, communication, recovery, documentation, and post-incident improvement.",
            items: new[]
            {
                NewItem(
                    "Incident Definition",
                    "The policy should define what qualifies as a security, operational, or data incident.",
                    "incident definition, security incident, operational incident, event, disruption"),
                NewItem(
                    "Reporting Process",
                    "The policy should explain how incidents are reported and who receives the report.",
                    "report, reporting process, submit, notify, service desk, security team"),
                NewItem(
                    "Severity Levels",
                    "The policy should classify incidents by severity or priority.",
                    "severity, priority, critical, high, medium, low"),
                NewItem(
                    "Response Roles",
                    "The policy should identify the people or teams responsible for incident response.",
                    "response team, roles, responsibilities, incident commander, owner"),
                NewItem(
                    "Escalation Procedure",
                    "The policy should describe when and how incidents are escalated.",
                    "escalation, escalate, management notification, urgent, escalation path"),
                NewItem(
                    "Containment Steps",
                    "The policy should explain how the organization limits damage during an incident.",
                    "containment, isolate, disable access, limit impact, mitigation"),
                NewItem(
                    "Communication Plan",
                    "The policy should define how stakeholders, users, vendors, or leadership are informed.",
                    "communication plan, stakeholders, leadership, users, vendor, notification"),
                NewItem(
                    "Recovery Process",
                    "The policy should explain how systems, services, or data are restored.",
                    "recovery, restore, remediation, service restoration, backup"),
                NewItem(
                    "Documentation Requirements",
                    "The policy should require incident details, timelines, actions, and decisions to be documented.",
                    "documentation, timeline, incident record, evidence, action log"),
                NewItem(
                    "Post-Incident Review",
                    "The policy should require a review after resolution to identify lessons learned.",
                    "post-incident review, lessons learned, root cause, improvement, follow-up")
            });

        await context.SaveChangesAsync();
    }

    private static async Task AddChecklistIfMissingAsync(
        PolicyGuardDbContext context,
        string name,
        string category,
        string description,
        IEnumerable<ChecklistItem> items)
    {
        var exists = await context.ComplianceChecklists
            .AnyAsync(checklist => checklist.Name == name);

        if (exists)
        {
            return;
        }

        context.ComplianceChecklists.Add(new ComplianceChecklist
        {
            Name = name,
            Category = category,
            Description = description,
            CreatedAt = DateTime.UtcNow,
            Items = items.ToList()
        });
    }

    private static ChecklistItem NewItem(string requirement, string description, string keywords)
    {
        return new ChecklistItem
        {
            Requirement = requirement,
            Description = description,
            Keywords = keywords,
            Weight = 10,
            CreatedAt = DateTime.UtcNow
        };
    }
}
