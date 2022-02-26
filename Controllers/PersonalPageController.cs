using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using CourseProject.Models;
using CourseProject.Data;

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
        public async Task<IActionResult> Index()
        {
            IdentityUser user = await _userManager.FindByNameAsync(User.Identity.Name);
            List<ReviewModel> ReviewsThisUser = db.Reviews
                .Where(c => c.UserId == user.Id)
                .ToList();
            return View(ReviewsThisUser);
        }
        public IActionResult CreateReview()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(ReviewModel review)
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
            ReviewModel review = db.Reviews.Single(s => s.Id == reviewId);
            return View(review);
        }
        public async Task<IActionResult> DeleteReview(int reviewId)
        {
            ReviewModel review = db.Reviews.Single(s => s.Id == reviewId);
            db.Reviews.Remove(review);
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
        }
        public async Task<IActionResult> EditReview(int reviewId)
        {
            ReviewModel review = db.Reviews.Single(s => s.Id == reviewId);
            return View(review);
        }
        [HttpPost]
        public async Task<IActionResult> SaveReview(ReviewModel review)
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
            List<ReviewModel> ReviewsThisUser = db.Reviews
                .Where(c => c.UserId == user.Id)
                .ToList();
            return View(ReviewsThisUser);
        }

    }
}
