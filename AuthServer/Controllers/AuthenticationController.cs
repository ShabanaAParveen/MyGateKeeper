using AuthServer.Models;
using AuthServer.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace AuthServer.Controllers
{
    [ApiController]
    [Route("auth")]
    public class AuthenticationController : ControllerBase
    {
        private readonly IUserRepository _repo;
        private readonly JwtTokenService _tokenService;

        public AuthenticationController(
            IUserRepository repo,
            JwtTokenService tokenService)
        {
            _repo = repo;
            _tokenService = tokenService;
        }

        [HttpPost("login")]
        public IActionResult Login(LoginRequest request)
        {
            try
            {
                var user = _repo.GetUser(request.UserName, request.Password);

                if (user == null)
                {
                    return StatusCode(500, "user empty");
                }

                var token = _tokenService.GenerateToken(user.Id.ToString());

                return Ok(new { token });
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}
