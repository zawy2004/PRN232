using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Q1.DTOs;
using Q1.Models;

namespace Q1.Controllers
{
    [Route("api/filter-movies")]
    [ApiController]
    public class MoviesController : Controller
    {
        private readonly Prn232PeSu2611Context _context;
        public MoviesController(Prn232PeSu2611Context context)
        {
            _context = context;
        }
        [HttpGet]
        public async Task<IActionResult> FilterMovies(
           [FromQuery] int? genreId,
           [FromQuery] int? maxDuration
          )
        {
            // Error handling: 400 Bad Request if page or pageSize is invalid.
            if (maxDuration < 30)
            {
                return BadRequest("maxDuration must be at least 30 minutes");
            }
            var genre = await _context.Genres.FindAsync(genreId);
            var entityQuery = _context.Movies.Where(m => m.Genres.Any(g => g.GenreId == genreId.Value));



            var projected = entityQuery.Select(s => new filterMovieResponse
            {
                MovieId = s.MovieId,
                Title = s.Title,
                Duration = s.Duration,
                BasePrice = s.BasePrice,
                TotalQuantitySold = 0,
                LastTicketSeat = s.TicketDetails
                        .OrderByDescending(td => td.Ticket!.BookingDate)
                        .Select(td => td.SeatNumber)
                        .FirstOrDefault() ?? "No Seats",
                GenreList = s.Genres
                        .Select(g => g.GenreName!)
                        .ToList()
            });

     
            return Ok(projected);
        }

    }
}
