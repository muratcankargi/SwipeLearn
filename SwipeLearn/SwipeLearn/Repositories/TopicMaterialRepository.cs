using Microsoft.EntityFrameworkCore;
using SwipeLearn.Context;
using SwipeLearn.Interfaces;
using SwipeLearn.Models;
using SwipeLearn.Utils;
using System.Linq.Expressions;

namespace SwipeLearn.Repositories
{
    public class TopicMaterialRepository : ITopicMaterial
    {
        private readonly ApplicationDbContext _context;

        public TopicMaterialRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(TopicMaterial entity)
        {
            var topic = await _context.TopicMaterials.AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            var topic = await _context.TopicMaterials.FirstOrDefaultAsync(x => x.Id == id);
            if (topic != null)
            {
                _context.TopicMaterials.Remove(topic);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<ListPagination<TopicMaterial>> Get(Expression<Func<TopicMaterial, bool>> filter = null, Expression<Func<IQueryable<TopicMaterial>, IOrderedQueryable<TopicMaterial>>> orderBy = null, string includeProperties = "", PagingInfo pagingInfo = null)
        {
            var obj = new ListPagination<TopicMaterial>();
            var query = _context.Set<TopicMaterial>().AsQueryable();

            if (filter != null)
            {
                query = query.Where(filter);
            }

            if (pagingInfo != null)
            {
                pagingInfo.total = query.Count();
                if (pagingInfo.page != null && pagingInfo.page > 0 && pagingInfo.size > 0 && pagingInfo.total > 0)
                {
                    int maxPageIndex = Convert.ToInt32(Math.Ceiling((double)pagingInfo.total / pagingInfo.size));
                    pagingInfo.max = maxPageIndex;
                    pagingInfo.page = pagingInfo.page > maxPageIndex ? maxPageIndex : pagingInfo.page;
                }
            }

            if (pagingInfo != null && pagingInfo.size > 0 && pagingInfo.page != null)
            {
                query = query.OrderByDescending(x => x.Id)
                             .Skip(pagingInfo.size * (pagingInfo.page - 1))
                             .Take(pagingInfo.size);
            }

            var orders = await query.AsNoTracking().ToListAsync();

            obj.list = orders;
            obj.pageInfo = pagingInfo;

            return obj;
        }

        public async Task<TopicMaterial> GetById(Guid id)
        {
            var topic = await _context.TopicMaterials.FirstOrDefaultAsync(x => x.Id == id);
            return topic;
        }

        public async Task UpdateAsync(TopicMaterial entity)
        {
            _context.TopicMaterials.Update(entity);
            await _context.SaveChangesAsync();
        }
        public async Task<TopicMaterial> GetByTopicId(Guid id)
        {
            var topic = await _context.TopicMaterials.FirstOrDefaultAsync(x => x.TopicId == id);
            return topic;
        }
        public Task<TopicMaterial> GetById(int id)
        {
            throw new NotImplementedException();
        }

        public Task DeleteAsync(int id)
        {
            throw new NotImplementedException();
        }
    }
}
