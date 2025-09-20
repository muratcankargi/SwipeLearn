using SwipeLearn.Models;
using SwipeLearn.Repositories;
using SwipeLearn.Utils;
using System.Linq.Expressions;

namespace SwipeLearn.Interfaces
{
    public interface ITopicMaterial : IRepository<TopicMaterial>
    {
        Task<ListPagination<TopicMaterial>> Get(
          Expression<Func<TopicMaterial, bool>> filter = null,
          Expression<Func<IQueryable<TopicMaterial>, IOrderedQueryable<TopicMaterial>>> orderBy = null,
          string includeProperties = "", PagingInfo pagingInfo = null);
        Task<TopicMaterial> GetById(Guid id);
        Task AddAsync(TopicMaterial entity);
        Task UpdateAsync(TopicMaterial entity);
        Task DeleteAsync(Guid id);
        Task<TopicMaterial> GetByTopicId(Guid guid);
    }
}
