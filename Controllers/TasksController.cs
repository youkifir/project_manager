using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using project_manager.Models;
using project_manager.Services;
using System.Security.Claims;

namespace project_manager.Controllers
{
    public class TasksController : Controller
    {
        private readonly IServiceTask _serviceTask;
        private readonly IServiceProject _serviceProject;
        public TasksController(IServiceTask serviceTask, IServiceProject serviceProject)
        {
            _serviceTask = serviceTask;
            _serviceProject = serviceProject;
        }

        //Get: /Tasks/Index
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var tasks = await _serviceTask.GetAllAsync();
            return View(tasks);
        }

        //Get: /Tasks/Create
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        //Post: /Tasks/Create
        [HttpPost]
        [IgnoreAntiforgeryToken]
        public async Task<IActionResult> Create(
            [Bind("Title,Description,DueDate,ProjectId,AssignedUserId")] Models.Task task)
        {
            if (ModelState.IsValid)
            {
                task.IsCompleted = false;
                await _serviceTask.CreateAsync(task);

                return RedirectToAction(nameof(Index));
            }
            return BadRequest();
        }

        //Get: /Tasks/Update
        [HttpGet]
        public async Task<IActionResult> Update(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }

            var task = await _serviceProject.GetByIdAsync(id.Value);

            if (task == null)
            {
                return NotFound();
            }
            return View(task);
        }

        //Post: /Tasks/Update/1
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(int id, [Bind("TaskId,Title,Description,DueDate,IsCompleted,ProjectId,AssignedUserId")] Models.Task task)
        {
            if (id != task.TaskId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {

                await _serviceTask.UpdateAsync(id, task);
                RedirectToAction(nameof(Index));
            }
            return View(task);
        }

        //Get: /Tasks/Delete
        [HttpGet("{id}")]
        public IActionResult Delete(int id)
        {
            return View("DeleteConfirmed", id);
        }

        //Post: /Tasks/Delete/1
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _serviceTask.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }

        //Get: /Tasks/Details/5
        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            if (id == 0)
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
    }
}