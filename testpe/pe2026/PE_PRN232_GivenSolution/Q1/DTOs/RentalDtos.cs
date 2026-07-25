namespace Q1.DTOs
{
    public record CreateRentalDto(int EquipmentId, int Quantity, DateTime StartDate, DateTime EndDate);

    public class RentalDto
    {
        public int RentalId { get; set; }
        public int EquipmentId { get; set; }
        public string EquipmentName { get; set; } = null!;
        public int UserId { get; set; }
        public int Quantity { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Status { get; set; } = null!;
        public decimal FineAmount { get; set; }
        public DateTime RentalDate { get; set; }
    }
}
