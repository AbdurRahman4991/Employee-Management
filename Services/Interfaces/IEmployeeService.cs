using MyFirstApi.DTOs.Common;
using MyFirstApi.DTOs.Employee;
using MyFirstApi.Models;

namespace MyFirstApi.Services.Interfaces
{
    public interface IEmployeeService
    {
        Task<PagedResponse<EmployeeResponse>> GetEmployeesAsync(
        string? search,
        int page = 1,
        int pageSize = 10);
        Task<List<Employee>> GetEmployeesForPdfAsync(string? search);
        Task<Employee?> GetEmployeeAsync(int id);

        Task<Employee> CreateEmployeeAsync(
            CreateEmployeeRequest request);

        Task<Employee?> UpdateEmployeeAsync(
            int id,
            UpdateEmployeeRequest request);

        Task<bool> DeleteEmployeeAsync(int id);
    }
}