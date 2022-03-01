using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using CourseProject.Models;

namespace CourseProject.Data
{
    public class Review
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public string UserName { get; set; }
        public int GroupId { get; set; }
        public Group Group { get; set; }
        public string Title { get; set; }
        public string Text { get; set; }
        public int Grade { get; set; }
    }
    public class Group
    {
        public int Id { get; set; }
        public string GroupName { get; set; }

        public List<Review> Reviews { get; set; }
    }
    [Keyless]
    public class TagReview
    {
        public int ReviewId { get; set; }
        public int TagId { get; set; }
        public Review Review { get; set; }
        public Tag Tag { get; set; }
    }
    public class Tag
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
    public class Image
    {
        public int Id { get; set; }
        public string Name { set; get; }
        public Review Review { get; set; }
    }

    public class ApplicationDbContext : IdentityDbContext
    {
        public DbSet<Review> Reviews { get; set; }
        public DbSet<Group> Groups { get; set; }
        public DbSet<Tag> Tags { get; set; }
        public DbSet<TagReview> TagReviews { get; set; }
        public DbSet<Image> Images { get; set; }
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
    }
}