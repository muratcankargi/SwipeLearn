using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace SwipeLearn.Models
{
    [Table("questions")]
    public class Question
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Required]
        [ForeignKey(nameof(Topic))]
        [Column("topic_id")]
        public Guid TopicId { get; set; }

        [Column("question")]
        public string? QuestionText { get; set; }

        [Required]
        [Column("answers")]
        public string[] Answers { get; set; } = [];

        [Required]
        [Column("correct")]
        public string Correct { get; set; } = string.Empty;

        // Navigation Property
        public Topic Topic { get; set; }
    }
}
