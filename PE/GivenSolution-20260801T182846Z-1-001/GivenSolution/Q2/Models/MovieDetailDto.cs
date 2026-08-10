namespace CinemaMvcApp.Models
{
    // Nested inside MovieDetailDto.PurchasedCombos
    public class ComboAnalysisDto
    {
        public int ComboID { get; set; }
        public string ComboName { get; set; } = string.Empty;
        public decimal ComboPrice { get; set; }
    }

    // Maps to GET /api/movies/{movieId} response
    public class MovieDetailDto
    {
        public int MovieID { get; set; }
        public string Title { get; set; } = string.Empty;
        public decimal BasePrice { get; set; }
        public List<string> Genres { get; set; } = new();
        public List<ComboAnalysisDto> PurchasedCombos { get; set; } = new();
    }
}
