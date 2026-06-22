using System.Globalization;
using System.Net.Http.Json;
using FUNewsManagement.Web.Common;
using FUNewsManagement.Web.Models.Dtos;

namespace FUNewsManagement.Web.Services.ApiClients;

public class ReportApiClient : IReportApiClient
{
    private readonly HttpClient _httpClient;

    public ReportApiClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<List<ReportItemDto>> GetReportAsync(DateTime startDate, DateTime endDate)
    {
        var start = startDate.ToString("o", CultureInfo.InvariantCulture);
        var end = endDate.ToString("o", CultureInfo.InvariantCulture);
        var url = $"api/reports?startDate={Uri.EscapeDataString(start)}&endDate={Uri.EscapeDataString(end)}";
        return await _httpClient.GetJsonSafeAsync<List<ReportItemDto>>(url) ?? new();
    }
}
