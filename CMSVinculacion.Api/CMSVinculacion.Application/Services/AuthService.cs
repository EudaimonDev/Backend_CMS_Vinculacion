using CMSVinculacion.Application.DTOs.auth;
using CMSVinculacion.Application.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace CMSVinculacion.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepo;
        private readonly IConfiguration _config;

        public AuthService(IUserRepository userRepo, IConfiguration config)
        {
            _userRepo = userRepo;
            _config = config;
        }

        public async Task<LoginResponseDto> LoginAsync(LoginRequestDto request)
        {
            var user = await _userRepo.GetByEmailAsync(request.Email);

            if (user is null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
                return new LoginResponseDto { Exito = false, Mensaje = "Credenciales inválidas." };

            var accessToken = GenerateAccessToken(user.UserId, user.Email, user.Role?.RoleName ?? "Editor");
            var refreshToken = GenerateRefreshToken();
            var expiry = DateTime.UtcNow.AddHours(1);

            await _userRepo.UpdateRefreshTokenAsync(user.UserId, refreshToken, DateTime.UtcNow.AddDays(7));

            return new LoginResponseDto
            {
                Exito = true,
                Mensaje = "Login exitoso.",
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                Expiration = expiry
            };
        }

        public async Task<LoginResponseDto> RefreshTokenAsync(string refreshToken)
        {
            var user = await _userRepo.GetByRefreshTokenAsync(refreshToken);
            if (user is null || user.RefreshTokenExpiry < DateTime.UtcNow)
                return new LoginResponseDto { Exito = false, Mensaje = "Refresh token inválido o expirado." };

            var newAccess = GenerateAccessToken(user.UserId, user.Email, user.Role?.RoleName ?? "Editor");
            var newRefresh = GenerateRefreshToken();
            await _userRepo.UpdateRefreshTokenAsync(user.UserId, newRefresh, DateTime.UtcNow.AddDays(7));

            return new LoginResponseDto
            {
                Exito = true,
                Mensaje = "Token renovado.",
                AccessToken = newAccess,
                RefreshToken = newRefresh,
                Expiration = DateTime.UtcNow.AddHours(1)
            };
        }

        public async Task LogoutAsync(string userId)
        {
            if (int.TryParse(userId, out var id))
                await _userRepo.UpdateRefreshTokenAsync(id, string.Empty, DateTime.MinValue);
        }

        private string GenerateAccessToken(int userId, string email, string role)
        {
            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_config["Jwt:Key"]
                    ?? throw new InvalidOperationException("Jwt:Key no configurado.")));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim("userId", userId.ToString()),
                new Claim("email", email),
                new Claim(ClaimTypes.Role, role)
            };

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(1),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private static string GenerateRefreshToken()
        {
            var bytes = new byte[64];
            RandomNumberGenerator.Fill(bytes);
            return Convert.ToBase64String(bytes);
        }
    }
}