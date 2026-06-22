using FUNewsManagement.Web.Models.ViewModels;
using FUNewsManagement.Web.Services.ApiClients;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FUNewsManagement.Web.Controllers;

[Authorize(Roles = "Admin")]
public class ReportController : Controller
{
    private readonly IReportApiClient _reportApiClient;

    public ReportController(IReportApiClient reportApiClient)
    {
        _reportApiClient = reportApiClient;
    }

    public async Task<IActionResult> Index(DateTime? startDate, DateTime? endDate)
    {
        var vm = new ReportViewModel
        {
            StartDate = startDate ?? DateTime.Today.AddDays(-30),
            EndDate = endDate ?? DateTime.Today,
        };
        vm.Items = await _reportApiClient.GetReportAsync(vm.StartDate, vm.EndDate.AddDays(1).AddTicks(-1));
        return View(vm);
    }
}
