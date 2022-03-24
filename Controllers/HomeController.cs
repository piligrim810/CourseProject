using CourseProject.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using Microsoft.AspNetCore.Identity;
using CourseProject.Data;
using Microsoft.EntityFrameworkCore;
using Azure.Storage.Blobs;
using Azure.Storage;
using Microsoft.Extensions.Options;

namespace CourseProject.Controllers
{
    public class HomeController : Controller
    {
        private readonly AzureStorageConfig _azureStorageConfig;
        private readonly ILogger<HomeController> _logger;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly ApplicationDbContext db;
        private readonly UserManager<IdentityUser> _userManager;

        public HomeController(ILogger<HomeController> logger, SignInManager<IdentityUser> signInManager,
                              ApplicationDbContext context, UserManager<IdentityUser> userManager, IOptions<AzureStorageConfig> config)
        {
            _logger = logger;
            _signInManager = signInManager;
            db = context;
            _userManager = userManager;
            _azureStorageConfig = config.Value;
        }
        public IActionResult Index(string Title,int Grade,SortState sortOrder = SortState.TitleAsc)
        {
            IQueryable<Review> Reviews = db.Reviews.Include(u => u.Group);
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
            ViewData["GroupSort"] = sortOrder == SortState.GroupAsc ? SortState.GroupDesc : SortState.GroupAsc;
            Reviews = sortOrder switch
            {
                SortState.TitleDesc => Reviews.OrderByDescending(s => s.Title),
                SortState.GradeAsc => Reviews.OrderBy(s => s.Grade),
                SortState.GradeDesc => Reviews.OrderByDescending(s => s.Grade),
                SortState.GroupAsc => Reviews.OrderBy(s => s.Group),
                SortState.GroupDesc => Reviews.OrderByDescending(s => s.Group),
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
            IQueryable<Review> Reviews = db.Reviews.Include(u => u.Group);
            Review review = Reviews.Single(s => s.Id == reviewId);
            review.Images = db.Images.Where(s => s.Review == review).ToList();

            if (review.Images.Count > 0)
            {
                Uri blobUri = new Uri("https://" +
                         _azureStorageConfig.AccountName +
                         ".blob.core.windows.net/" +
                         _azureStorageConfig.ContainerName +
                         "/" + review.Images.First().Name);

                StorageSharedKeyCredential storageCredentials =
                    new StorageSharedKeyCredential(_azureStorageConfig.AccountName, _azureStorageConfig.AccountKey);

                BlobClient blobClient = new BlobClient(blobUri, storageCredentials);
                var memoryStream = new MemoryStream();
                await blobClient.DownloadToAsync(memoryStream);

                ViewBag.imgSrc = String.Format("data:image/png;base64,{0}", Convert.ToBase64String(memoryStream.ToArray()));
                memoryStream.Close();
            }
            return View(review);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}