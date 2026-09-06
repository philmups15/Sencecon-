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
    DbSet<Handover> Handovers { get; }
    DbSet<PlantAttachment> PlantAttachments { get; }
    DbSet<CommissioningTestResult> CommissioningTestResults { get; }
    DbSet<CommissioningTestTemplate> CommissioningTestTemplates { get; }
    DbSet<WorkOrder> WorkOrders { get; }
    DbSet<WorkOrderChecklistItem> WorkOrderChecklistItems { get; }
    DbSet<WorkOrderPart> WorkOrderParts { get; }
    DbSet<WorkOrderLabour> WorkOrderLabour { get; }
    DbSet<WorkOrderAttachment> WorkOrderAttachments { get; }
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
    DbSet<SurveyMeasurement> SurveyMeasurements { get; }
    DbSet<SurveyObstruction> SurveyObstructions { get; }
    DbSet<SurveyPhoto> SurveyPhotos { get; }
    DbSet<Design> Designs { get; }
    DbSet<DesignAttachment> DesignAttachments { get; }
    DbSet<DesignRevision> DesignRevisions { get; }
    DbSet<BomItem> BomItems { get; }
    DbSet<NonConformity> NonConformities { get; }
    DbSet<Report> Reports { get; }
    DbSet<AuditLogEntry> AuditLogEntries { get; }
    DbSet<PasswordResetToken> PasswordResetTokens { get; }
    DbSet<IntegrationSetting> IntegrationSettings { get; }
    DbSet<RolePermission> RolePermissions { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}
