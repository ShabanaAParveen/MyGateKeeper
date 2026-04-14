using AuthServer.Repositories;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using AuthServer.Models;
using LoginRequest = AuthServer.Models.LoginRequest;

namespace AuthServer.Controllers
{
    [ApiController]
    [Route("auth")]
    public class AuthController : ControllerBase
    {
        private readonly IUserRepository _repo;
        private readonly HttpClient _httpClient;
        private readonly TokenService _tokenService;

        public AuthController(
            IUserRepository repo,
            IHttpClientFactory httpClientFactory,
            TokenService tokenService)
        {
            _repo = repo;
            _httpClient = httpClientFactory.CreateClient("authz");
            _tokenService = tokenService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginRequest request)
        {
            // 1️⃣ validate user
            var user = _repo.GetUser(request.UserName, request.Password);
            try
            {
                if (user == null)
                    return StatusCode(500,"user empty");
                else
                {
                    // 2️⃣ call AuthZ server → get roles

                    var roles = await _httpClient.GetFromJsonAsync<List<string>>(
                        $"https://localhost:7184/authz/roles/{user.UserId}");

                    if (roles == null)
                        return StatusCode(500, "Roles API returned null");

                    if (roles.Count == 0)
                        return StatusCode(500, "Roles empty");

                    var token = _tokenService.GenerateToken(user.UserName, roles);


                    return Ok(new { token });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
            // 3️⃣ generate JWT
            //var token = _tokenService.GenerateToken(user.UserName, roles);

            // 4️⃣ return
           // return Ok(new { token });
        }
    }
}