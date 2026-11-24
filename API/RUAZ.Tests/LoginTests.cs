using Moq;
using RUAZ.Application.Contracts;
using RUAZ.Application.ExceptionsAPI;
using RUAZ.Application.Interfaces;
using RUAZ.Application.Services;
using RUAZ.Domain.Entities;
using RUAZ.Domain.Enums;

namespace RUAZ.Tests
{
    public class LoginTests
    {
        private readonly Mock<IUserRepository> _userRepository = new();
        private readonly Mock<IPasswordHasher> _passwordHasher = new();
        private readonly Mock<IJWTService> _jwtService = new();
        private readonly AuthService _sut;

        public LoginTests()
        {
            _sut = new AuthService(
                _userRepository.Object,
                _passwordHasher.Object,
                _jwtService.Object);
        }

        [Fact]
        public async Task AutenticateUser_ValidCredentials_ReturnsLoginResponse()
        {
            var user = new User
            {
                Id = Guid.NewGuid(),
                Name = "Gabriel",
                NameNormalized = "GABRIEL",
                Email = "gabriel@ruaz.com",
                PasswordHash = "hashed-password",
                TypeActivity = TypeActivity.Store
            };

            var expected = new LoginResponse
            {
                TokenJWT = "jwt-token",
                Role = nameof(TypeActivity.Store),
                ExpiresIn = "3600"
            };

            _userRepository
                .Setup(r => r.GetByEmail("gabriel@ruaz.com"))
                .ReturnsAsync(user);
            _passwordHasher
                .Setup(h => h.VerifyPassword("Senha@123", "hashed-password"))
                .Returns(true);
            _jwtService
                .Setup(j => j.GenerateAcessToken(user))
                .Returns(expected);

            var result = await _sut.AutenticateUser(new LoginRequest
            {
                Email = "gabriel@ruaz.com",
                Password = "Senha@123"
            });

            Assert.Equal(expected.TokenJWT, result.TokenJWT);
            Assert.Equal(expected.Role, result.Role);
            Assert.Equal(expected.ExpiresIn, result.ExpiresIn);
        }

        [Fact]
        public async Task AutenticateUser_UserNotFound_ThrowsNotFoundException()
        {
            _userRepository
                .Setup(r => r.GetByEmail("naoexiste@ruaz.com"))
                .ReturnsAsync((User?)null);

            await Assert.ThrowsAsync<NotFoundException>(() =>
                _sut.AutenticateUser(new LoginRequest
                {
                    Email = "naoexiste@ruaz.com",
                    Password = "Senha@123"
                }));

            _jwtService.Verify(j => j.GenerateAcessToken(It.IsAny<User>()), Times.Never);
        }

        [Fact]
        public async Task AutenticateUser_InvalidPassword_ThrowsNotFoundException()
        {
            var user = new User
            {
                Id = Guid.NewGuid(),
                Name = "Gabriel",
                NameNormalized = "GABRIEL",
                Email = "gabriel@ruaz.com",
                PasswordHash = "hashed-password",
                TypeActivity = TypeActivity.User
            };

            _userRepository
                .Setup(r => r.GetByEmail("gabriel@ruaz.com"))
                .ReturnsAsync(user);
            _passwordHasher
                .Setup(h => h.VerifyPassword("Errada@123", "hashed-password"))
                .Returns(false);

            await Assert.ThrowsAsync<NotFoundException>(() =>
                _sut.AutenticateUser(new LoginRequest
                {
                    Email = "gabriel@ruaz.com",
                    Password = "Errada@123"
                }));

            _jwtService.Verify(j => j.GenerateAcessToken(It.IsAny<User>()), Times.Never);
        }
    }
}
