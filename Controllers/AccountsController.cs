using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using project_manager.Models;

namespace project_manager.Controllers
{
    public class AccountsController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        public AccountsController(
            UserManager<ApplicationUser> userManager, 
            SignInManager<ApplicationUser> signInManager, 
            RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
        }

        //Get: /Accounts/Register
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        //Post: /Accounts/Register
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register([Bind("UserName,Email,PasswordHash")] ApplicationUser newUser)
        {
            if (ModelState.IsValid)
            {
                newUser.EmailConfirmed = true;
                var result = await _userManager.CreateAsync(newUser, newUser.PasswordHash);
                if (result.Succeeded)
                {
                    return RedirectToAction("Index", "Projects");
                }
                else
                {
                    return BadRequest("Error");
                }
            }
            return View();
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login([Bind("UserName,PasswordHash")] ApplicationUser user)
        {
            if (ModelState.IsValid)
            {
                var result = await _signInManager.PasswordSignInAsync(user.UserName, user.PasswordHash, false, false);
                if (result.Succeeded)
                {
                    return RedirectToAction("Index", "Projects");
                }
                else
                {
                    return BadRequest("Error[01]");
                }
            }
            return BadRequest("Error[02]");
        }

        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Projects");
        }
    }
}
