using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using RUAZ.Application.Contracts;
using RUAZ.Application.ExceptionsAPI;
using RUAZ.Application.Interfaces;
using RUAZ.Domain.Entities;

namespace RUAZ.Application
{
    public class AuthService(
      IUserRepository userRepository,
      IPasswordHasher passwordHasher,
      IJWTService jwtService
    )

    {
        public async Task<LoginResponse> AutenticateUser(LoginRequest request)
        {
            var user = await userRepository.GetByEmail(request.Email);

            if (user is null)
            {
                throw new NotFoundException("Usuário não encontrado!");
            }

            var equalPassword = passwordHasher.VerifyPassword(request.Password, user.PasswordHash);

            if (!equalPassword)
            {
                throw new NotFoundException("Usuário não encontrado!");
            }

            var token = jwtService.GenerateAcessToken(user);

            return token;

        }
    }
}