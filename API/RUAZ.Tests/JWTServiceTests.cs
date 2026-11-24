using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.Extensions.Configuration;
using Moq;
using RUAZ.Domain.Entities;
using RUAZ.Domain.Enums;
using RUAZ.Infrastructure.Security;

namespace RUAZ.Tests
{
    public class JWTServiceTests
    {
        private static JWTService CreateService()
        {
            var configuration = new Mock<IConfiguration>();
            configuration.Setup(c => c["Jwt:Key"]).Returns("chave-super-secreta-para-testes-com-32-bytes");
            configuration.Setup(c => c["Jwt:Issuer"]).Returns("RUAZ.Test");
            configuration.Setup(c => c["Jwt:Audience"]).Returns("RUAZ.Client");
            configuration.Setup(c => c["Jwt:ExpiresInMinutes"]).Returns("60");

            return new JWTService(configuration.Object);
        }

        [Theory]
        [InlineData(TypeActivity.User)]
        [InlineData(TypeActivity.Store)]
        [InlineData(TypeActivity.Admin)]
        public void GenerateAcessToken_EmitsRoleAndTypeActivityClaims(TypeActivity typeActivity)
        {
            var sut = CreateService();
            var user = new User
            {
                Id = Guid.NewGuid(),
                Name = "Gabriel",
                NameNormalized = "GABRIEL",
                Email = "gabriel@ruaz.com",
                PasswordHash = "hash",
                TypeActivity = typeActivity
            };

            var response = sut.GenerateAcessToken(user);
            var token = new JwtSecurityTokenHandler().ReadJwtToken(response.TokenJWT);

            Assert.Equal(typeActivity.ToString(), response.Role);
            Assert.Equal(typeActivity.ToString(), token.Claims.First(c => c.Type == ClaimTypes.Role).Value);
            Assert.Equal(typeActivity.ToString(), token.Claims.First(c => c.Type == "type_activity").Value);
        }
    }
}
