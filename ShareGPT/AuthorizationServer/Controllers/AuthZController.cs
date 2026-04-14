using AuthorizationServer.Repositories;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;

namespace AuthorizationServer.Controllers
{
    [ApiController]
    [Route("authz")]
    public class AuthZController(IAuthorizationRepository repo) : ControllerBase
    {
        private readonly IAuthorizationRepository _repo = repo;

        [HttpGet("roles/{userId}")]
        public IActionResult GetRoles(int userId)
        {
            var roles = _repo.GetRoles(userId);

            if (roles == null || roles.Count == 0)
                return NotFound();

            return Ok(roles);
        }
    }
}
