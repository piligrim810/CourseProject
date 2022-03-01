using CourseProject.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using Microsoft.AspNetCore.Identity;
using CourseProject.Data;
using Microsoft.EntityFrameworkCore;

namespace CourseProject.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly ApplicationDbContext db;
        private readonly UserManager<IdentityUser> _userManager;

        public HomeController(ILogger<HomeController> logger, SignInManager<IdentityUser> signInManager,
                              ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _logger = logger;
            _signInManager = signInManager;
            db = context;
            _userManager = userManager;
        }
        public IActionResult Index(string Title,int Grade,SortState sortOrder = SortState.TitleAsc)
        {
            IQueryable<Review> Reviews = db.Reviews;
            if (!String.IsNullOrEmpty(Title))
            {
                Reviews = Reviews.Where(p => p.Title.Contains(Title));
            }
            if (Grade != null && Grade != 0)
            {
                Reviews = Reviews.Where(p => p.Grade == Grade );
            }

            ViewData["TitleSort"] = sortOrder == SortState.TitleAsc ? SortState.TitleDesc : SortState.TitleAsc;
            ViewData["GradeSort"] = sortOrder == SortState.GradeAsc ? SortState.GradeDesc : SortState.GradeAsc;
            Reviews = sortOrder switch
            {
                SortState.TitleDesc => Reviews.OrderByDescending(s => s.Title),
                SortState.GradeAsc => Reviews.OrderBy(s => s.Grade),
                SortState.GradeDesc => Reviews.OrderByDescending(s => s.Grade),
                _ => Reviews.OrderBy(s => s.Title),
            };
            return View(Reviews.AsNoTracking().ToList());
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ReadReview(int reviewId)
        {
            Review review = db.Reviews.Single(s => s.Id == reviewId);
            return View(review);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}