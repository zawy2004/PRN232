using System.ComponentModel.DataAnnotations;

namespace BusinessObjects
{
    public partial class SystemAccount
    {
        [Key]
        public int AccountId { get; set; }
        public string? EmailAddress { get; set; }
        public string? AccountPassword { get; set; }
        public int? Role { get; set; }
        public string? AccountNote { get; set; }
    }
}
