using System.Net.Http.Headers;
using System.Net.Http.Json;
using Q2.Models;
using Q2;

namespace Q2.Services
{
    public class ApiClient
    {
        private readonly HttpClient _http;
        public ApiClient(HttpClient http) => _http = http;

        public async Task<List<EquipmentDto>> GetEquipmentsAsync()
            => await _http.GetFromJsonAsync<List<EquipmentDto>>(Utilities.GetAbsoluteUrl("api/equipments")) ?? new();

        public async Task<EquipmentDto?> GetEquipmentAsync(int id)
        {
            var res = await _http.GetAsync(Utilities.GetAbsoluteUrl($"api/equipments/{id}"));
            if (!res.IsSuccessStatusCode) return null;
            return await res.Content.ReadFromJsonAsync<EquipmentDto>();
        }

        public async Task<(bool ok, string? error)> CreateRentalAsync(string token, CreateRentalDto dto)
        {
            var req = new HttpRequestMessage(HttpMethod.Post, Utilities.GetAbsoluteUrl("api/rentals"))
            {
                Content = JsonContent.Create(dto)
            };
            req.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var res = await _http.SendAsync(req);
            return res.IsSuccessStatusCode ? (true, null) : (false, await res.Content.ReadAsStringAsync());
        }

        public async Task<(bool ok, LoginResponseDto? data)> LoginAsync(string email, string password)
        {
            var res = await _http.PostAsJsonAsync(Utilities.GetAbsoluteUrl("api/auth/login"), new { email, password });
            if (!res.IsSuccessStatusCode) return (false, null);
            return (true, await res.Content.ReadFromJsonAsync<LoginResponseDto>());
        }
    }
}
