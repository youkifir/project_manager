using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using project_manager.Models.Entities;
using project_manager.Services;

namespace project_manager.Controllers
{
    [Authorize]
    public class TasksController : Controller
    {
        private readonly IServiceTask _serviceTask;
        private readonly IServiceProject _serviceProject;
        private readonly UserManager<ApplicationUser> _userManager;

        public TasksController(IServiceTask serviceTask, IServiceProject serviceProject, UserManager<ApplicationUser> userManager)
        {
            _serviceTask = serviceTask;
            _serviceProject = serviceProject;
            _userManager = userManager;
        }

        private string CurrentUserId => _userManager.GetUserId(User);

        // GET: /Tasks/Index?projectId=5
        public async Task<IActionResult> Index(int projectId, string sortOrder)
        {
            var tasks = await _serviceTask.GetTasksByProjectAsync(projectId, CurrentUserId, sortOrder);

            var project = await _serviceProject.GetByIdAsync(projectId, CurrentUserId);
            if (project == null) return NotFound();

            ViewBag.ProjectName = project.Name;
            ViewBag.ProjectId = projectId;
            ViewBag.CurrentSort = sortOrder;

            return View(tasks);
        }

        // GET: /Tasks/Create?projectId=5
        public async Task<IActionResult> Create(int projectId)
        {
            var project = await _serviceProject.GetByIdAsync(projectId, CurrentUserId);
            if (project == null) return NotFound();

            var users = await _userManager.Users.ToListAsync();
            ViewBag.Users = new SelectList(users, "Id", "UserName");
            ViewBag.ProjectId = projectId;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProjectTask task)
        {
            ModelState.Remove("Project");
            ModelState.Remove("AssignedUser");

            if (ModelState.IsValid)
            {
                var result = await _serviceTask.CreateAsync(task, CurrentUserId);
                if (result != null)
                    return RedirectToAction(nameof(Index), new { projectId = task.ProjectId });

                ModelState.AddModelError("", "Ошибка доступа к проекту.");
            }

            var users = await _userManager.Users.ToListAsync();
            ViewBag.Users = new SelectList(users, "Id", "UserName", task.AssignedUserId);
            ViewBag.ProjectId = task.ProjectId;
            return View(task);
        }

        // GET: /Tasks/Update/5
        public async Task<IActionResult> Update(int id)
        {
            var task = await _serviceTask.GetByIdAsync(id, CurrentUserId);
            if (task == null) return NotFound();

            var users = await _userManager.Users.ToListAsync();
            ViewBag.Users = new SelectList(users, "Id", "UserName", task.AssignedUserId);
            return View(task);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(int id, ProjectTask task)
        {
            if (id != task.TaskId) return NotFound();

            ModelState.Remove("Project");
            ModelState.Remove("AssignedUser");

            if (ModelState.IsValid)
            {
                var result = await _serviceTask.UpdateAsync(id, task, CurrentUserId);
                if (result != null)
                    return RedirectToAction(nameof(Index), new { projectId = task.ProjectId });
            }

            var users = await _userManager.Users.ToListAsync();
            ViewBag.Users = new SelectList(users, "Id", "UserName", task.AssignedUserId);
            return View(task);
        }

        // GET: /Tasks/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var task = await _serviceTask.GetByIdAsync(id, CurrentUserId);
            if (task == null) return NotFound();

            return View(task);
        }

        // GET: /Tasks/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var task = await _serviceTask.GetByIdAsync(id, CurrentUserId);
            if (task == null) return NotFound();

            return View(task);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var task = await _serviceTask.GetByIdAsync(id, CurrentUserId);
            if (task == null) return NotFound();

            int projectId = task.ProjectId;
            await _serviceTask.DeleteAsync(id, CurrentUserId);

            return RedirectToAction(nameof(Index), new { projectId = projectId });
        }
    }
}