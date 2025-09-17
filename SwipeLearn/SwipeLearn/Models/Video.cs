using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace SwipeLearn.Models
{
    [Table("videos")]
    public class Video
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Required]
        [ForeignKey(nameof(Topic))]
        [Column("topic_id")]
        public Guid TopicId { get; set; }

        [Required]
        [Column("video_path")]
        public string VideoPath { get; set; } = string.Empty;

        // Navigation Property
        public Topic? Topic { get; set; }
    }
}
