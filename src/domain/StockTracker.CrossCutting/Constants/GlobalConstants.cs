namespace StockTracker.CrossCutting.Constants;

public static class GlobalConstants
{
    #region Azure Table settings

    public const string MarketTrackerEndOfDayInfoTableName = "EndOfDayTracker";

    public const string AzureTableRepositorySettingsSectionKeyName = "StockTracker:Repository:AzureTable";

    public const string StockKpiTableName = "StockKpi";

    public const string AzureQueueRepositorySettingsSectionKeyName = "StockTracker:Repository:AzureQueue";
    
    public const string KpiProcessorQueueName = "kpi-message-broker";
    public const string CleanupProcessorQueueName = "cleanup-message-broker";

    public const string TrackedCompanyTableName = "TrackedCompany";

    #endregion

    #region User roles

    public const string UserRoleAdmin = "Admin";
    public const string UserRoleViewer = "Viewer";

    #endregion
}