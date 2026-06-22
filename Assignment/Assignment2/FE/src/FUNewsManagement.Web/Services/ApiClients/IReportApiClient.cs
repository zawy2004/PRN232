using FUNewsManagement.Web.Models.Dtos;

namespace FUNewsManagement.Web.Services.ApiClients;

public interface IReportApiClient
{
    Task<List<ReportItemDto>> GetReportAsync(DateTime startDate, DateTime endDate);
}
