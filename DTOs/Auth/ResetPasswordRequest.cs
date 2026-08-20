using System.ComponentModel.DataAnnotations;

namespace MyFirstApi.DTOs.Auth
{
    public class ResetPasswordRequest
{
        [Required]
        public string Email { get; set; } = "";

        [Required]
        [StringLength(6, MinimumLength = 6)]
        public string Otp { get; set; } = "";

        [Required]
        [MinLength(6)]
        public string NewPassword { get; set; } = "";

        [Required]
        [Compare("NewPassword")]
        public string ConfirmPassword { get; set; } = "";
    }
}