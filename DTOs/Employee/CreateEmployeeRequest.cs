using System.ComponentModel.DataAnnotations;

namespace MyFirstApi.DTOs.Employee
{
    public class CreateEmployeeRequest
    {
        [Required(ErrorMessage = "Name is required")]
        [StringLength(100, MinimumLength = 2,
            ErrorMessage = "Name must be between 2 and 100 characters")]
        public string Name { get; set; } = "";

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email address")]
        public string Email { get; set; } = "";

        [Required(ErrorMessage = "Phone is required")]
        public string Phone { get; set; } = "";

        [Required(ErrorMessage = "Department is required")]
        public int DepartmentId { get; set; }
    }
}