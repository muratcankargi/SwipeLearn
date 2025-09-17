using SwipeLearn.Utils;
using System.Linq.Expressions;

namespace SwipeLearn.Repositories
{
    public interface IRepository<TEntity> where TEntity : class
    {
        Task<ListPagination<TEntity>> Get(
            Expression<Func<TEntity, bool>> filter = null,
            Expression<Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>> orderBy = null,
            string includeProperties = "", PagingInfo pagingInfo = null);
        Task<TEntity> GetById(int id);
        Task AddAsync(TEntity entity);
        Task UpdateAsync(TEntity entity);
        Task DeleteAsync(int id);
    }
}
