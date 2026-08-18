using MyFirstApi.DTOs.Auth;

namespace MyFirstApi.Services.Interfaces
{
    public interface IAuthService
    {
        Task<object> RegisterAsync(RegisterRequest request);
        Task<string?> LoginAsync(LoginRequest request);
    }
}