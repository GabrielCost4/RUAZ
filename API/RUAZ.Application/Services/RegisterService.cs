using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Mapster;
using RUAZ.Application.Contracts;
using RUAZ.Application.ExceptionsAPI;
using RUAZ.Application.Interfaces;
using RUAZ.Domain.Entities;
using RUAZ.Domain.Enums;
using RUAZ.Domain.Normalizer;

namespace RUAZ.Application.Services
{
    public class RegisterService(
        IUserRepository userRepository,
        IPasswordHasher passwordHasher,
        IJWTService jwtService
    )
    {
        public async Task<LoginResponse> RegisterUser(RegisterRequest request)
        {
            if (request.TypeActivity is not (TypeActivity.User or TypeActivity.Store))
            {
                throw new BadRequestException("Tipo de atividade inválido para cadastro!");
            }

            var userFound = await userRepository.GetByEmail(request.Email);

            if (userFound is not null)
            {
                throw new ConflictException("Já existe um usuário com este email!");
            }

            var normalizated = StringNormalizer.Normalize(request.Name);

            var passwordStrong =
             request.Password.Length >= 8 &&
             request.Password.Any(char.IsDigit) &&
             request.Password.Any(char.IsLetter) &&
             request.Password.Any(c =>
                 char.IsSymbol(c) || char.IsPunctuation(c));

            if (!passwordStrong)
            {
                throw new BadRequestException("Senha fraca!");
            }

            var hash = passwordHasher.GenerateHash(request.Password);

            var user = request.Adapt<User>();
            user.PasswordHash = hash;
            user.NameNormalized = normalizated;

            await userRepository.AddAsync(user);

            return jwtService.GenerateAcessToken(user);
        }
    }
}