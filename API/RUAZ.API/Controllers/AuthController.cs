using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using RUAZ.Application;

namespace RUAZ.API
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController(
        AuthService authService
    ) : ControllerBase
    {
        [HttpPost]
        public async Task<IActionResult> Login (LoginRequest request)
        {
            var result = await authService.AutenticateUser(request);

            return result;
        }
        
    }
}