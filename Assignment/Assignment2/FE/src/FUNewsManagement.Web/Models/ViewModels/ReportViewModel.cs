using FUNewsManagement.Web.Models.Dtos;

namespace FUNewsManagement.Web.Models.ViewModels;

public class ReportViewModel
{
    public DateTime StartDate { get; set; } = DateTime.Today.AddDays(-30);

    public DateTime EndDate { get; set; } = DateTime.Today;

    public List<ReportItemDto> Items { get; set; } = new();
}
