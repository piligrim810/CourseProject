using CourseProject.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using Microsoft.AspNetCore.Identity;
using CourseProject.Data;

namespace CourseProject.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly ApplicationDbContext db;
        UserManager<IdentityUser> _userManager;


        public HomeController(ILogger<HomeController> logger, SignInManager<IdentityUser> signInManager,
                              ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _logger = logger;
            _signInManager = signInManager;
            db = context;
            _userManager = userManager;
        }

        public IActionResult Index()
        {
            List<ReviewModel> ListReviews = db.Reviews.ToList();
            ListReviews.Reverse();
            return View(ListReviews);
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

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}