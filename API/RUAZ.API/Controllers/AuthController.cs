using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using RUAZ.Application;
using RUAZ.Application.Contracts;
using RUAZ.Application.ExceptionsAPI;
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
        [HttpPost]
        public async Task<LoginResponse> Login (LoginRequest request)
        {
            var result = await authService.AutenticateUser(request);

            if(result is null)
            {
                throw new BadRequestException("Credenciais inválidas!");
            }

            return result;
        }
        
        [HttpPost("register")]
        public async Task<LoginResponse> Register (RegisterRequest request)
        {
            var result = await registerService.GetByEmail(request);

            if(result is null)
            {
                throw new BadRequestException("Credenciais inválidas!");
            }

            return result;
        }
    }
}