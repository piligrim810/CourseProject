using Microsoft.EntityFrameworkCore;
namespace CourseProject.Models
{
    public class ReviewDbContext : DbContext
    {
        public DbSet<ReviewModel> Reviews { get; set; }

        public ReviewDbContext(DbContextOptions<ReviewDbContext> options)
        : base(options)
        { }
    }
}
