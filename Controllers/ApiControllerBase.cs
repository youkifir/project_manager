using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace project_manager.Controllers
{
    public abstract class ApiControllerBase : ControllerBase
    {
        protected string CurrentUserId => User.FindFirstValue(ClaimTypes.NameIdentifier);
    }
}
