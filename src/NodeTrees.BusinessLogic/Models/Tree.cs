using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NodeTrees.BusinessLogic.Models
{
    [Table("trees")]
    public sealed class Tree : BaseModel
    {
        [Required]
        [MaxLength(200)]
        [Column("name")]
        public string Name { get; set; } = string.Empty;

        [Required]
        [Column("user_id")]
        public long UserId { get; set; }

        /// <summary>
        /// Unique key for the tree
        /// </summary>
        [Required]
        [Column("tree_key")]
        public Guid TreeKey { get; set; } = Guid.NewGuid();

        public User? User { get; set; }

        public ICollection<TreeNode> Nodes { get; set; } = new List<TreeNode>();
    }
}
