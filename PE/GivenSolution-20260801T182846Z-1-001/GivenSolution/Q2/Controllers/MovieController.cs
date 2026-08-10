using CinemaMvcApp.Models;
using CinemaMvcApp.Services;
using Microsoft.AspNetCore.Mvc;

namespace CinemaMvcApp.Controllers
{
    public class MovieController : Controller
    {
        private readonly ICinemaApiService _apiService;

        public MovieController(ICinemaApiService apiService)
        {
            _apiService = apiService;
        }

        // GET /Movie
        // GET /Movie?genreId=1&priceSegment=Premium ( >= 90K )
        [HttpGet]
        public async Task<IActionResult> Index(int genreId = 0, string priceSegment = "All prices")
        {
            var genres = await _apiService.GetGenresAsync();
            var movies = await _apiService.SearchMoviesAsync(genreId, priceSegment);

            ViewBag.Genres = genres;
            ViewBag.SelectedGenreId = genreId;
            ViewBag.SelectedPriceSegment = priceSegment;

            return View(movies);
        }

        // GET /Movie/Analyze/{id}
        [HttpGet]
        public async Task<IActionResult> Analyze(int id)
        {
            var detail = await _apiService.GetMovieDetailAsync(id);
            if (detail == null)
            {
                return NotFound();
            }

            return View(detail);
        }
    }
}
