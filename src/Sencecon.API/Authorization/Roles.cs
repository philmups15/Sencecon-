namespace Sencecon.API.Authorization;

public static class Roles
{
    public const string Admin = "Admin";
    public const string User = "User";
    public const string ProjectManager = "ProjectManager";
    public const string Sales = "Sales";
    public const string DesignEngineer = "DesignEngineer";
}

// Per-module Read/Write role lists, matching the access matrix:
//
// Module            Admin  User  Sales  ProjectManager  DesignEngineer
// Opportunities      RW     R     RW         R                -
// Surveys            RW     R     -          R                RW
// Designs            RW     R     -          R                RW
// BomItems           RW     R     -          R                RW
// Projects           RW     R     R          RW               R
// Plants             RW     R     -          RW               -
// WorkOrders         RW     R     -          RW               -
// NonConformities    RW     R     -          RW               -
// Reports            RW     R     R          RW               R
// Users (admin)      RW     -     -          -                -
// AuditLog           R      -     -          -                -
public static class ModuleAccess
{
    public const string OpportunitiesRead = $"{Roles.Admin},{Roles.User},{Roles.Sales},{Roles.ProjectManager}";
    public const string OpportunitiesWrite = $"{Roles.Admin},{Roles.Sales}";

    public const string SurveysRead = $"{Roles.Admin},{Roles.User},{Roles.ProjectManager},{Roles.DesignEngineer}";
    public const string SurveysWrite = $"{Roles.Admin},{Roles.DesignEngineer}";

    public const string DesignsRead = $"{Roles.Admin},{Roles.User},{Roles.ProjectManager},{Roles.DesignEngineer}";
    public const string DesignsWrite = $"{Roles.Admin},{Roles.DesignEngineer}";

    public const string BomItemsRead = $"{Roles.Admin},{Roles.User},{Roles.ProjectManager},{Roles.DesignEngineer}";
    public const string BomItemsWrite = $"{Roles.Admin},{Roles.DesignEngineer}";

    public const string ProjectsRead = $"{Roles.Admin},{Roles.User},{Roles.Sales},{Roles.ProjectManager},{Roles.DesignEngineer}";
    public const string ProjectsWrite = $"{Roles.Admin},{Roles.ProjectManager}";

    public const string PlantsRead = $"{Roles.Admin},{Roles.User},{Roles.ProjectManager}";
    public const string PlantsWrite = $"{Roles.Admin},{Roles.ProjectManager}";

    public const string WorkOrdersRead = $"{Roles.Admin},{Roles.User},{Roles.ProjectManager}";
    public const string WorkOrdersWrite = $"{Roles.Admin},{Roles.ProjectManager}";

    public const string NonConformitiesRead = $"{Roles.Admin},{Roles.User},{Roles.ProjectManager}";
    public const string NonConformitiesWrite = $"{Roles.Admin},{Roles.ProjectManager}";

    public const string ReportsRead = $"{Roles.Admin},{Roles.User},{Roles.Sales},{Roles.ProjectManager},{Roles.DesignEngineer}";
    public const string ReportsWrite = $"{Roles.Admin},{Roles.ProjectManager}";
}
