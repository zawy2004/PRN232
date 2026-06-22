using FUNewsManagement.BusinessLogic.Common;
using FUNewsManagement.BusinessLogic.Dtos;
using FUNewsManagement.BusinessLogic.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FUNewsManagement.Api.Controllers;

[ApiController]
[Route("api/reports")]
[Authorize(Roles = RoleNames.Admin)]
public class ReportsController : ControllerBase
{
    private readonly INewsService _newsService;

    public ReportsController(INewsService newsService)
    {
        _newsService = newsService;
    }

    /// <summary>News-by-period statistic report, sorted descending by CreatedDate.</summary>
    [HttpGet]
    public async Task<ActionResult<List<ReportItemDto>>> GetReport([FromQuery] DateTime startDate, [FromQuery] DateTime endDate) =>
        Ok(await _newsService.GetReportAsync(startDate, endDate));
}
