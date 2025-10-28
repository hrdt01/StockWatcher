using System.Net;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Enums;
using Microsoft.Extensions.Logging;
using StockTracker.CrossCutting.Constants;
using System.Net.Mime;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.OpenApi.Models;
using StockTracker.CrossCutting.ExceptionHandling.CustomExceptions;
using StockTracker.Infrastructure.Auth.Definition;
using StockTracker.Models.ApiModels;

namespace StockTracker.ExtractorFunction;

public class APIEndpoints
{
    private readonly IMediator _mediator;
    private readonly IConsumerAuthFlow _authFlow;
    private readonly IConfiguration _configuration;
    private readonly ILogger<APIEndpoints> _logger;

    public APIEndpoints(
        IMediator mediator,
        IConsumerAuthFlow authFlow,
        IConfiguration configuration,
        ILogger<APIEndpoints> logger)
    {
        _mediator = mediator;
        _authFlow = authFlow;
        _configuration = configuration;
        _logger = logger;
    }

    [Function(nameof(GetSymbols))]
    [OpenApiOperation(
        operationId: nameof(GetSymbols),
        tags: ["v1"],
        Description = "Get the symbol ticker stored in database",
        Deprecated = false,
        Visibility = OpenApiVisibilityType.Important)]
    [OpenApiSecurity("apikey_auth",
        SecuritySchemeType.ApiKey,
        In = OpenApiSecurityLocationType.Query,
        Name = "code")]
    [OpenApiResponseWithBody(HttpStatusCode.Unauthorized, MediaTypeNames.Application.Json, typeof(UnauthorizedException))]
    public async Task<IActionResult> GetSymbols(
        [HttpTrigger(
            AuthorizationLevel.Function, 
            "get", 
            Route = ApiConstants.GetSymbolsApiEndpointRoute)] HttpRequest req)
    {
        _authFlow.CheckAuthentication(req.Query["code"]!, _configuration);
        var result = await _mediator.Send(new SymbolsRequest());
            
        return new OkObjectResult(result);
    }

    [Function(nameof(GetDatesBySymbol))]
    [OpenApiOperation(
        operationId: nameof(GetDatesBySymbol),
        tags: ["v1"],
        Description = "Get the range of dates that the symbol ticker has stored in database",
        Deprecated = false,
        Visibility = OpenApiVisibilityType.Important)]
    [OpenApiSecurity("apikey_auth",
        SecuritySchemeType.ApiKey,
        In = OpenApiSecurityLocationType.Query,
        Name = "code")]
    [OpenApiParameter(
        name: "symbol", 
        In = ParameterLocation.Query, 
        Required = true, Type = typeof(string), 
        Description = "The symbol ticker parameter")]
    [OpenApiResponseWithBody(HttpStatusCode.Unauthorized, MediaTypeNames.Application.Json, typeof(UnauthorizedException))]
    public async Task<IActionResult> GetDatesBySymbol(
        [HttpTrigger(
            AuthorizationLevel.Function,
            "get",
            Route = ApiConstants.GetDatesBySymbolApiEndpointRoute)] HttpRequest req,
        string symbol)
    {
        _authFlow.CheckAuthentication(req.Query["code"]!, _configuration);

        var requestInstance = new DatesBySymbolRequest {Symbol = symbol};

        var result = await _mediator.Send(requestInstance);

        return new OkObjectResult(result);
    }

    [Function(nameof(GetInfoBySymbolDataRange))]
    [OpenApiOperation(
        operationId: nameof(GetInfoBySymbolDataRange),
        tags: ["v1"],
        Description = "Get the symbol ticker stored in database",
        Deprecated = false,
        Visibility = OpenApiVisibilityType.Important)]
    [OpenApiSecurity("apikey_auth",
        SecuritySchemeType.ApiKey,
        In = OpenApiSecurityLocationType.Query,
        Name = "code")]
    [OpenApiRequestBody(MediaTypeNames.Application.Json, typeof(SymbolRangeRequest))]
    [OpenApiResponseWithBody(HttpStatusCode.OK, MediaTypeNames.Application.Json,typeof(StockInfoResponse))]
    [OpenApiResponseWithBody(HttpStatusCode.BadRequest, MediaTypeNames.Application.Json, typeof(BadRequestException))]
    [OpenApiResponseWithBody(HttpStatusCode.InternalServerError, MediaTypeNames.Application.Json, typeof(RequestValidationException))]
    [OpenApiResponseWithBody(HttpStatusCode.Unauthorized, MediaTypeNames.Application.Json, typeof(UnauthorizedException))]
    public async Task<IActionResult> GetInfoBySymbolDataRange(
        [HttpTrigger(
            AuthorizationLevel.Function,
            "post",
            Route = ApiConstants.GetInfoBySymbolDataRangeApiEndpointRoute)]
        HttpRequest req)
    {
        _authFlow.CheckAuthentication(req.Query["code"]!, _configuration);
        var request = await req.ReadFromJsonAsync<SymbolRangeRequest>();

        var result = await _mediator.Send(request);

        return new OkObjectResult(result);
    }

    //[Function(nameof(TestKpi))]
    //[OpenApiOperation(
    //    operationId: nameof(TestKpi),
    //    tags: ["v1"],
    //    Description = "testkpi",
    //    Deprecated = false,
    //    Visibility = OpenApiVisibilityType.Important)]
    //[OpenApiSecurity("apikey_auth",
    //    SecuritySchemeType.ApiKey,
    //    In = OpenApiSecurityLocationType.Query,
    //    Name = "code")]
    //[OpenApiParameter(
    //    name: "symbol",
    //    In = ParameterLocation.Query,
    //    Required = true, Type = typeof(string),
    //    Description = "The symbol ticker parameter")]
    //[OpenApiParameter(
    //    name: "suppliedDate",
    //    In = ParameterLocation.Query,
    //    Required = true, Type = typeof(string),
    //    Description = "The queue message date parameter")]
    //public async Task<IActionResult> TestKpi(
    //    [HttpTrigger(
    //        AuthorizationLevel.Function,
    //        "get",
    //        Route = ExtractorFunctionConstants.TestKpiApiEndpointRoute)] HttpRequest req,
    //    string symbol, string suppliedDate)
    //{
    //    _authFlow.CheckAuthentication(req.Query["code"]!, _configuration);

    //    var suppliedCurrentDate = DateTime.Parse(suppliedDate);

    //    var kpiCalculationRequest = new KpiProcessMessageRequest
    //    {
    //        Symbol = symbol,
    //        ProcessDate = suppliedCurrentDate.AddDays(-1)
    //    };

    //    var result = await _mediator.Send(kpiCalculationRequest);

    //    return new OkObjectResult(result);
    //}
        
    [Function(nameof(GetKpisBySymbol))]
    [OpenApiOperation(
        operationId: nameof(GetKpisBySymbol),
        tags: ["v1"],
        Description = "Get the persisted kpis that the symbol ticker has stored in database",
        Deprecated = false,
        Visibility = OpenApiVisibilityType.Important)]
    [OpenApiSecurity("apikey_auth",
        SecuritySchemeType.ApiKey,
        In = OpenApiSecurityLocationType.Query,
        Name = "code")]
    [OpenApiParameter(
        name: "symbol",
        In = ParameterLocation.Query,
        Required = true, Type = typeof(string),
        Description = "The symbol ticker parameter")]
    [OpenApiResponseWithBody(HttpStatusCode.Unauthorized, MediaTypeNames.Application.Json, typeof(UnauthorizedException))]
    public async Task<IActionResult> GetKpisBySymbol(
        [HttpTrigger(
            AuthorizationLevel.Function,
            "get",
            Route = ApiConstants.GetKpisBySymbolApiEndpointRoute)] HttpRequest req,
        string symbol)
    {
        _authFlow.CheckAuthentication(req.Query["code"]!, _configuration);

        var requestInstance = new KpisBySymbolRequest { Symbol = symbol };

        var result = await _mediator.Send(requestInstance);

        return new OkObjectResult(result);
    }


    [Function(nameof(GetKpiDatesBySymbol))]
    [OpenApiOperation(
        operationId: nameof(GetKpiDatesBySymbol),
        tags: ["v1"],
        Description = "Get the range of dates that the symbol ticker has stored in database",
        Deprecated = false,
        Visibility = OpenApiVisibilityType.Important)]
    [OpenApiSecurity("apikey_auth",
        SecuritySchemeType.ApiKey,
        In = OpenApiSecurityLocationType.Query,
        Name = "code")]
    [OpenApiParameter(
        name: "symbol",
        In = ParameterLocation.Query,
        Required = true, Type = typeof(string),
        Description = "The symbol ticker parameter")]
    [OpenApiResponseWithBody(HttpStatusCode.Unauthorized, MediaTypeNames.Application.Json, typeof(UnauthorizedException))]
    public async Task<IActionResult> GetKpiDatesBySymbol(
        [HttpTrigger(
            AuthorizationLevel.Function,
            "get",
            Route = ApiConstants.GetKpiDatesBySymbolApiEndpointRoute)] HttpRequest req,
        string symbol)
    {
        _authFlow.CheckAuthentication(req.Query["code"]!, _configuration);

        var requestInstance = new KpiDatesBySymbolRequest { Symbol = symbol };

        var result = await _mediator.Send(requestInstance);

        return new OkObjectResult(result);
    }

    [Function(nameof(GetKpisBySymbolDateRange))]
    [OpenApiOperation(
        operationId: nameof(GetKpisBySymbolDateRange),
        tags: ["v1"],
        Description = "Get the calculated kpi stored in database based on symbol ticker and date range",
        Deprecated = false,
        Visibility = OpenApiVisibilityType.Important)]
    [OpenApiSecurity("apikey_auth",
        SecuritySchemeType.ApiKey,
        In = OpenApiSecurityLocationType.Query,
        Name = "code")]
    [OpenApiRequestBody(MediaTypeNames.Application.Json, typeof(KpisBySymbolDateRangeRequest))]
    [OpenApiResponseWithBody(HttpStatusCode.OK, MediaTypeNames.Application.Json, typeof(StockKpiResponse))]
    [OpenApiResponseWithBody(HttpStatusCode.BadRequest, MediaTypeNames.Application.Json, typeof(BadRequestException))]
    [OpenApiResponseWithBody(HttpStatusCode.InternalServerError, MediaTypeNames.Application.Json, typeof(RequestValidationException))]
    [OpenApiResponseWithBody(HttpStatusCode.Unauthorized, MediaTypeNames.Application.Json, typeof(UnauthorizedException))]
    public async Task<IActionResult> GetKpisBySymbolDateRange(
        [HttpTrigger(
            AuthorizationLevel.Function,
            "post",
            Route = ApiConstants.GetKpisBySymbolDateRangeApiEndpointRoute )]
        HttpRequest req)
    {
        _authFlow.CheckAuthentication(req.Query["code"]!, _configuration);
        var request = await req.ReadFromJsonAsync<KpisBySymbolDateRangeRequest>();

        var result = await _mediator.Send(request);

        return new OkObjectResult(result);
    }
    
    [Function(nameof(CreateKpiCalculationRequest))]
    [OpenApiOperation(
        operationId: nameof(CreateKpiCalculationRequest),
        tags: ["v1"],
        Description = "Create a message to calculate kpi based on symbol ticker and date",
        Deprecated = false,
        Visibility = OpenApiVisibilityType.Important)]
    [OpenApiSecurity("apikey_auth",
        SecuritySchemeType.ApiKey,
        In = OpenApiSecurityLocationType.Query,
        Name = "code")]
    [OpenApiRequestBody(MediaTypeNames.Application.Json, typeof(KpiProcessMessageRequest))]
    [OpenApiResponseWithBody(HttpStatusCode.OK, MediaTypeNames.Application.Json, typeof(bool))]
    [OpenApiResponseWithBody(HttpStatusCode.BadRequest, MediaTypeNames.Application.Json, typeof(BadRequestException))]
    [OpenApiResponseWithBody(HttpStatusCode.InternalServerError, MediaTypeNames.Application.Json, typeof(RequestValidationException))]
    [OpenApiResponseWithBody(HttpStatusCode.Unauthorized, MediaTypeNames.Application.Json, typeof(UnauthorizedException))]
    public async Task<IActionResult> CreateKpiCalculationRequest(
        [HttpTrigger(
            AuthorizationLevel.Function,
            "post",
            Route = ApiConstants.CreateKpiCalculationApiEndpointRoute )]
        HttpRequest req)
    {
        _authFlow.CheckAuthentication(req.Query["code"]!, _configuration);
        var request = await req.ReadFromJsonAsync<KpiProcessMessageRequest>();

        var result = await _mediator.Send(request);

        return new OkObjectResult(result);
    }

    
    [Function(nameof(GetTrackedCompanies))]
    [OpenApiOperation(
        operationId: nameof(GetTrackedCompanies),
        tags: ["v1"],
        Description = "Get the collection of companies stored in database that are been tracked",
        Deprecated = false,
        Visibility = OpenApiVisibilityType.Important)]
    [OpenApiSecurity("apikey_auth",
        SecuritySchemeType.ApiKey,
        In = OpenApiSecurityLocationType.Query,
        Name = "code")]
    [OpenApiParameter(
        name: "enabled",
        In = ParameterLocation.Query,
        Required = true, Type = typeof(string),
        Description = "The tracked enable parameter")]
    [OpenApiResponseWithBody(HttpStatusCode.Unauthorized, MediaTypeNames.Application.Json, typeof(UnauthorizedException))]
    public async Task<IActionResult> GetTrackedCompanies(
        [HttpTrigger(
            AuthorizationLevel.Function,
            "get",
            Route = ApiConstants.GetTrackedCompaniesApiEndpointRoute)] HttpRequest req,
        string enabled)
    {
        _authFlow.CheckAuthentication(req.Query["code"]!, _configuration);

        var requestInstance = new TrackedCompaniesRequest { Enabled = bool.Parse(enabled)};

        var result = await _mediator.Send(requestInstance);

        return new OkObjectResult(result);
    }

    [Function(nameof(ExecuteInitialMigration))]
    [OpenApiOperation(
        operationId: nameof(ExecuteInitialMigration),
        tags: ["v1"],
        Description = "Execute the initial migration to database",
        Deprecated = false,
        Visibility = OpenApiVisibilityType.Important)]
    [OpenApiSecurity("apikey_auth",
        SecuritySchemeType.ApiKey,
        In = OpenApiSecurityLocationType.Query,
        Name = "code")]
    [OpenApiResponseWithBody(HttpStatusCode.Unauthorized, MediaTypeNames.Application.Json, typeof(UnauthorizedException))]
    public async Task<IActionResult> ExecuteInitialMigration(
        [HttpTrigger(
            AuthorizationLevel.Function,
            "get",
            Route = ApiConstants.ExecuteInitialMigrationApiEndpointRoute)] HttpRequest req)
    {
        _authFlow.CheckAuthentication(req.Query["code"]!, _configuration);

        var requestInstance = new InitialMigrationRequest();

        var result = await _mediator.Send(requestInstance);

        return new OkObjectResult(result);
    }

    [Function(nameof(SaveTrackedCompany))]
    [OpenApiOperation(
        operationId: nameof(SaveTrackedCompany),
        tags: ["v1"],
        Description = "Save the tracked company info in database",
        Deprecated = false,
        Visibility = OpenApiVisibilityType.Important)]
    [OpenApiSecurity("apikey_auth",
        SecuritySchemeType.ApiKey,
        In = OpenApiSecurityLocationType.Query,
        Name = "code")]
    [OpenApiRequestBody(MediaTypeNames.Application.Json, typeof(SaveTrackedCompanyRequest))]
    [OpenApiResponseWithBody(HttpStatusCode.OK, MediaTypeNames.Application.Json, typeof(bool))]
    [OpenApiResponseWithBody(HttpStatusCode.BadRequest, MediaTypeNames.Application.Json, typeof(BadRequestException))]
    [OpenApiResponseWithBody(HttpStatusCode.InternalServerError, MediaTypeNames.Application.Json, typeof(RequestValidationException))]
    [OpenApiResponseWithBody(HttpStatusCode.Unauthorized, MediaTypeNames.Application.Json, typeof(UnauthorizedException))]
    public async Task<IActionResult> SaveTrackedCompany(
        [HttpTrigger(
            AuthorizationLevel.Function,
            "post",
            Route = ApiConstants.PostSaveTrackedCompanyApiEndpointRoute )]
        HttpRequest req)
    {
        _authFlow.CheckAuthentication(req.Query["code"]!, _configuration);
        var request = await req.ReadFromJsonAsync<SaveTrackedCompanyRequest>();

        var result = await _mediator.Send(request);

        return new OkObjectResult(result);
    }
}