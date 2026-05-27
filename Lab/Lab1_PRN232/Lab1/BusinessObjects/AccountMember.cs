using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BusinessObjects
{
    public class AccountMember
    {
        [Key]
        [StringLength(20)]
        public string MemberId { get; set; } = null!;

        [StringLength(80)]
        public string MemberPassword { get; set; } = null!;

        [StringLength(80)]
        public string FullName { get; set; } = null!;

        [StringLength(100)]
        public string EmailAddress { get; set; } = null!;

        public int MemberRole { get; set; }
    }
}
