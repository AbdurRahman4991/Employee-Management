using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyFirstApi.DTOs.Common;
using MyFirstApi.Models;
using MyFirstApi.Services.Interfaces;
using MyFirstApi.DTOs.Employee;
using Microsoft.AspNetCore.RateLimiting;

namespace MyFirstApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EmployeeController : ControllerBase
    {
        private readonly IEmployeeService _employeeService;
        private readonly IPdfService _pdfService;

        public EmployeeController(IEmployeeService employeeService, 
        IPdfService pdfService)
        {
            _employeeService = employeeService;
            _pdfService = pdfService;
        }

        // =========================================
        // GET: api/employee
        // =========================================

        [Authorize]
        [HttpGet]
        [EnableRateLimiting("api")]
        public async Task<IActionResult> GetEmployees(
            string? search,
            int page = 1,
            int pageSize = 10)
        {
            var result = await _employeeService.GetEmployeesAsync(
                search,
                page,
                pageSize
            );

            return Ok(
                new ApiResponse<PagedResponse<Employee>>(
                    true,
                    "Employees fetched successfully",
                    result
                )
            );
        }

        [HttpGet("pdf")]
        [EnableRateLimiting("api")]
        public async Task<IActionResult> GenerateEmployeePdf(
            [FromQuery] string? search)
        {
            var employees =
                await _employeeService.GetEmployeesForPdfAsync(search);

            var pdf =
                _pdfService.GenerateEmployeePdf(employees);

            return File(
                pdf,
                "application/pdf",
                "employees.pdf"
            );
        }


        // =========================================
        // GET: api/employee/1
        // =========================================
        [Authorize]
        [HttpGet("{id}")]
        [EnableRateLimiting("api")]
        public async Task<IActionResult> GetEmployee(int id)
        {
            var employee = await _employeeService.GetEmployeeAsync(id);

            if (employee == null)
            {
                return NotFound(
                    new ApiResponse<object>(
                        false,
                        "Employee not found"
                    )
                );
            }

            return Ok(
                new ApiResponse<Employee>(
                    true,
                    "Employee fetched successfully",
                    employee
                )
            );
        }


        // =========================================
        // POST: api/employee
        // =========================================

        [Authorize]
        [HttpPost]
        [EnableRateLimiting("api")]
        public async Task<IActionResult> CreateEmployee(
            CreateEmployeeRequest request)
        {
            var employee =
                await _employeeService.CreateEmployeeAsync(request);

            return Ok(
                new ApiResponse<Employee>(
                    true,
                    "Employee created successfully",
                    employee
                )
            );
        }


        // =========================================
        // PUT: api/employee/1
        // =========================================
        [Authorize]
        [HttpPut("{id}")]
        [EnableRateLimiting("api")]
        public async Task<IActionResult> UpdateEmployee(
            int id,
            UpdateEmployeeRequest request)
        {
            var employee =
                await _employeeService.UpdateEmployeeAsync(
                    id,
                    request
                );

            if (employee == null)
            {
                return NotFound(
                    new ApiResponse<object>(
                        false,
                        "Employee not found"
                    )
                );
            }

            return Ok(
                new ApiResponse<Employee>(
                    true,
                    "Employee updated successfully",
                    employee
                )
            );
        }


        // =========================================
        // DELETE: api/employee/1
        // =========================================
        [Authorize]
        [HttpDelete("{id}")]
        [EnableRateLimiting("api")]
        public async Task<IActionResult> DeleteEmployee(int id)
        {
            var deleted =
                await _employeeService.DeleteEmployeeAsync(id);

            if (!deleted)
            {
                return NotFound(
                    new ApiResponse<object>(
                        false,
                        "Employee not found"
                    )
                );
            }

            return Ok(
                new ApiResponse<object>(
                    true,
                    "Employee deleted successfully"
                )
            );
        }
    }
}