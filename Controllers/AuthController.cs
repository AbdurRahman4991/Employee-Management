using Microsoft.AspNetCore.Mvc;
using MyFirstApi.DTOs.Auth;
using MyFirstApi.DTOs.Common;
using MyFirstApi.Services.Interfaces;

namespace MyFirstApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(
            RegisterRequest request)
        {
            try
            {
                var user = await _authService.RegisterAsync(request);

                return Ok(
                    new ApiResponse<object>(
                        true,
                        "Registration successful",
                        user
                    )
                );
            }
            catch (Exception ex)
            {
                return BadRequest(
                    new ApiResponse<object>(
                        false,
                        ex.Message
                    )
                );
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(
            LoginRequest request)
        {
            var token = await _authService.LoginAsync(request);

            if (token == null)
            {
                return Unauthorized(
                    new ApiResponse<object>(
                        false,
                        "Invalid username or password"
                    )
                );
            }

            var response = new LoginResponse
            {
                Token = token
            };

            return Ok(
                new ApiResponse<LoginResponse>(
                    true,
                    "Login successful",
                    response
                )
            );
        }
    }
}