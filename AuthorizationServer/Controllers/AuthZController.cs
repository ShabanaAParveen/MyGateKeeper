using AuthorizationServer.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace AuthorizationServer.Controllers
{
    [ApiController]
    [Route("authz")]
    public class AuthZController(IAuthorizationRepository repo) : ControllerBase
    {
        private readonly IAuthorizationRepository _repo = repo;

        [HttpGet("roles/{code}")]
        public IActionResult GetRoles(string code)
        {
            var roles = _repo.GetRoles(code);

            if (roles.Count == 0)
            {
                return NotFound();
            }

            return Ok(roles);
        }

        [Authorize]
        [HttpPost("dashboard-context")]
        public IActionResult GetDashboardContext()
        {
            var userCode = User.FindFirstValue("userCode");

            if (string.IsNullOrWhiteSpace(userCode))
            {
                return Unauthorized("Missing user code claim");
            }

            var context = _repo.GetDashboardContext(userCode);

            if (context is null)
            {
                return NotFound();
            }

            return Ok(context);
        }
    }
}
