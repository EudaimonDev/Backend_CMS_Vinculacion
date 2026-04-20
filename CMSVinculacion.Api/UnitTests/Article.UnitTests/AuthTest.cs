using CMSVinculacion.Application.DTOs.auth;
using CMSVinculacion.Application.Interfaces;
using CMSVinculacion.Application.Services;
using CMSVinculacion.Domain.Entities.Seguridad;
using Microsoft.Extensions.Configuration;
using Moq;
using Xunit;
using Microsoft.Extensions.Configuration;
using FluentAssertions;

namespace Article.UnitTests
{
    public class AuthTest
    {
        private readonly Mock<IUserRepository> _userRepoMock;
        private readonly IConfiguration _config;
        private readonly AuthService _service;

        public AuthTest()
        {
            _userRepoMock = new Mock<IUserRepository>();

            var inMemorySettings = new Dictionary<string, string>
        {
            {"Jwt:Key", "super_secret_key_123456789"},
            {"Jwt:Issuer", "testIssuer"},
            {"Jwt:Audience", "testAudience"}
        };

            _config = new ConfigurationBuilder()
                .AddInMemoryCollection(inMemorySettings!)
                .Build();

            _service = new AuthService(_userRepoMock.Object, _config);
        }

        [Fact]
        public async Task LoginAsync_DeberiaRetornarTokens_CuandoCredencialesSonCorrectas()
        {
            // Arrange
            var password = "123456";
            var hashed = BCrypt.Net.BCrypt.HashPassword(password);

            var user = new Users
            {
                UserId = 1,
                Email = "test@test.com",
                PasswordHash = hashed,
                Role = new Roles { RoleName = "Admin" }
            };

            _userRepoMock
                .Setup(x => x.GetByEmailAsync(It.IsAny<string>()))
                .ReturnsAsync(user);

            var request = new LoginRequestDto
            {
                Email = "test@test.com",
                Password = password
            };

            // Act
            var result = await _service.LoginAsync(request);

            // Assert
            result.Exito.Should().BeTrue();
            result.AccessToken.Should().NotBeNull();
            result.RefreshToken.Should().NotBeNull();
            result.Expiration.Should().NotBeNull();
        }

        [Fact]
        public async Task RefreshTokenAsync_DeberiaFallar_CuandoTokenExpirado()
        {
            // Arrange
            var user = new Users
            {
                UserId = 1,
                Email = "test@test.com",
                RefreshToken = "token123",
                RefreshTokenExpiry = DateTime.UtcNow.AddMinutes(-10), // EXPIRADO
                LastLogin = DateTime.UtcNow.AddDays(-1),
                Role = new Roles { RoleName = "Admin" }
            };

            _userRepoMock
                .Setup(x => x.GetByRefreshTokenAsync("token123"))
                .ReturnsAsync(user);

            // Act
            var result = await _service.RefreshTokenAsync("token123");

            // Assert
            result.Exito.Should().BeFalse();
            result.Mensaje.Should().Contain("expirada");
        }

        [Fact]
        public async Task Login_DeberiaFallar_SiUsuarioNoExiste()
        {
            // Arrange
            _userRepoMock
                .Setup(x => x.GetByEmailAsync("test@test.com"))
                .ReturnsAsync(new Users());

            var request = new LoginRequestDto
            {
                Email = "test@test.com",
                Password = "123456"
            };

            var service = new AuthService(_userRepoMock.Object, _config);

            // Act
            var result = await service.LoginAsync(request);

            // Assert
            result.Exito.Should().BeFalse();
            result.Mensaje.Should().Be("El usuario no está registrado.");
        }



    }
}
