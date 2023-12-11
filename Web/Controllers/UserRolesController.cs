using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Web.Auth;

namespace Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserRolesController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole<int>> _roleManager;

        public UserRolesController(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole<int>> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }
       
        // GET: api/userroles/{userId}
        [HttpGet("{userId}")]
        public async Task<IActionResult> GetUserRoles(string userId)
        {
            ApplicationUser user = await _userManager.FindByIdAsync(userId);

            if (user == null)
            {
                return NotFound("User not found");
            }

            var userRoles = await _userManager.GetRolesAsync(user);

            var userInformation = new
            {
                UserId = user.Id,
                UserName = user.UserName,
                Email = user.Email,
                Roles = userRoles
            };

            return Ok(userInformation);
        }

        // POST: api/userroles/{userId}
        [HttpPost("{userId}")]
        public async Task<IActionResult> AddRolesToUser(string userId, [FromBody] List<string> roleNames)
        {
            ApplicationUser user = await _userManager.FindByIdAsync(userId);

            if (user == null)
            {
                return NotFound("User not found");
            }

            // Ensure the roles exist before attempting to add them
            var rolesToAdd = roleNames.Where(role => _roleManager.RoleExistsAsync(role).Result).ToList();

            if (rolesToAdd.Count == 0)
            {
                return BadRequest("No valid roles provided");
            }

            var result = await _userManager.AddToRolesAsync(user, rolesToAdd);

            if (result.Succeeded)
            {
                return Ok("Roles added to user successfully");
            }
            else
            {
                return BadRequest(result.Errors);
            }
        }

        // DELETE: api/userroles/{userId}
        [HttpDelete("{userId}")]
        public async Task<IActionResult> RemoveRolesFromUser(string userId, [FromBody] List<string> roleNames)
        {
            ApplicationUser user = await _userManager.FindByIdAsync(userId);

            if (user == null)
            {
                return NotFound("User not found");
            }

            var result = await _userManager.RemoveFromRolesAsync(user, roleNames);

            if (result.Succeeded)
            {
                return Ok("Roles removed from user successfully");
            }
            else
            {
                return BadRequest(result.Errors);
            }
        }

        // Other Web API actions...
    }
}
