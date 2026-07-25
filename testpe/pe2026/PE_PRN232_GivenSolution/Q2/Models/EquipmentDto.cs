namespace Q2.Models
{
    public class EquipmentDto
    {
        public int EquipmentId { get; set; }
        public string Name { get; set; } = null!;
        public string Category { get; set; } = null!;
        public decimal PricePerDay { get; set; }
        public decimal DepositFee { get; set; }
        public int StockQuantity { get; set; }
        public string UploaderEmail { get; set; } = null!;
    }
}
