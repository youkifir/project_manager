using Microsoft.AspNetCore.Mvc;
using project_manager.Models;
using project_manager.Services;

namespace project_manager.Controllers
{
    public class ProjectsController : Controller
    {
        private readonly IServiceProject _serviceProject;
        public ProjectsController(IServiceProject serviceProject)
        {
            _serviceProject = serviceProject;
        }
        //Get: /Projects/Index
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var projects = await _serviceProject.GetAllAsync();
            return View(projects);
        }
        // GET: /Projects/Details/5
        [HttpGet]
        public async Task<IActionResult> Details(int id)
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
        //Get: /Projects/Create
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }
        //Post: /Projects/Create/
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name,Description,OwnerId")]Project project)
        {
            if (ModelState.IsValid)
            {
                project.CreatedAt = DateTime.UtcNow;
                await _serviceProject.CreateAsync(project);
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
        public async Task<IActionResult> Update(int id, [Bind("ProjectId,Name,Description")] Project project)
        {
            if(id != project.ProjectId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    await _serviceProject.UpdateAsync(id, project);
                    return RedirectToAction(nameof(Index));
                }
                catch (KeyNotFoundException)
                {
                    return NotFound();
                }
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
