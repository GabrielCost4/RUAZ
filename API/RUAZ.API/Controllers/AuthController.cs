using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using RUAZ.Application;
using RUAZ.Application.Contracts;
using RUAZ.Application.ExceptionsAPI;

namespace RUAZ.API
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController(
        AuthService authService
    ) : ControllerBase
    {
        [HttpPost]
        public async Task<LoginResponse> Login (LoginRequest request)
        {
            var result = await authService.AutenticateUser(request);

            if(result is null)
            {
                
            }

            return result;
        }
        
    }
}