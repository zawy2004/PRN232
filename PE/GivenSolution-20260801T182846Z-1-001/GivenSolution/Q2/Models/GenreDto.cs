namespace CinemaMvcApp.Models
{
    // Maps to GET /api/genres response
    public class GenreDto
    {
        public int GenreID { get; set; }
        public string GenreName { get; set; } = string.Empty;
    }
}
