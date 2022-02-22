using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using CourseProject.Data;
using Microsoft.AspNetCore.Authorization;
using System.ComponentModel.DataAnnotations;

namespace CourseProject.Controllers
{
    public class AdminController : Controller
    {
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly CourseProject.Data.ApplicationDbContext db;
        UserManager<IdentityUser> _userManager;

        public AdminController(SignInManager<IdentityUser> signInManager,
                              CourseProject.Data.ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _signInManager = signInManager;
            db = context;
            _userManager = userManager;
        }
        public async Task<IActionResult> Index()
        {
            return View(db.Users.ToList());
        }

        public async Task<IActionResult> Delete(string id)
        {
            IdentityUser user = await _userManager.FindByIdAsync(id);
            IdentityResult result = await _userManager.DeleteAsync(user);
            return RedirectToAction("Index");
        }
    }
}
