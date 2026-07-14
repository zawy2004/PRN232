using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Q2.Models;

namespace Q2.Controllers
{
    [Route("Schedule")]
    public class ScheduleController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;

        private static readonly JsonSerializerOptions JsonOptions = new()
        {
            PropertyNameCaseInsensitive = true
        };

        public ScheduleController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        // Schedule List Page: /Schedule
        [HttpGet("")]
        public async Task<IActionResult> Index(string? instructor, string? course)
        {
            var client = _httpClientFactory.CreateClient();

            // Combine the base URL (from appsettings.json) with the endpoint using string concatenation.
            var endpoint = "/api/schedules/search?instructor=" + Uri.EscapeDataString(instructor ?? "")
                         + "&course=" + Uri.EscapeDataString(course ?? "");
            var url = Utilities.GetAbsoluteUrl(endpoint);

            var schedules = new List<ScheduleListItem>();
            try
            {
                var response = await client.GetAsync(url);
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    schedules = JsonSerializer.Deserialize<List<ScheduleListItem>>(json, JsonOptions) ?? new();
                }
            }
            catch
            {
                // If the API is unreachable, show an empty list.
            }

            ViewBag.Instructor = instructor;
            ViewBag.Course = course;
            return View(schedules);
        }

        // Schedule Detail Page: /Schedule/{SectionId}
        [HttpGet("{sectionId:int}")]
        public async Task<IActionResult> Details(int sectionId)
        {
            var client = _httpClientFactory.CreateClient();

            var endpoint = "/api/schedules/" + sectionId;
            var url = Utilities.GetAbsoluteUrl(endpoint);

            ScheduleDetail? detail = null;
            try
            {
                var response = await client.GetAsync(url);
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    detail = JsonSerializer.Deserialize<ScheduleDetail>(json, JsonOptions);
                }
            }
            catch
            {
                // ignore, handled below
            }

            if (detail == null)
            {
                return NotFound();
            }

            return View(detail);
        }
    }
}
