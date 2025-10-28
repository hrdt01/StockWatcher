namespace StockTracker.Frontend.Constants;

public class FrontendConstants
{
    public const string FrontendClientFactoryName = "StockTrackerFrontend";
    public const string AuthClientFactoryName = "StockTrackerAuth";
    public const string IdentityApiBaseUrlSettingName = "IdentityApiBaseUrl";

    public const string ResiliencePipelineName = "StockTracker";

    #region ApiEndpoints

    public const string GetSymbolsApiEndpointRoute = "api/tracker/getsymbols";
    public const string GetDatesBySymbolApiEndpointRoute = "api/tracker/getdates";
    public const string GetInfoBySymbolDateRangeApiEndpointRoute = "api/tracker/getstockinfo";
    public const string GetKpisBySymbolApiEndpointRoute = "api/tracker/getkpisbysymbol";
    public const string GetKpisBySymbolDateRangeApiEndpointRoute = "api/tracker/getkpiinfo";
    public const string GetTrackedCompaniesApiEndpointRoute = "api/tracker/gettrackedcompanies";
    public const string PostSaveTrackedCompanyApiEndpointRoute = "api/tracker/savetrackedcompany";
    public const string GetKpiDatesBySymbolApiEndpointRoute = "api/tracker/getkpidates";
    public const string CreateKpiCalculationApiEndpointRoute = "api/tracker/requestkpicalculation";

    public const string LoginApiEndpointRoute = "api/Authenticate/login";
    public const string TokenRefreshApiEndpointRoute = "api/Authenticate/token/refresh";
    public const string TokenCloseToExpireApiEndpointRoute = "api/Authenticate/token/closetoexpire";
    public const string LogoutApiEndpointRoute = "api/Authenticate/token/revoke";
    public const string UserInfoApiEndpointRoute = "api/Authenticate/manage/info";
        
    #endregion

}