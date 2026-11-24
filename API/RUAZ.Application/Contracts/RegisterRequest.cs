using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RUAZ.Domain.Enums;

namespace RUAZ.Application.Contracts
{
    public class RegisterRequest
    {
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public TypeActivity TypeActivity { get; set; }

    }
}