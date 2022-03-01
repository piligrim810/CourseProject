using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using CourseProject.Models;
using CourseProject.Data;
using Microsoft.EntityFrameworkCore;

namespace CourseProject.Controllers
{
    public class PersonalPageController : Controller
    {
        UserManager<IdentityUser> _userManager;
        private readonly ApplicationDbContext db;
        public PersonalPageController(UserManager<IdentityUser> userManager, ApplicationDbContext context) 
        {
            _userManager = userManager;
            db = context;
        }

        [Authorize]
        public async Task<IActionResult> Index(string Title, int Grade, SortState sortOrder = SortState.TitleAsc)
        {
            IdentityUser user = await _userManager.FindByNameAsync(User.Identity.Name);
            IQueryable<Review> ReviewsThisUser = db.Reviews
                .Where(c => c.UserId == user.Id);
            if (!String.IsNullOrEmpty(Title))
            {
                ReviewsThisUser = ReviewsThisUser.Where(p => p.Title.Contains(Title));
            }
            if (Grade != null && Grade != 0)
            {
                ReviewsThisUser = ReviewsThisUser.Where(p => p.Grade == Grade);
            }

            ViewData["TitleSort"] = sortOrder == SortState.TitleAsc ? SortState.TitleDesc : SortState.TitleAsc;
            ViewData["GradeSort"] = sortOrder == SortState.GradeAsc ? SortState.GradeDesc : SortState.GradeAsc;
            ReviewsThisUser = sortOrder switch
            {
                SortState.TitleDesc => ReviewsThisUser.OrderByDescending(s => s.Title),
                SortState.GradeAsc => ReviewsThisUser.OrderBy(s => s.Grade),
                SortState.GradeDesc => ReviewsThisUser.OrderByDescending(s => s.Grade),
                _ => ReviewsThisUser.OrderBy(s => s.Title),
            };
            return View(ReviewsThisUser.AsNoTracking().ToList());
        }
        public IActionResult CreateReview()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Review review)
        {
            IdentityUser user = await _userManager.FindByNameAsync(User.Identity.Name);
            review.UserId = user.Id;
            db.Reviews.Add(review);
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> ReadReview(int reviewId)
        {
            Review review = db.Reviews.Single(s => s.Id == reviewId);
            return View(review);
        }
        public async Task<IActionResult> DeleteReview(int reviewId)
        {
            Review review = db.Reviews.Single(s => s.Id == reviewId);
            db.Reviews.Remove(review);
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
        }
        public async Task<IActionResult> EditReview(int reviewId)
        {
            Review review = db.Reviews.Single(s => s.Id == reviewId);
            return View(review);
        }
        [HttpPost]
        public async Task<IActionResult> SaveReview(Review review)
        {
            IdentityUser user = await _userManager.FindByNameAsync(User.Identity.Name);
            review.UserId = user.Id;
            db.Reviews.Update(review);
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
        }
        public async Task<IActionResult> IndexAdmin(string id)
        {
            IdentityUser user = await _userManager.FindByIdAsync(id);
            List<Review> ReviewsThisUser = db.Reviews
                .Where(c => c.UserId == user.Id)
                .ToList();
            return View(ReviewsThisUser);
        }

    }
}
