using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BusinessObjects
{
    public class Product
    {
        [Key]
        public int ProductId { get; set; }

        [StringLength(40)]
        public string ProductName { get; set; } = null!;

        public int CategoryId { get; set; }

        public short? UnitsInStock { get; set; }

        [Column(TypeName = "money")]
        public decimal? UnitPrice { get; set; }

        public virtual Category? Category { get; set; }
    }
}
