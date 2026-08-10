using System.Net;
using System.Text.Json;
using CinemaMvcApp.Models;

namespace CinemaMvcApp.Services
{
    public class CinemaApiService : ICinemaApiService
    {
        private readonly HttpClient _httpClient;
        private readonly string _baseUrl;

        private static readonly JsonSerializerOptions JsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        public CinemaApiService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;

            // GivenAPIBaseUrl is read from appsettings.json, e.g. "http://localhost:5100"
            _baseUrl = configuration["GivenAPIBaseUrl"] ?? string.Empty;
        }

        // GET /api/genres
        public async Task<List<GenreDto>> GetGenresAsync()
        {
            // Explicit string concatenation for the endpoint, as required by the assignment
            string url = _baseUrl + "/api/genres";

            var response = await _httpClient.GetAsync(url);
            if (!response.IsSuccessStatusCode)
            {
                return new List<GenreDto>();
            }

            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<List<GenreDto>>(json, JsonOptions) ?? new List<GenreDto>();
        }

        // GET /api/movies/search?genreId={id}&priceSegment={segment}
        public async Task<List<MovieSearchDto>> SearchMoviesAsync(int genreId, string priceSegment)
        {
            string url = _baseUrl + "/api/movies/search?genreId=" + genreId
                + "&priceSegment=" + Uri.EscapeDataString(priceSegment);

            var response = await _httpClient.GetAsync(url);
            if (!response.IsSuccessStatusCode)
            {
                return new List<MovieSearchDto>();
            }

            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<List<MovieSearchDto>>(json, JsonOptions) ?? new List<MovieSearchDto>();
        }

        // GET /api/movies/{movieId}
        public async Task<MovieDetailDto?> GetMovieDetailAsync(int movieId)
        {
            string url = _baseUrl + "/api/movies/" + movieId;

            var response = await _httpClient.GetAsync(url);
            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                return null;
            }
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<MovieDetailDto>(json, JsonOptions);
        }
    }
}
