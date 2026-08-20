using System.ComponentModel.DataAnnotations;

namespace MyFirstApi.DTOs.Auth
{
    public class ForgotPasswordRequest
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = "";
    }
}