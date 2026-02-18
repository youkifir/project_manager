using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using project_manager.Models.Entities;
using project_manager.Models.ViewModels;

namespace project_manager.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<ApplicationUser> _userManager;

        public AdminController(
            RoleManager<IdentityRole> roleManager,
            UserManager<ApplicationUser> userManager)
        {
            _roleManager = roleManager;
            _userManager = userManager;
        }

        // GET: /Admin/ListRoles
        public IActionResult ListRoles()
        {
            var roles = _roleManager.Roles.ToList();
            return View(roles);
        }

        // GET: /Admin/CreateRole
        public IActionResult CreateRole() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateRole(string roleName)
        {
            if (string.IsNullOrWhiteSpace(roleName))
            {
                ModelState.AddModelError("", "Role name cannot be empty.");
                return View();
            }

            if (!await _roleManager.RoleExistsAsync(roleName))
            {
                var result = await _roleManager.CreateAsync(new IdentityRole(roleName));
                if (result.Succeeded)
                {
                    return RedirectToAction(nameof(ListRoles));
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }
            else
            {
                ModelState.AddModelError("", "Role already exists.");
            }

            return View();
        }

        // GET: /Admin/UsersList
        public async Task<IActionResult> UsersList()
        {
            var users = await _userManager.Users.ToListAsync();
            var model = new List<UserRolesViewModel>();

            foreach (var user in users)
            {
                model.Add(new UserRolesViewModel
                {
                    User = user,
                    Roles = await _userManager.GetRolesAsync(user)
                });
            }

            return View(model);
        }

        // GET: /Admin/EditUserRoles/5
        [HttpGet]
        public async Task<IActionResult> EditUserRoles(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return NotFound();

            var userRoles = await _userManager.GetRolesAsync(user);

            var allRoles = await _roleManager.Roles
                .Where(r => r.Name != "Admin")
                .ToListAsync();

            ViewBag.AllRoles = allRoles;

            var model = new UserRolesViewModel
            {
                User = user,
                Roles = userRoles
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditUserRoles(string userId, List<string> selectedRoles)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return NotFound();

            var currentUserId = _userManager.GetUserId(User);
            if (user.Id == currentUserId && !selectedRoles.Contains("Admin"))
            {
                ModelState.AddModelError("", "You cannot remove the Admin role from yourself to prevent lockout.");
                ViewBag.AllRoles = await _roleManager.Roles.ToListAsync();
                return View(new UserRolesViewModel { User = user, Roles = await _userManager.GetRolesAsync(user) });
            }

            if (selectedRoles.Contains("Admin") && user.Id != currentUserId)
            {
                selectedRoles.Remove("Admin");
            }

            var currentRoles = await _userManager.GetRolesAsync(user);

            await _userManager.RemoveFromRolesAsync(user, currentRoles);
            if (selectedRoles != null && selectedRoles.Any())
            {
                await _userManager.AddToRolesAsync(user, selectedRoles);
            }

            return RedirectToAction(nameof(UsersList));
        }
    }
}