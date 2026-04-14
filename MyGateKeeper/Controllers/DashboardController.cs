using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyGateKeeper.Services;

namespace MyGateKeeper.Controllers
{
    [ApiController]
    [Route("api/dashboard")]
    public class DashboardController(DashboardOrchestrationService dashboardOrchestrationService) : ControllerBase
    {
        private readonly DashboardOrchestrationService _dashboardOrchestrationService = dashboardOrchestrationService;

        [Authorize]
        [HttpGet("context")]
        public async Task<IActionResult> GetContext(CancellationToken cancellationToken)
        {
            var authorizationHeader = Request.Headers.Authorization.ToString();

            if (!authorizationHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
            {
                return Unauthorized("Missing bearer token");
            }

            var token = authorizationHeader["Bearer ".Length..].Trim();
            var response = await _dashboardOrchestrationService.BuildAsync(token, User, cancellationToken);

            if (response is null)
            {
                return NotFound();
            }

            return Ok(response);
        }
    }
}
