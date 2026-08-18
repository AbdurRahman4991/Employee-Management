using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using MyFirstApi.Data;
using MyFirstApi.DTOs.Auth;
using MyFirstApi.Models;
using MyFirstApi.Services.Interfaces;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace MyFirstApi.Services
{
    public class AuthService : IAuthService
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly PasswordHasher<User> _passwordHasher;

        public AuthService(
            AppDbContext context,
            IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
            _passwordHasher = new PasswordHasher<User>();
        }

        // =========================
        // REGISTER
        // =========================

        public async Task<object> RegisterAsync(RegisterRequest request)
        {
            var existingUser = await _context.Users
                .FirstOrDefaultAsync(x =>
                    x.Username == request.Username);

            if (existingUser != null)
            {
                throw new Exception("Username already exists");
            }

            var user = new User
            {
                Username = request.Username
            };

            // Hash password
            user.Password = _passwordHasher.HashPassword(
                user,
                request.Password
            );

            _context.Users.Add(user);

            await _context.SaveChangesAsync();

            return new
            {
                Id = user.Id,
                Username = user.Username
            };
        }


        // =========================
        // LOGIN
        // =========================

        public async Task<string?> LoginAsync(LoginRequest request)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(x =>
                    x.Username == request.Username);

            if (user == null)
            {
                return null;
            }

            var passwordResult =
                _passwordHasher.VerifyHashedPassword(
                    user,
                    user.Password,
                    request.Password
                );

            if (passwordResult == PasswordVerificationResult.Failed)
            {
                return null;
            }

            return GenerateToken(user);
        }


        // =========================
        // GENERATE JWT
        // =========================

        private string GenerateToken(User user)
        {
            var claims = new[]
            {
                new Claim(
                    ClaimTypes.NameIdentifier,
                    user.Id.ToString()
                ),

                new Claim(
                    ClaimTypes.Name,
                    user.Username
                )
            };

            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(
                    _configuration["Jwt:Key"]!
                )
            );

            var credentials = new SigningCredentials(
                key,
                SecurityAlgorithms.HmacSha256
            );

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(1),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler()
                .WriteToken(token);
        }
    }
}