using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using CourseProject.Models;
using CourseProject.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;
using Azure.Storage.Blobs;
using Azure.Storage;
using Microsoft.Extensions.Options;

namespace CourseProject.Controllers
{
    public class PersonalPageController : Controller
    {
        private readonly AzureStorageConfig _azureStorageConfig;
        UserManager<IdentityUser> _userManager;
        private readonly IWebHostEnvironment env;
        private readonly ApplicationDbContext db;
        public PersonalPageController(UserManager<IdentityUser> userManager, ApplicationDbContext context, IOptions<AzureStorageConfig> config, IWebHostEnvironment appEnvironmewt) 
        {
            _userManager = userManager;
            db = context;
            _azureStorageConfig = config.Value;
            env = appEnvironmewt;
        }

        [Authorize]
        public async Task<IActionResult> Index(string Title, int Grade, SortState sortOrder = SortState.TitleAsc)
        {
            IdentityUser user = await _userManager.FindByNameAsync(User.Identity.Name);
            IQueryable<Review> ReviewsThisUser = db.Reviews
                .Where(c => c.UserId == user.Id);
            ReviewsThisUser = ReviewsThisUser.Include(u => u.Group);
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
            ViewData["GroupSort"] = sortOrder == SortState.GroupAsc ? SortState.GroupDesc : SortState.GroupAsc;
            ReviewsThisUser = sortOrder switch
            {
                SortState.TitleDesc => ReviewsThisUser.OrderByDescending(s => s.Title),
                SortState.GradeAsc => ReviewsThisUser.OrderBy(s => s.Grade),
                SortState.GradeDesc => ReviewsThisUser.OrderByDescending(s => s.Grade),
                SortState.GroupAsc => ReviewsThisUser.OrderBy(s => s.Group),
                SortState.GroupDesc => ReviewsThisUser.OrderByDescending(s => s.Group),
                _ => ReviewsThisUser.OrderBy(s => s.Title),
            };
            ViewBag.UserName = user.UserName;
            return View(ReviewsThisUser.AsNoTracking().ToList());
        }

        public IActionResult CreateReview()
        {
            SelectList Groups = new SelectList(db.Groups, "Id", "GroupName");
            Request request = new Request();
            ViewBag.Groups = Groups;
            return View();
        }
        public async Task<IActionResult> CreateReviewAdmin(string UserName)
        {
            IdentityUser user = await _userManager.FindByNameAsync(UserName);
            SelectList Groups = new SelectList(db.Groups, "Id", "GroupName");
            Request request = new Request();
            ViewBag.UserName = UserName;
            ViewBag.Groups = Groups;
            return View("CreateReview");
        }

        [HttpPost]
        public async Task<IActionResult> Create(Request request)
        {
            if (request.Review.UserName == null)
            {
                IdentityUser user = await _userManager.FindByNameAsync(User.Identity.Name);
                request.Review.UserName = user.UserName;
                request.Review.UserId = user.Id;
            }
            else
            {
                IdentityUser user = await _userManager.FindByNameAsync(request.Review.UserName);
                request.Review.UserName = user.UserName;
                request.Review.UserId = user.Id;
            }

            db.Reviews.Add(request.Review);
            await db.SaveChangesAsync();

            if (request.Image != null)
            {
                string FileName = request.Image.FileName;
                List<string> ImagesNames = new List<string>();
                var Lol = db.Images.ToList();
                foreach(var image in Lol)
                {
                    ImagesNames.Add(image.Name);
                }
                while(ImagesNames.Contains(FileName) == true) 
                {
                    foreach(var x in Lol)
                    {
                        if(FileName == x.Name)
                        {
                            FileName = '1' + FileName;
                        }
                    }
                }
                Uri blobUri = new Uri("https://" +
                     _azureStorageConfig.AccountName +
                     ".blob.core.windows.net/" +
                     _azureStorageConfig.ContainerName +
                     "/" + FileName);

                StorageSharedKeyCredential storageCredentials =
                    new StorageSharedKeyCredential(_azureStorageConfig.AccountName, _azureStorageConfig.AccountKey);

                BlobClient blobClient = new BlobClient(blobUri, storageCredentials);

                if (request.Image != null)
                {
                    using (var fileStream = request.Image.OpenReadStream())
                    {
                        await blobClient.UploadAsync(fileStream);
                    }
                    Data.Image file = new Data.Image { Name = FileName, Review = request.Review};
                    db.Images.Add(file);
                    db.SaveChanges();
                }
            }
            return RedirectToAction("Index");
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
        public async Task<IActionResult> DeleteReview(int reviewId)
        {
            Review review = db.Reviews.Single(s => s.Id == reviewId);
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
                await blobClient.DeleteAsync();

            }
            db.Reviews.Remove(review);
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
        }
        public async Task<IActionResult> EditReview(int reviewId)
        {
            SelectList Groups = new SelectList(db.Groups, "Id", "GroupName");
            ViewBag.Groups = Groups;
            Review review = db.Reviews.Single(s => s.Id == reviewId);
            return View(review);
        }
        [HttpPost]
        public async Task<IActionResult> SaveReview(Review review)
        {
            IdentityUser user = await _userManager.FindByNameAsync(review.UserName);
            review.UserId = user.Id;
            review.UserName = user.UserName;
            db.Reviews.Update(review);
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
        }
        public async Task<IActionResult> IndexAdmin(string id)
        {
            IdentityUser user = await _userManager.FindByIdAsync(id);
            ViewBag.UserName = user.UserName;
            IQueryable<Review> reviews = db.Reviews
                .Where(c => c.UserId == user.Id);
            List<Review> ReviewsThisUser = reviews.Include(u => u.Group).ToList();
            return View(ReviewsThisUser);
        }
    }
}
