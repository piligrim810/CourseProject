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
