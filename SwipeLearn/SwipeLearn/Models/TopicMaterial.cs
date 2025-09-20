namespace SwipeLearn.Models
{
    public class TopicMaterial
    {
        public Guid Id { get; set; }

        public Guid TopicId { get; set; }

        public string? Description { get; set; }

        public List<string>? Images { get; set; }

        public List<string>? Voice { get; set; }

        // Navigation Property
        public Topic? Topic { get; set; }
    }
}
