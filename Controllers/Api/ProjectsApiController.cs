using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using project_manager.Models;
using project_manager.Models.DTOs;
using project_manager.Models.Entities;
using project_manager.Services;
using System.Security.Claims;

namespace project_manager.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class ProjectsApiController : ApiControllerBase
    {
        private readonly IServiceProject _serviceProject;
        public ProjectsApiController(IServiceProject serviceProject, UserManager<ApplicationUser> userManager)
        {
            _serviceProject = serviceProject;
        }

        //Get: api/projects/
        [HttpGet]
        public async Task<IActionResult> GetProjects()
        {
            var projects = await _serviceProject.GetUserProjectsAsync(CurrentUserId);
            return Ok(new { projects = projects });
        }

        //Get: api/projects/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetProject(int id)
        {
            var project = await _serviceProject.GetByIdAsync(id, CurrentUserId);

            if (project == null) return NotFound(new { message = $"Проект с ID {id} не найден или у вас нет к нему доступа." });

            return Ok(project);
        }

        //Post: api/projects
        [HttpPost]
        public async Task<IActionResult> CreateProject([FromBody] ProjectDto model)
        {
            var project = new Project
            {
                Name = model.Name,
                Description = model.Description,
                OwnerId = CurrentUserId,
            };

            var createdProject = await _serviceProject.CreateAsync(project, CurrentUserId);

            return CreatedAtAction(nameof(GetProject), new { id = createdProject.ProjectId });
        }

        //Delete: api/projects/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProject(int id)
        {
            var deletedProduct = await _serviceProject.DeleteAsync(id, CurrentUserId);
            if (deletedProduct == null) return NotFound(new { message = $"Проект с ID {id} не найден или у вас нет к нему доступа." });
            return Ok(deletedProduct);
        }

        //Put: api/projects/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProject(int id, [FromBody] ProjectDto model)
        {
            var updatedProject = await _serviceProject.UpdateAsync(id, model, CurrentUserId);
            if (updatedProject == null)
            {
                return NotFound(new { message = "Проект не найден или у вас нет прав на редактирование" });
            }
            return Ok(updatedProject);
        }

    }
}
