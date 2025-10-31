using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NodeTrees.BusinessLogic.Models
{
    using NodeTrees.Shared;

    public abstract class BaseModel
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("id")]
        public long Id { get; init; }

        [Column("is_deleted")]
        public bool IsDeleted { get; set; }

        [Column("deleted_at")]
        public DateTime? DeletedAt { get; set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; set; }

        [Column("updated_at")]
        public DateTime UpdatedAt { get; set; }

        public void MarkCreated()
        {
            if (CreatedAt != default(DateTime))
            {
                throw new ConflictSecureException("Entity is already marked as created.");
            }

            CreatedAt = DateTime.UtcNow;
        }

        public void MarkUpdated()
        {
            UpdatedAt = DateTime.UtcNow;
        }

        public void Delete()
        {
            if (IsDeleted)
            {
                throw new ConflictSecureException("Entity is already deleted.");
            }

            IsDeleted = true;
            DeletedAt = DateTime.UtcNow;
        }
    }
}
