using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using project_manager.Models;
using project_manager.Services;

namespace project_manager.Controllers
{
    public class ProjectsController : Controller
    {
        private readonly IServiceProject _serviceProject;
        private readonly UserManager<ApplicationUser> _userManager;
        public ProjectsController(IServiceProject serviceProject, UserManager<ApplicationUser> userManager)
        {
            _serviceProject = serviceProject;
            _userManager = userManager;
        }
        //Get: /Projects/Index
        [Authorize]
        public async Task<IActionResult> Index()
        {

            var userId = _userManager.GetUserId(User);
            var projects = await _serviceProject.GetUserProjectsAsync(userId);
            return View(projects);
        }
        // GET: /Projects/Details/5
        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var project = await _serviceProject.GetByIdAsync(id);

            if (project == null || project.OwnerId != _userManager.GetUserId(User))
            {
                return NotFound();
            }

            return View(project);
        }
        //Get: /Projects/Create
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }
        //Post: /Projects/Create/
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Create([Bind("Name,Description")]Project project)
        {
            var userId = _userManager.GetUserId(User);

            ModelState.Remove("OwnerId");

            if (ModelState.IsValid)
            {
                await _serviceProject.CreateAsync(project, userId);
                return RedirectToAction(nameof(Index));
            }
            return View(project);
        }
        //Get: /Projects/Udpate
        [HttpGet]
        public async Task<IActionResult> Update(int id)
        {
            if(id == 0)
            {
                return NotFound();
            }
            try
            {
                var project = await _serviceProject.GetByIdAsync(id);
                return View(project);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
            
        }
        //Post: /Projects/Update/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Update(int id, [Bind("ProjectId,Name,Description")] Project project)
        {
            if (id != project.ProjectId) return NotFound();

            var existingProject = await _serviceProject.GetByIdAsync(id);
            if (existingProject == null || existingProject.OwnerId != _userManager.GetUserId(User))
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                project.OwnerId = existingProject.OwnerId;
                project.CreatedAt = existingProject.CreatedAt;

                await _serviceProject.UpdateAsync(id, project);
                return RedirectToAction(nameof(Index));
            }
            return View(project);
        }

        //Get: /Projects/Delete
        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            if (id == 0)
            {
                return NotFound();
            }

            try
            {
                var project = await _serviceProject.GetByIdAsync(id);
                return View(project);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }
        //Post: /Projects/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                await _serviceProject.DeleteAsync(id);
                return RedirectToAction(nameof(Index));
            }
            catch(KeyNotFoundException)
            {
                return NotFound();
            }
        }

    }
}
