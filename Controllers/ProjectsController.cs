using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using project_manager.Models.DTOs;
using project_manager.Models.Entities;
using project_manager.Services;

namespace project_manager.Controllers
{
    [Authorize]
    public class ProjectsController : Controller
    {
        private readonly IServiceProject _serviceProject;
        private readonly UserManager<ApplicationUser> _userManager;

        public ProjectsController(IServiceProject serviceProject, UserManager<ApplicationUser> userManager)
        {
            _serviceProject = serviceProject;
            _userManager = userManager;
        }

        private string CurrentUserId => _userManager.GetUserId(User);

        public async Task<IActionResult> Index(string searchTerm)
        {
            var projects = await _serviceProject.GetUserProjectsAsync(CurrentUserId, searchTerm);

            ViewBag.CurrentFilter = searchTerm;
            return View(projects);
        }

        public async Task<IActionResult> Details(int id)
        {
            var project = await _serviceProject.GetByIdAsync(id, CurrentUserId);

            if (project == null) return NotFound();

            return View(project);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name,Description")] Project project)
        {
            ModelState.Remove("OwnerId");

            if (ModelState.IsValid)
            {
                await _serviceProject.CreateAsync(project, CurrentUserId);
                return RedirectToAction(nameof(Index));
            }
            return View(project);
        }

        public async Task<IActionResult> Update(int id)
        {
            var project = await _serviceProject.GetByIdAsync(id, CurrentUserId);

            if (project == null) return NotFound();

            return View(project);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(int id, ProjectDto model)
        {
            if (ModelState.IsValid)
            {
                var result = await _serviceProject.UpdateAsync(id, model, CurrentUserId);

                if (result == null) return NotFound();

                return RedirectToAction(nameof(Index));
            }
            return View(model);
        }

        public async Task<IActionResult> Delete(int id)
        {
            var project = await _serviceProject.GetByIdAsync(id, CurrentUserId);

            if (project == null) return NotFound();

            return View(project);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var result = await _serviceProject.DeleteAsync(id, CurrentUserId);

            if (result == null) return NotFound();

            return RedirectToAction(nameof(Index));
        }
    }
}