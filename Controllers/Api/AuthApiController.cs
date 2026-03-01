using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Validation;
using Microsoft.IdentityModel.Tokens;
using project_manager.Models.DTOs;
using project_manager.Models.Entities;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace project_manager.Controllers.Api
{

    [Route("api/[controller]")]
    [ApiController]
    public class AuthApiController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IConfiguration _config;
        public AuthApiController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IConfiguration configuration)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _config = configuration;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByNameAsync(model.UserName);
                if (user == null) return BadRequest("Wrong username or password");

                var result = await _signInManager.PasswordSignInAsync(
                    user.UserName,
                    model.Password,
                    isPersistent: false,
                    lockoutOnFailure: false
                    );
                if (result.Succeeded)
                {
                    var jwt_token = GenerateJwtToken(user);
                    return Ok(new { token = jwt_token });
                }
                else
                {
                    return BadRequest();
                }
            }
            return BadRequest(ModelState);
        }
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] LoginDto model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var newUser = new ApplicationUser
            {
                UserName = model.UserName
            };

            var result = await _userManager.CreateAsync(newUser, model.Password);

            if (result.Succeeded)
            {
                return Ok(new { message = "Регистрация прошла успешно!" });
            }

            return BadRequest(result.Errors);
        }
        private string GenerateJwtToken(IdentityUser user)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim("name", user.UserName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var expireMinutes = int.Parse(_config["Jwt:ExpireMinutes"] ?? "60");

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(expireMinutes),
                signingCredentials: credentials
            );
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
