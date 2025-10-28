namespace StockTracker.CrossCutting.Constants;

public static class ExtractorFunctionConstants
{
    public const string CronScheduleSettingName = "StockTracker:ExtractorFunction:CronExpression";
    public const string QueryDelaySettingName = "StockTracker:ExtractorFunction:QueryDelay";
    public const string CleanupDeprecatedInfo = "StockTracker:ExtractorFunction:CleanupDeprecatedInfo";
}