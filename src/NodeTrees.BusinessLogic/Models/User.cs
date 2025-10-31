using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NodeTrees.BusinessLogic.Models
{
    using NodeTrees.Shared;

    [Table("user")]
    public class User : BaseModel
    {
        [Required]
        [MaxLength(200)]
        [Column("code")]
        public string Code { get; set; } = string.Empty;

        [Required]
        [Column("security_stamp")]
        public string SecurityStamp { get; set; } = Guid.NewGuid().ToString();

        [Required]
        [Column("role")]
        [MaxLength(20)]
        public UserRole Role { get; set; }
    }
}
