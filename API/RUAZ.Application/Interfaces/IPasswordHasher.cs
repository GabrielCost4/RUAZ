using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RUAZ.Application.Interfaces
{
    public interface IPasswordHasher
    {
        string GenerateHash(string password);
        bool VerifyPassword(string password, string passwordHash);
    }
}