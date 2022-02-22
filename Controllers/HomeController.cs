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
        private readonly ReviewDbContext db;
        UserManager<IdentityUser> _userManager;


        public HomeController(ILogger<HomeController> logger, SignInManager<IdentityUser> signInManager,
                              ReviewDbContext context, UserManager<IdentityUser> userManager)
        {
            _logger = logger;
            _signInManager = signInManager;
            db = context;
            _userManager = userManager;
        }

        public IActionResult Index()
        {
            return View(db.Reviews.ToList());
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(ReviewModel review)
        {
            review.UserId = User.FindFirst(User.Identity.Name).Value;
            db.Reviews.Add(review);
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}