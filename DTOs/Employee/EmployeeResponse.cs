namespace MyFirstApi.DTOs.Employee
{
    public class EmployeeResponse
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string Phone { get; set; } = string.Empty;

        public DepartmentResponse Department { get; set; } = null!;
    }
}