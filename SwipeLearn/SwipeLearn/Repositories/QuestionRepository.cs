using Microsoft.EntityFrameworkCore;
using SwipeLearn.Context;
using SwipeLearn.Interfaces;
using SwipeLearn.Models;
using SwipeLearn.Utils;
using System.Linq.Expressions;

namespace SwipeLearn.Repositories
{
    public class QuestionRepository : IQuestion
    {
        private readonly ApplicationDbContext _context;

        public QuestionRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(Question entity)
        {
            var Question = await _context.Questions.AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var Question = await _context.Questions.FirstOrDefaultAsync(x => x.Id == id);
            if (Question != null)
            {
                _context.Questions.Remove(Question);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<ListPagination<Question>> Get(Expression<Func<Question, bool>> filter = null, Expression<Func<IQueryable<Question>, IOrderedQueryable<Question>>> orderBy = null, string includeProperties = "", PagingInfo pagingInfo = null)
        {
            var obj = new ListPagination<Question>();
            var query = _context.Set<Question>().AsQueryable();

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

        public async Task<Question> GetById(int id)
        {
            var Question = await _context.Questions.FirstOrDefaultAsync(x => x.Id == id);
            return Question;
        }

        public async Task UpdateAsync(Question entity)
        {
            _context.Questions.Update(entity);
            await _context.SaveChangesAsync();
        }

        public async Task<Question> GetByTopicId(Guid id)
        {
            var topic = await _context.Questions.FirstOrDefaultAsync(x => x.TopicId == id);
            return topic;
        }

        public async Task<List<Question>> GetQuestionsByTopicIdAsync(Guid topicId)
        {
            return await _context.Questions
                .Where(q => q.TopicId == topicId)
                .ToListAsync();
        }

        public async Task<List<Question>> GetQuestionsByTopicIdAsyncOrderById(Guid topicId)
        {
            return await _context.Questions
                .Where(q => q.TopicId == topicId)
                .OrderBy(q => q.Id)
                .ToListAsync();
        }

        public Task DeleteAsync(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task<Question> GetById(Guid id)
        {
            throw new NotImplementedException();
        }
    }
}
