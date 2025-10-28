namespace StockTracker.CrossCutting.Constants;

public static class ApiConstants
{
    #region MarketStack

    public const string MarketStackBaseUrl = "https://api.marketstack.com/";
    public const string ClientFactoryName = "MarketStackNamedClient";
    public const string EndOfDayEndpointUrl = "v2/eod";
    public const string RequiredAccessKeyQueryParam = "access_key";
    public const string RequiredSymbolsQueryParam = "symbols";
    public const string OptionalDateFromQueryParam = "date_from";
    public const string OptionalDateToQueryParam = "date_to";

    public const string ResiliencePipelineName = "MarketStack";
    public const string ExtractorServiceName = "MarketStack";
    public const string AccessKeySettingName = "StockTracker:ThirdParty:MarketStack:AccessKey";

    #endregion

    #region API Endpoints

    public const string GetSymbolsApiEndpointRoute = "getsymbols";
    public const string GetDatesBySymbolApiEndpointRoute = "getdates";
    public const string GetInfoBySymbolDataRangeApiEndpointRoute = "getstockinfo";
    public const string GetKpisBySymbolApiEndpointRoute = "getkpisbysymbol";
    public const string GetKpisBySymbolDateRangeApiEndpointRoute = "getkpiinfo";
    public const string GetKpiDatesBySymbolApiEndpointRoute = "getkpidates";
    public const string CreateKpiCalculationApiEndpointRoute = "requestkpicalculation";
    public const string TestKpiApiEndpointRoute = "testkpi";
    
    public const string GetTrackedCompaniesApiEndpointRoute = "gettrackedcompanies";
    public const string GetTrackedCompaniesFilteredCacheKey = "tracked-companies-filtered";
    public const string GetTrackedCompaniesAllCacheKey = "tracked-companies-all";
    public const string ExecuteInitialMigrationApiEndpointRoute = "initialmigration";
    public const string PostSaveTrackedCompanyApiEndpointRoute = "savetrackedcompany";

    public const string FrontendClientFactoryName = "StockTrackerFrontend";
    public const string ApiBaseUrlSettingName = "ApiBaseUrl";
    public const string ApiKeySettingName = "ApiKey";
    public const string FunctionResiliencePipelineName = "StockTracker";

    public const string LoginAuthApiEndpointRoute = "login";
    public const string TokenCloseToExpireAuthApiEndpointRoute = "token/closetoexpire";
    public const string TokenRefreshAuthApiEndpointRoute = "token/refresh";
    public const string TokenRevokeAuthApiEndpointRoute = "token/revoke";
    public const string GetUserInfoAuthApiEndpointRoute = "manage/info";

    #endregion

}