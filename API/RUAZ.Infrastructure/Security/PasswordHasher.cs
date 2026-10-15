using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RUAZ.Application.Interfaces;

namespace RUAZ.Infrastructure.Security
{
    public class PasswordHasher : IPasswordHasher
    {
        public string GenerateHash (string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password);
        }

        public bool VerifyPassword (string passwordHash, string password)
        {
            return BCrypt.Net.BCrypt.Verify(password, passwordHash);
        }

    }
}