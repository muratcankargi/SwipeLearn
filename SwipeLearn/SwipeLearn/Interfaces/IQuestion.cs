using SwipeLearn.Models;
using SwipeLearn.Repositories;
using SwipeLearn.Utils;
using System.Linq.Expressions;

namespace SwipeLearn.Interfaces
{
    public interface IQuestion : IRepository<Question>
    {
        Task<ListPagination<Question>> Get(
           Expression<Func<Question, bool>> filter = null,
           Expression<Func<IQueryable<Question>, IOrderedQueryable<Question>>> orderBy = null,
           string includeProperties = "", PagingInfo pagingInfo = null);
        Task<Question> GetById(int id);
        Task AddAsync(Question entity);
        Task UpdateAsync(Question entity);
        Task DeleteAsync(Guid id);
        Task<List<Question>> GetQuestionsByTopicIdAsync(Guid topicId);
        Task<List<Question>> GetQuestionsByTopicIdAsyncOrderById(Guid topicId);

    }
}
