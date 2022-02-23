using CourseProject.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using Microsoft.AspNetCore.Identity;

namespace CourseProject.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly ReviewDbContext dbReviews;
        UserManager<IdentityUser> _userManager;


        public HomeController(ILogger<HomeController> logger, SignInManager<IdentityUser> signInManager,
                              ReviewDbContext contextReviews, UserManager<IdentityUser> userManager)
        {
            _logger = logger;
            _signInManager = signInManager;
            dbReviews = contextReviews;
            _userManager = userManager;
        }

        public IActionResult Index()
        {
            return View(dbReviews.Reviews.ToList());
        }

        public IActionResult Privacy()
        {
            return View();
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
            dbReviews.Reviews.Add(review);
            await dbReviews.SaveChangesAsync();
            return RedirectToAction("Index");
        }
        [HttpPost]
        public async Task<IActionResult> ReadReview(int reviewId)
        {
            ReviewModel review = dbReviews.Reviews.Single(s => s.Id == reviewId);
            return View(review);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}