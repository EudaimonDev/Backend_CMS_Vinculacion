using CMSVinculacion.Application.DTOs.auth;
using CMSVinculacion.Application.DTOs.gatekeeper;
using CMSVinculacion.Application.Interfaces;
using CMSVinculacion.Application.Services;
using CMSVinculacion.Domain.Entities.Gatekeeper;
using CMSVinculacion.Domain.Entities.Seguridad;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration;
using Moq;
using Xunit;

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
        //No se puede hashear en pruebas unitarias
        [Fact]
        public async Task LoginAsync_DeberiaRetornarTokens_CuandoCredencialesSonCorrectas()
        {
            // Arrange
            var password = "123456";
            var pass1 = "123456";
            var hashed = BCrypt.Net.BCrypt.HashPassword(password);

            BCrypt.Net.BCrypt.Verify(password, hashed).Should().BeTrue();

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
                Password = hashed
            };

            // Act
            var result = await _service.LoginAsync(request);

            // Assert
            result.Exito.Should().BeTrue(result.Mensaje);
            result.AccessToken.Should().NotBeNull();
            result.RefreshToken.Should().NotBeNull();
            result.Expiration.Should().NotBeNull();
            

            // Validación importante
            _userRepoMock.Verify(x => x.UpdateRefreshTokenAsync(
                user.UserId,
                It.IsAny<string>(),
                It.IsAny<DateTime>()),
                Times.Once);
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
            //// Arrange
            //_userRepoMock
            //    .Setup(x => x.GetByEmailAsync(It.IsAny<string>()))
            //    .ReturnsAsync((Users?)null);

            //var request = new LoginRequestDto
            //{
            //    Email = "test@test.com",
            //    Password = "123456"
            //};

            //var service = new AuthService(_userRepoMock.Object, _config);

            //// Act
            //var result = await service.LoginAsync(request);

            //// Assert
            //result.Exito.Should().BeFalse();
            //result.Mensaje.Should().Be("El usuario no está registrado.");
            // Arrange
            _userRepoMock
                .Setup(x => x.GetByEmailAsync(It.IsAny<string>()))
                .ReturnsAsync((Users?)null);

            var request = new LoginRequestDto
            {
                Email = "test@test.com",
                Password = "123456"
            };

            // Act
            var result = await _service.LoginAsync(request);

            // Assert
            result.Exito.Should().BeFalse();
            result.Mensaje.Should().Be("Credenciales inválidas.");
        }

        [Fact]
        public async Task RegistrarVisitanteAsync_DeberiaGuardarJson_CuandoDatosOpcionalesExisten()
        {
            // Arrange
            var repoMock = new Mock<IGatekeeperRepository>();

            Visitors? capturedVisitor = null;

            repoMock.Setup(x => x.GuardarVisitanteAsync(It.IsAny<Visitors>()))
                .Callback<Visitors>(v => capturedVisitor = v)
                .Returns(Task.CompletedTask);

            var service = new GatekeeperService(repoMock.Object);

            var request = new GatekeeperRequestDto
            {
                Sexo = "Masculino",
                Edad = 25,
                Extras = new Dictionary<string, object>
                {
                    { "Nombre", "Juan" },
                    { "Institucion", "UPS" }
                }
            };

            // Act
            var result = await service.RegistrarVisitanteAsync(request, "127.0.0.1");

            // Assert
            result.Exito.Should().BeTrue();
            capturedVisitor.Should().NotBeNull();
            capturedVisitor!.DataJson.Should().NotBeNull();
            capturedVisitor!.DataJson.Should().Contain("Juan");
            capturedVisitor.DataJson.Should().Contain("UPS");
        }

    }
}
