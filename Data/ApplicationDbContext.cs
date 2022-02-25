using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using CourseProject.Models;

namespace CourseProject.Data
{ 
    public class ApplicationDbContext : IdentityDbContext
    {
        public DbSet<ReviewModel> Reviews { get; set; }
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
    }
}