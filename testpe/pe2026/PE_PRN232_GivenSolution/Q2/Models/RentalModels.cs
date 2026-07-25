namespace Q2.Models
{
    public class CreateRentalDto
    {
        public int EquipmentId { get; set; }
        public int Quantity { get; set; } = 1;
        public DateTime StartDate { get; set; } = DateTime.Today;
        public DateTime EndDate { get; set; } = DateTime.Today.AddDays(1);
    }

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
