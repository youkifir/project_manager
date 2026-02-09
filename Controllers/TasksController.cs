using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using project_manager.Models;
using project_manager.Services;

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

    [HttpGet]
    public async Task<IActionResult> Index(int projectId)
    {
        var tasks = await _serviceTask.GetByProjectIdAsync(projectId);

        var project = await _serviceProject.GetByIdAsync(projectId);
        if (project == null) return NotFound();

        ViewBag.ProjectName = project.Name;
        ViewBag.ProjectId = projectId;

        return View(tasks);
    }

    [HttpGet]
    public async Task<IActionResult> Create(int projectId)
    {
        var users = await _userManager.Users.ToListAsync();

        ViewBag.Users = new SelectList(users, "Id", "UserName");
        ViewBag.ProjectId = projectId;
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Title,Description,DueDate,ProjectId,AssignedUserId")] project_manager.Models.Task task)
    {
        if (ModelState.IsValid)
        {
            task.IsCompleted = false;
            await _serviceTask.CreateAsync(task);

            return RedirectToAction(nameof(Index), new { projectId = task.ProjectId });
        }

        var users = await _userManager.Users.ToListAsync();
        ViewBag.Users = new SelectList(users, "Id", "UserName");
        return View(task);
    }

    [HttpGet]
    public async Task<IActionResult> Update(int? id)
    {
        if (id == null || id == 0) return NotFound();

        var task = await _serviceTask.GetByIdAsync(id.Value);

        if (task == null) return NotFound();

        var users = await _userManager.Users.ToListAsync();
        ViewBag.Users = new SelectList(users, "Id", "UserName", task.AssignedUserId);
        return View(task);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Update(int id, [Bind("TaskId,Title,Description,DueDate,IsCompleted,ProjectId,AssignedUserId")] project_manager.Models.Task task)
    {
        if (id != task.TaskId) return NotFound();

        ModelState.Remove("Project");
        ModelState.Remove("AssignedUser");

        if (ModelState.IsValid)
        {
            await _serviceTask.UpdateAsync(id, task);
            return RedirectToAction(nameof(Index), new { projectId = task.ProjectId });
        }

        var users = _userManager.Users.ToList();
        ViewBag.Users = new SelectList(users, "Id", "UserName", task.AssignedUserId);
        return View(task);
    }

    [HttpGet]
    public async Task<IActionResult> Details(int id)
    {
        if (id <= 0)
        {
            return NotFound();
        }

        var task = await _serviceTask.GetByIdAsync(id);

        if (task == null)
        {
            return NotFound();
        }

        return View(task);
    }

    [HttpGet]
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null) return NotFound();

        var task = await _serviceTask.GetByIdAsync(id.Value);
        if (task == null) return NotFound();

        return View(task);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var task = await _serviceTask.GetByIdAsync(id);
        if (task == null) return NotFound();

        int projectId = task.ProjectId;

        await _serviceTask.DeleteAsync(id);

        return RedirectToAction(nameof(Index), new { projectId = projectId });
    }
}