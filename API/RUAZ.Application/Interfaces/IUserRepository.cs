using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RUAZ.Domain.Entities;

namespace RUAZ.Application.Interfaces
{
    public interface IUserRepository
    {
        Task<User?> GetByEmail(string email);
        Task AddAsync(User user);
    }
}