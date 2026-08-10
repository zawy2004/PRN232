namespace CinemaMvcApp.Models
{
    // Maps to GET /api/movies/search?genreId={id}&priceSegment={segment} response
    public class MovieSearchDto
    {
        public int MovieID { get; set; }
        public string Title { get; set; } = string.Empty;
        public int Duration { get; set; }
        public decimal BasePrice { get; set; }
        public List<string> Genres { get; set; } = new();
    }
}
