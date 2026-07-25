namespace Q1.DTOs
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

    public record CreateEquipmentDto(string Name, string Category, decimal PricePerDay, decimal DepositFee, int StockQuantity);
}
