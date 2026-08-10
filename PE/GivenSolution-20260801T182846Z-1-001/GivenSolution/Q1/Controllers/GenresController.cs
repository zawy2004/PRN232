using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Q1.Models;

namespace Q1.Controllers
{
    [Route("api/Genres")]
    [ApiController]
    public class GenresController : Controller
    {
        private readonly Prn232PeSu2611Context _context;
        public GenresController(Prn232PeSu2611Context context)
        {
            _context = context;
        }

        [HttpDelete("{genresId}")]
        public async Task<IActionResult> DeleteGenres(int genresId)
        {
            var genre = await _context.Genres.FindAsync(genresId);
            if (genre == null)
            {
                return NotFound();
            }


            var isUsedByMovies = await _context.Movies.FindAsync(genre);
                

            if (isUsedByMovies != null)
            {
                return BadRequest("Cannot delete genre that is assigned to active movie ");
            }
            try
            {
                _context.Genres.Remove(genre);

            }
            catch
            {
                return BadRequest("Cannot delete genre that is assigned to active movie ");
            }
            
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
