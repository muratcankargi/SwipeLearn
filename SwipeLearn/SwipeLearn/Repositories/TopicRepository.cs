using Microsoft.EntityFrameworkCore;
using SwipeLearn.Context;
using SwipeLearn.Interfaces;
using SwipeLearn.Models;
using SwipeLearn.Utils;
using System.Linq.Expressions;

namespace SwipeLearn.Repositories
{
    public class TopicRepository : ITopic
    {
        private readonly ApplicationDbContext _context;

        public TopicRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(Topic entity)
        {
            var topic = await _context.Topics.AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            var topic = await _context.Topics.FirstOrDefaultAsync(x => x.Id == id);
            if (topic != null)
            {
                _context.Topics.Remove(topic);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<ListPagination<Topic>> Get(Expression<Func<Topic, bool>> filter = null, Expression<Func<IQueryable<Topic>, IOrderedQueryable<Topic>>> orderBy = null, string includeProperties = "", PagingInfo pagingInfo = null)
        {
            var obj = new ListPagination<Topic>();
            var query = _context.Set<Topic>().AsQueryable();

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

        public async Task<Topic> GetById(Guid id)
        {
            var topic = await _context.Topics.FirstOrDefaultAsync(x => x.Id == id);
            return topic;
        }

        public async Task UpdateAsync(Topic entity)
        {
            _context.Topics.Update(entity);
            await _context.SaveChangesAsync();
        }

        public async Task<Topic> GetByDescription(string description)
        {
            var topic = await _context.Topics.FirstOrDefaultAsync(x => x.Description == description);
            return topic;
        }

        public Task<Topic> GetById(int id)
        {
            throw new NotImplementedException();
        }

        public Task DeleteAsync(int id)
        {
            throw new NotImplementedException();
        }
    }
}
