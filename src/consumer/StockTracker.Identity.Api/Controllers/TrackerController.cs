using Microsoft.AspNetCore.Mvc;
using StockTracker.CrossCutting.Constants;
using StockTracker.Identity.Api.Areas.Identity.Models;
using StockTracker.Identity.Api.Areas.Tracker.Services;
using System.Net.Mime;
using Microsoft.AspNetCore.Authorization;
using StockTracker.Models.FrontendModels;

namespace StockTracker.Identity.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
[Produces(MediaTypeNames.Application.Json)]
[Consumes(MediaTypeNames.Application.Json)]
[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
public class TrackerController : ControllerBase
{
    private readonly ITrackerService _trackerService;
    private readonly ILogger<TrackerController> _logger;

    public TrackerController(
        ITrackerService trackerService,
        ILogger<TrackerController> logger)
    {
        ArgumentNullException.ThrowIfNull(logger);
        ArgumentNullException.ThrowIfNull(trackerService);

        _trackerService = trackerService;
        _logger = logger;
    }

    [HttpGet(ApiConstants.GetSymbolsApiEndpointRoute)]
    public async Task<IActionResult> GetSymbols()
    {
        try
        {
            return Ok(await _trackerService.GetSymbols());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
            return StatusCode(StatusCodes.Status500InternalServerError);
        }
    }

    [AllowAnonymous]
    [HttpGet(ApiConstants.GetTrackedCompaniesApiEndpointRoute)]
    public async Task<IActionResult> GetTrackedCompanies(bool enabled)
    {
        try
        {
            return Ok(await _trackerService.GetTrackedCompanies(enabled));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
            return StatusCode(StatusCodes.Status500InternalServerError);
        }
    }

    [HttpGet(ApiConstants.GetDatesBySymbolApiEndpointRoute)]
    public async Task<IActionResult> GetDatesBySymbol(string symbol)
    {
        try
        {
            return Ok(await _trackerService.GetDatesBySymbol(symbol));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
            return StatusCode(StatusCodes.Status500InternalServerError);
        }
    }

    [HttpGet(ApiConstants.GetInfoBySymbolDataRangeApiEndpointRoute)]
    public async Task<IActionResult> GetInfoBySymbolDateRange(string symbol, string from, string to)
    {
        try
        {
            return Ok(await _trackerService.GetInfoBySymbolDateRange(symbol,from, to));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
            return StatusCode(StatusCodes.Status500InternalServerError);
        }
    }

    [HttpGet(ApiConstants.GetKpisBySymbolDateRangeApiEndpointRoute)]
    public async Task<IActionResult> GetKpisBySymbolDateRange(string symbolKpi, string from, string to)
    {
        try
        {
            return Ok(await _trackerService.GetKpisBySymbolDateRange(symbolKpi, from, to));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
            return StatusCode(StatusCodes.Status500InternalServerError);
        }
    }

    [HttpGet(ApiConstants.GetKpisBySymbolApiEndpointRoute)]
    public async Task<IActionResult> GetKpisBySymbol(string symbol)
    {
        try
        {
            return Ok(await _trackerService.GetKpisBySymbol(symbol));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
            return StatusCode(StatusCodes.Status500InternalServerError);
        }
    }

    [HttpGet(ApiConstants.GetKpiDatesBySymbolApiEndpointRoute)]
    public async Task<IActionResult> GetKpiDatesBySymbol(string symbol)
    {
        try
        {
            return Ok(await _trackerService.GetKpiDatesBySymbol(symbol));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
            return StatusCode(StatusCodes.Status500InternalServerError);
        }
    }

    [HttpPost(ApiConstants.PostSaveTrackedCompanyApiEndpointRoute)]
    public async Task<IActionResult> SaveTrackedCompany(TrackedCompany source)
    {
        try
        {
            return Ok(await _trackerService.SaveTrackedCompany(source));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
            return StatusCode(StatusCodes.Status500InternalServerError);
        }
    }

    [HttpPost(ApiConstants.CreateKpiCalculationApiEndpointRoute)]
    public async Task<IActionResult> CreateKpiCalculationRequest(KpiProcessRequest source)
    {
        try
        {
            return Ok(await _trackerService.CreateKpiCalculationRequest(source));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
            return StatusCode(StatusCodes.Status500InternalServerError);
        }
    }
    
}