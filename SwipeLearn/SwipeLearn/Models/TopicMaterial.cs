using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SwipeLearn.Models
{
    [Table("topic_material")]
    public class TopicMaterial
    {
        [Key]
        [Column("id")]
        public Guid Id { get; set; }

        [Column("topic_id")]
        public Guid TopicId { get; set; }

        [Column("description")]
        public string? Description { get; set; }

        [Column("images")]
        public List<string>? Images { get; set; }

        [Column("voice")]
        public List<string>? Voice { get; set; }

        // Navigation Property
        public Topic? Topic { get; set; }
    }
}
