using SwipeLearn.Interfaces;
using SwipeLearn.Models;
using SwipeLearn.Repositories;

namespace SwipeLearn.Services
{
    public class TopicService
    {
        private readonly ITopic _repository;

        public TopicService(ITopic repository)
        {
            _repository = repository;
        }

        public async Task<Guid> Create(Topic topic)
        {
            if (topic == null || topic.Description == null) return Guid.Empty;

            var topicExist = await _repository.GetByDescription(topic.Description);
            if (topicExist != null) return Guid.Empty;

            topic.Id = Guid.NewGuid();
            await _repository.AddAsync(topic);
            return topic.Id;
        }
    }

}
