using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using project_manager.Models.Entities;
using project_manager.Services;
using System.Security.Claims;

namespace project_manager.Controllers.Api
{
    [Route("api/projectsapi/{projectId}/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class TasksApiController : ControllerBase
    {
        private readonly IServiceTask _serviceTask;

        public TasksApiController(IServiceTask serviceTask)
        {
            _serviceTask = serviceTask;
        }

        private string CurrentUserId => User.FindFirstValue(ClaimTypes.NameIdentifier);

        // POST: api/projectsapi/{id}/tasks/{id}
        [HttpGet("{taskId}")]
        public async Task<IActionResult> GetTask(int projectId, int taskId)
        {
            var task = await _serviceTask.GetByIdAsync(taskId, CurrentUserId);
            if(task == null)
            {
                return NotFound(new { message = "Проект не найден или у вас нет прав на добавление задач." });
            }
            return Ok(task);
        }

        [HttpGet]
        public async Task<IActionResult> GetTasks(int projectId)
        {
            var tasks = await _serviceTask.GetTasksByProjectAsync(projectId, CurrentUserId);

            return Ok(tasks);
        }

        // POST: api/projects/3002/tasks
        [HttpPost]
        public async Task<IActionResult> CreateTask(int projectId, [FromBody] ProjectTask task)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            task.ProjectId = projectId;

            var result = await _serviceTask.CreateAsync(task, CurrentUserId);

            if (result == null)
            {
                return NotFound(new { message = "Проект не найден или у вас нет прав на добавление задач." });
            }

            return Ok(result);
        }

        // PUT: api/projects/3002/tasks/5
        [HttpPut("{taskId}")]
        public async Task<IActionResult> UpdateTask(int projectId, int taskId, [FromBody] ProjectTask model)
        {
            var updatedTask = await _serviceTask.UpdateAsync(taskId, model, CurrentUserId);

            if (updatedTask == null) return NotFound(new { message = "Задача не найдена." });

            return Ok(updatedTask);
        }

        // DELETE: api/projects/3002/tasks/5
        [HttpDelete("{taskId}")]
        public async Task<IActionResult> DeleteTask(int taskId)
        {
            var deleted = await _serviceTask.DeleteAsync(taskId, CurrentUserId);

            if (!deleted) return NotFound();

            return NoContent();
        }
    }
}