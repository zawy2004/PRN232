using CinemaMvcApp.Models;

namespace CinemaMvcApp.Services
{
    public interface ICinemaApiService
    {
        Task<List<GenreDto>> GetGenresAsync();
        Task<List<MovieSearchDto>> SearchMoviesAsync(int genreId, string priceSegment);
        Task<MovieDetailDto?> GetMovieDetailAsync(int movieId);
    }
}
