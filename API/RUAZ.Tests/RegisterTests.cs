using Moq;
using RUAZ.Application.Contracts;
using RUAZ.Application.ExceptionsAPI;
using RUAZ.Application.Interfaces;
using RUAZ.Application.Services;
using RUAZ.Domain.Entities;
using RUAZ.Domain.Enums;

namespace RUAZ.Tests
{
    public class RegisterTests
    {
        private readonly Mock<IUserRepository> _userRepository = new();
        private readonly Mock<IPasswordHasher> _passwordHasher = new();
        private readonly Mock<IJWTService> _jwtService = new();
        private readonly RegisterService _sut;

        public RegisterTests()
        {
            _sut = new RegisterService(
                _userRepository.Object,
                _passwordHasher.Object,
                _jwtService.Object);
        }

        [Theory]
        [InlineData(TypeActivity.User)]
        [InlineData(TypeActivity.Store)]
        public async Task Register_ValidRequest_PersistsUserAndReturnsToken(TypeActivity typeActivity)
        {
            User? savedUser = null;

            _userRepository
                .Setup(r => r.GetByEmail("novo@ruaz.com"))
                .ReturnsAsync((User?)null);
            _passwordHasher
                .Setup(h => h.GenerateHash("Senha@123"))
                .Returns("hash-seguro");
            _userRepository
                .Setup(r => r.AddAsync(It.IsAny<User>()))
                .Callback<User>(u => savedUser = u)
                .Returns(Task.CompletedTask);
            _jwtService
                .Setup(j => j.GenerateAcessToken(It.IsAny<User>()))
                .Returns<User>(u => new LoginResponse
                {
                    TokenJWT = "jwt-token",
                    Role = u.TypeActivity.ToString(),
                    ExpiresIn = "3600"
                });

            var result = await _sut.RegisterUser(new RegisterRequest
            {
                Name = "Gabriel Costa",
                Email = "novo@ruaz.com",
                Password = "Senha@123",
                TypeActivity = typeActivity
            });

            Assert.NotNull(savedUser);
            Assert.Equal(typeActivity, savedUser.TypeActivity);
            Assert.Equal("hash-seguro", savedUser.PasswordHash);
            Assert.Equal("jwt-token", result.TokenJWT);
            Assert.Equal(typeActivity.ToString(), result.Role);
            _userRepository.Verify(r => r.AddAsync(It.IsAny<User>()), Times.Once);
        }

        [Theory]
        [InlineData(TypeActivity.Admin)]
        [InlineData((TypeActivity)999)]
        public async Task Register_DisallowedTypeActivity_ThrowsBadRequestException(TypeActivity typeActivity)
        {
            await Assert.ThrowsAsync<BadRequestException>(() =>
                _sut.RegisterUser(new RegisterRequest
                {
                    Name = "Admin",
                    Email = "admin@ruaz.com",
                    Password = "Senha@123",
                    TypeActivity = typeActivity
                }));

            _userRepository.Verify(r => r.AddAsync(It.IsAny<User>()), Times.Never);
        }

        [Fact]
        public async Task Register_EmailAlreadyExists_ThrowsConflictException()
        {
            _userRepository
                .Setup(r => r.GetByEmail("existente@ruaz.com"))
                .ReturnsAsync(new User { Email = "existente@ruaz.com" });

            await Assert.ThrowsAsync<ConflictException>(() =>
                _sut.RegisterUser(new RegisterRequest
                {
                    Name = "Gabriel",
                    Email = "existente@ruaz.com",
                    Password = "Senha@123",
                    TypeActivity = TypeActivity.User
                }));

            _userRepository.Verify(r => r.AddAsync(It.IsAny<User>()), Times.Never);
        }

        [Fact]
        public async Task Register_WeakPassword_ThrowsBadRequestException()
        {
            _userRepository
                .Setup(r => r.GetByEmail("novo@ruaz.com"))
                .ReturnsAsync((User?)null);

            await Assert.ThrowsAsync<BadRequestException>(() =>
                _sut.RegisterUser(new RegisterRequest
                {
                    Name = "Gabriel",
                    Email = "novo@ruaz.com",
                    Password = "123",
                    TypeActivity = TypeActivity.User
                }));

            _userRepository.Verify(r => r.AddAsync(It.IsAny<User>()), Times.Never);
        }
    }
}
