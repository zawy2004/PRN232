namespace Q1.DTOs
{
    public class CombosListDto
    {
        public int ComboId { get; set; }

        public string? ComboName { get; set; }

        public decimal? Price { get; set; }
        public decimal? PriceWithTax {  get; set; }
    }
    public class CreateComboRequest
    {
        public string? ComboName { get; set; }
        public decimal? Price { get; set; }

    }
    public class filterMovieResponse
    {
        public int MovieId { get; set; }

        public string? Title { get; set; }

        public int? Duration { get; set; }

        public decimal? BasePrice { get; set; }
        public int? TotalQuantitySold { get; set; }
        public string? LastTicketSeat { get; set; }
        public List<string>? GenreList { get; set; }


    }
}
