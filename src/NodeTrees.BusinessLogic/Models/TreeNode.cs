using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NodeTrees.BusinessLogic.Models
{
    [Table("tree_nodes")]
    public sealed class TreeNode : BaseModel
    {
        [Required]
        [MaxLength(200)]
        [Column("name")]
        public string Name { get; set; } = string.Empty;

        [Required]
        [Column("tree_id")]
        public long TreeId { get; set; }

        [Column("parent_id")]
        public long? ParentId { get; set; }

        /// <summary>
        /// Depth of the node in the tree
        /// </summary>
        [Column("depth")]
        public int Depth { get; set; }

        /// <summary>
        /// Materialized path of the node (e.g., "12/34/56")
        /// </summary>
        [MaxLength(2048)]
        [Column("path")]
        public string? Path { get; set; }

        public Tree? Tree { get; set; }

        public TreeNode? Parent { get; set; }

        public ICollection<TreeNode> Children { get; set; } = new List<TreeNode>();

        [NotMapped]
        public bool IsRoot
        {
            get
            {
                return ParentId == null;
            }
        }
    }
}
