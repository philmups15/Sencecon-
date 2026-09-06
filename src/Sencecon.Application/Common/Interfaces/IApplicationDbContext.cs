using Microsoft.EntityFrameworkCore;
using Sencecon.Domain.Entities;

namespace Sencecon.Application.Common.Interfaces;

public interface IApplicationDbContext
{
    // Atomic counter for human-readable entity codes (see EntityCodeGenerator).
    Task<long> NextSequenceValueAsync(string sequenceName, CancellationToken cancellationToken);

    DbSet<TodoItem> TodoItems { get; }
    DbSet<User> Users { get; }
    DbSet<Plant> Plants { get; }
    DbSet<PlantAttachment> PlantAttachments { get; }
    DbSet<CommissioningTestResult> CommissioningTestResults { get; }
    DbSet<WorkOrder> WorkOrders { get; }
    DbSet<Project> Projects { get; }
    DbSet<ProjectMilestone> ProjectMilestones { get; }
    DbSet<ProjectTask> ProjectTasks { get; }
    DbSet<Subcontractor> Subcontractors { get; }
    DbSet<ProjectRisk> ProjectRisks { get; }
    DbSet<ProjectBudgetLine> ProjectBudgetLines { get; }
    DbSet<Opportunity> Opportunities { get; }
    DbSet<OpportunityAttachment> OpportunityAttachments { get; }
    DbSet<OpportunityNote> OpportunityNotes { get; }
    DbSet<OpportunityActivity> OpportunityActivities { get; }
    DbSet<Survey> Surveys { get; }
    DbSet<Design> Designs { get; }
    DbSet<BomItem> BomItems { get; }
    DbSet<NonConformity> NonConformities { get; }
    DbSet<Report> Reports { get; }
    DbSet<AuditLogEntry> AuditLogEntries { get; }
    DbSet<PasswordResetToken> PasswordResetTokens { get; }
    DbSet<IntegrationSetting> IntegrationSettings { get; }
    DbSet<RolePermission> RolePermissions { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}
