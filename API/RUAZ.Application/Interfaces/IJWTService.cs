using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RUAZ.Application.Contracts;
using RUAZ.Domain.Entities;

namespace RUAZ.Application.Interfaces
{
    public interface IJWTService
    {
        LoginResponse GenerateAcessToken(User user);
    }
}