using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyFirstApi.Data;
using MyFirstApi.Models;
using Microsoft.AspNetCore.Authorization;

namespace MyFirstApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EmployeeController : ControllerBase
    {
        private readonly AppDbContext _context;

        public EmployeeController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/employee
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetEmployees()
        {
            try
            {
                var employees = await _context.Employees.ToListAsync();

                return Ok(new
                {
                    message = "Employees fetched successfully",
                    data = employees
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = "Something went wrong",
                    error = ex.Message
                });
            }
        }

        // GET: api/employee/1
        [HttpGet("{id}")]
        public async Task<IActionResult> GetEmployee(int id)
        {
            var employee = await _context.Employees
                .FirstOrDefaultAsync(x => x.Id == id);

            if (employee == null)
            {
                return NotFound(new
                {
                    message = "Employee not found"
                });
            }

            return Ok(employee);
        }

        // POST: api/employee
        [HttpPost]
        public async Task<IActionResult> CreateEmployee(Employee employee)
        {
            _context.Employees.Add(employee);

            await _context.SaveChangesAsync();

            return CreatedAtAction(
                nameof(GetEmployee),
                new { id = employee.Id },
                employee
            );
        }

        // PUT: api/employee/1
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateEmployee(
            int id,
            Employee employee)
        {
            var existingEmployee = await _context.Employees
                .FirstOrDefaultAsync(x => x.Id == id);

            if (existingEmployee == null)
            {
                return NotFound(new
                {
                    message = "Employee not found"
                });
            }

            existingEmployee.Name = employee.Name;
            existingEmployee.Email = employee.Email;
            existingEmployee.Phone = employee.Phone;
            existingEmployee.Department = employee.Department;

            await _context.SaveChangesAsync();

            return Ok(existingEmployee);
        }

        // DELETE: api/employee/1
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEmployee(int id)
        {
            var employee = await _context.Employees
                .FirstOrDefaultAsync(x => x.Id == id);

            if (employee == null)
            {
                return NotFound(new
                {
                    message = "Employee not found"
                });
            }

            _context.Employees.Remove(employee);

            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "Employee deleted successfully"
            });
        }
    }
}