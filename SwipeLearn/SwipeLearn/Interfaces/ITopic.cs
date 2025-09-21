using SwipeLearn.Models;
using SwipeLearn.Repositories;
using SwipeLearn.Utils;
using System.Linq.Expressions;

namespace SwipeLearn.Interfaces
{
    public interface ITopic : IRepository<Topic>
    {
        Task<ListPagination<Topic>> Get(
           Expression<Func<Topic, bool>> filter = null,
           Expression<Func<IQueryable<Topic>, IOrderedQueryable<Topic>>> orderBy = null,
           string includeProperties = "", PagingInfo pagingInfo = null);
        Task<Topic> GetById(Guid id);
        Task AddAsync(Topic entity);
        Task UpdateAsync(Topic entity);
        Task DeleteAsync(Guid id);
        Task<Topic> GetByDescription(string description);
    }
}
