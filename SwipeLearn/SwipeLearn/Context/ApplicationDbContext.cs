using Microsoft.EntityFrameworkCore;
using SwipeLearn.Models;

namespace SwipeLearn.Context
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<Topic> Topics { get; set; }
        public DbSet<Video> Videos { get; set; }
        public DbSet<Question> Questions { get; set; }
    }
}
