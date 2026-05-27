using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DemoValidationRouting.Models
{
    public class Company
    {
        [Column("CompanyId")]
        public Guid Id { get; set; }

        [Required(ErrorMessage = "Company name is required field.")]
        [MaxLength(60, ErrorMessage = "Company name cannot be longer than 60 characters.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Company address is required field.")]
        [MaxLength(60, ErrorMessage = "Company address cannot be longer than 60 characters.")]
        public string Address { get; set; }

        public string Country { get; set; }
        public ICollection<Employee> Employees { get; set; }
    }
}
