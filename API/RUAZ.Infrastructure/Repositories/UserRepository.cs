using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using RUAZ.Application.Interfaces;
using RUAZ.Domain.Entities;
using RUAZ.Infrastructure.Data;

namespace RUAZ.Infrastructure.Repositories
{
    public class UserRepository(
        AppDbContext _context
    ) : IUserRepository
    {
        public async Task<User?> GetByEmail (string email)
        {
            return await _context.Users
            .FirstOrDefaultAsync(e => e.Email == email);
        }
    }
}