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
using Microsoft.AspNetCore.SignalR;
using MyFirstApi.Hubs;

namespace MyFirstApi.Services
{
    public class AuthService : IAuthService
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly PasswordHasher<User> _passwordHasher;
        private readonly IEmailService _emailService;
        private readonly IEmailQueueService _emailQueue;        
        private readonly IHubContext<NotificationHub> _hub;

        public AuthService(
            AppDbContext context,
            IConfiguration configuration,
            IEmailService emailService,
            IEmailQueueService emailQueue,
            IHubContext<NotificationHub> hub)
        {
            _context = context;
            _configuration = configuration;
            _emailService = emailService;
            _emailQueue = emailQueue;
            _passwordHasher = new PasswordHasher<User>();
            _hub = hub;
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
                Username = request.Username,
                Email = request.Email
            };


            // Hash password
            user.Password = _passwordHasher.HashPassword(
                user,
                request.Password
            );

            _context.Users.Add(user);

            await _context.SaveChangesAsync();

             // Realtime notification
            await _hub.Clients.All.SendAsync(
                "ReceiveNotification",
                new
                {
                    Message = $"New user registered: {user.Username}",
                    UserId = user.Id,
                    Username = user.Username
                }
            );

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

        // Forget Password

        public async Task<bool> ForgotPasswordAsync(
        ForgotPasswordRequest request)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(x =>
                    x.Email == request.Email);

            if (user == null)
            {
                return false;
            }

            var otp = GenerateOtp();

            var subject = "Password Reset OTP";

            var body = $"""
                <html>
                <body>
                    <h2>Password Reset</h2>

                    <p>Your OTP is:</p>

                    <h1>{otp}</h1>

                    <p>This OTP will expire in 5 minutes.</p>
                </body>
                </html>
                """;

            await _emailQueue.QueueEmailAsync(
                user.Email!,
                subject,
                body
            );

            return true;
        }

        private string GenerateOtp()
        {
            return Random.Shared
                .Next(100000, 1000000)
                .ToString();
        }
        
    }
}