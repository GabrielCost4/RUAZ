using Microsoft.AspNetCore.Mvc;
using RUAZ.Application;
using RUAZ.Application.Contracts;
using RUAZ.Application.Services;

namespace RUAZ.API
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController(
        AuthService authService,
        RegisterService registerService
    ) : ControllerBase
    {
        [HttpPost("login")]
        public async Task<LoginResponse> Login(LoginRequest request)
        {
            var result = await authService.AutenticateUser(request);

            return result;
        }

        [HttpPost("register")]
        public async Task<LoginResponse> Register(RegisterRequest request)
        {
            var result = await registerService.RegisterUser(request);

            return result;
        }
    }
}