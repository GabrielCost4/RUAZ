using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RUAZ.Application.Contracts
{
    public class LoginResponse
    {
        public string TokenJWT { get; set; }
        public string Role { get; set; }
        public string ExpiresIn { get; set; }
    }
}