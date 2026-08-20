using MyFirstApi.DTOs.Auth;

namespace MyFirstApi.Services.Interfaces
{
    public interface IAuthService
    {
        Task<object> RegisterAsync(RegisterRequest request);
        Task<string?> LoginAsync(LoginRequest request);
        Task<bool> ForgotPasswordAsync(ForgotPasswordRequest request);
        // Task<bool> VerifyOtpAsync(VerifyOtpRequest request);
        // Task<bool> ResetPasswordAsync(ResetPasswordRequest request);
    }
}