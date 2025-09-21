using Microsoft.EntityFrameworkCore;
using SwipeLearn.Context;
using SwipeLearn.Interfaces;
using SwipeLearn.Models;
using SwipeLearn.Utils;
using System.Linq.Expressions;

namespace SwipeLearn.Repositories
{
    public class VideoRepository : IVideo
    {
        private readonly ApplicationDbContext _context;

        public VideoRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(Video entity)
        {
            var Video = await _context.Videos.AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var Video = await _context.Videos.FirstOrDefaultAsync(x => x.Id == id);
            if (Video != null)
            {
                _context.Videos.Remove(Video);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<ListPagination<Video>> Get(Expression<Func<Video, bool>> filter = null, Expression<Func<IQueryable<Video>, IOrderedQueryable<Video>>> orderBy = null, string includeProperties = "", PagingInfo pagingInfo = null)
        {
            var obj = new ListPagination<Video>();
            var query = _context.Set<Video>().AsQueryable();

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

        public async Task<Video> GetById(int id)
        {
            var Video = await _context.Videos.FirstOrDefaultAsync(x => x.Id == id);
            return Video;
        }

        public async Task UpdateAsync(Video entity)
        {
            _context.Videos.Update(entity);
            await _context.SaveChangesAsync();
        }

        public async Task<Video> GetByTopicId(Guid id)
        {
            var topic = await _context.Videos.FirstOrDefaultAsync(x => x.TopicId == id);
            return topic;
        }

        public async Task<(List<string> VideoPaths, List<string> Descriptions)> GetVideoPathsByTopicIdAsync(Guid topicId)
        {
            var videoPaths = await _context.Videos
                .Where(v => v.TopicId == topicId)
                .Select(v => v.VideoPath)
                .ToListAsync();

            var topicMaterial = await _context.TopicMaterials
                .Where(tm => tm.TopicId == topicId)
                .FirstOrDefaultAsync();

            var descriptions = topicMaterial?.Description ?? new List<string>();

            return (videoPaths, descriptions);
        }

        public Task DeleteAsync(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task<Video> GetById(Guid id)
        {
            throw new NotImplementedException();
        }
    }
}
