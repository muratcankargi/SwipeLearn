using SwipeLearn.Models;
using SwipeLearn.Repositories;
using SwipeLearn.Utils;
using System.Linq.Expressions;

namespace SwipeLearn.Interfaces
{
    public interface IVideo : IRepository<Video>
    {
        Task<ListPagination<Video>> Get(
        Expression<Func<Video, bool>> filter = null,
        Expression<Func<IQueryable<Video>, IOrderedQueryable<Video>>> orderBy = null,
        string includeProperties = "", PagingInfo pagingInfo = null);
        Task<Video> GetById(Guid id);
        Task AddAsync(Video entity);
        Task UpdateAsync(Video entity);
        Task DeleteAsync(Guid id);
        Task<Video> GetByTopicId(Guid id);
        Task<(List<string> VideoPaths, List<string> Descriptions)> GetVideoPathsByTopicIdAsync(Guid topicId);

    }
}
