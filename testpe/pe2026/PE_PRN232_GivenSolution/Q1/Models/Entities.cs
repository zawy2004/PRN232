namespace Q1.Models
{
    public class User
    {
        public int UserId { get; set; }
        public string Email { get; set; } = null!;
        public string Password { get; set; } = null!;
        public string Role { get; set; } = null!; // "admin" | "customer"
        public DateTime CreatedAt { get; set; }
    }

    public class Equipment
    {
        public int EquipmentId { get; set; }
        public string Name { get; set; } = null!;
        public string Category { get; set; } = null!;
        public decimal PricePerDay { get; set; }
        public decimal DepositFee { get; set; }
        public int StockQuantity { get; set; }
        public DateTime CreatedAt { get; set; }
        public int UserId { get; set; }
        public User User { get; set; } = null!;
    }

    public class Rental
    {
        public int RentalId { get; set; }
        public int EquipmentId { get; set; }
        public int UserId { get; set; }
        public int Quantity { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Status { get; set; } = "Active";
        public decimal FineAmount { get; set; }
        public DateTime RentalDate { get; set; }
        public Equipment Equipment { get; set; } = null!;
    }
}
