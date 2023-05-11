using _1670_Source_Duy.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace _1670_Source_Duy.Controllers
{
    [Authorize(Roles = "Admin, StoreOwner")]
    public class ManagerController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public ManagerController(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager, ApplicationDbContext context)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _context = context;
        }
        public IActionResult Admin()
        {
            return View();
        }

        [Authorize(Roles = "StoreOwner")]
        public IActionResult StoreOwner()
        {
            return View();
        }

        public IActionResult Product()
        {
            return View();
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Account()
        {
                var users = await _userManager.Users.ToListAsync();
                var roles = await _roleManager.Roles.ToListAsync();

                var model = new List<object>();

                foreach (var user in users)
                {
                    var userRoles = await _userManager.GetRolesAsync(user);
                    var viewModel = new
                    {
                        UserId = user.Id,
                        Email = user.Email,
                        Roles = userRoles,
                        AllRoles = roles
                    };
                    model.Add(viewModel);
                }
                return View(model);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateRoles(string userId, string[] roles)
        {
            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
            {
                return NotFound();
            }

            if (user.Email == "admin@gmail.com")
            {
                TempData["message"] = "Cannot change role for admin@gmail.com because it's a default account!!";
                return RedirectToAction("Account");
            }

            var userRoles = await _userManager.GetRolesAsync(user);
            var result = await _userManager.RemoveFromRolesAsync(user, userRoles);


            if (!result.Succeeded)
            {
                ModelState.AddModelError("", "Failed to remove user roles");
                return BadRequest(ModelState);
            }

            result = await _userManager.AddToRolesAsync(user, roles);

            if (!result.Succeeded)
            {
                ModelState.AddModelError("", "Failed to add user roles");
                return BadRequest(ModelState);
            }

            return RedirectToAction("Account");
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }


            var user = await _userManager.FindByIdAsync(id);
            if (user.Email == "admin@gmail.com")
            {
                TempData["MessageDelete"] = "Cannot Delete admin account because it can make the product crash!";
                return RedirectToAction("Account");
            }

            if (user == null)
            {
                return NotFound();
            }

            var result = await _userManager.DeleteAsync(user);
            if (!result.Succeeded)
            {
                ModelState.AddModelError("", "Failed to delete user");
                return BadRequest(ModelState);
            }

            return RedirectToAction("Account");
        }
    }
}
