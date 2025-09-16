using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace SwipeLearn.Models
{
    [Table("topics")]
    public class Topic
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Required]
        [Column("description")]
        [MaxLength(255)]
        public string Description { get; set; } = string.Empty;

        // Navigation Properties
        public ICollection<Video> Videos { get; set; } = new List<Video>();
        public ICollection<Question> Questions { get; set; } = new List<Question>();
    }
}
