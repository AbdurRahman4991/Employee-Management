using Microsoft.EntityFrameworkCore;
using MyFirstApi.Data;
using MyFirstApi.DTOs.Employee;
using MyFirstApi.Models;
using MyFirstApi.Services.Interfaces;
using MyFirstApi.DTOs.Common;

namespace MyFirstApi.Services
{
    public class EmployeeService : IEmployeeService
    {
        private readonly AppDbContext _context;

        public EmployeeService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<PagedResponse<Employee>> GetEmployeesAsync(
        string? search,
        int page = 1,
        int pageSize = 10)
        {
            if (page < 1)
            {
                page = 1;
            }

            if (pageSize < 1)
            {
                pageSize = 10;
            }

            var query = _context.Employees
                .AsNoTracking()
                .AsQueryable();

            // Search
            if (!string.IsNullOrWhiteSpace(search))
            {
                search = search.Trim();

                query = query.Where(x =>
                    x.Name.Contains(search) ||
                    x.Email.Contains(search) ||
                    x.Phone.Contains(search) ||
                    x.Department.Contains(search)
                );
            }

            // Total records
            var totalRecords = await query.CountAsync();

            // Total pages
            var totalPages =
                (int)Math.Ceiling(
                    totalRecords / (double)pageSize
                );

            // Pagination
            var employees = await query
                .OrderBy(x => x.Id)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PagedResponse<Employee>
            {
                Items = employees,
                Page = page,
                PageSize = pageSize,
                TotalRecords = totalRecords,
                TotalPages = totalPages,
                HasNextPage = page < totalPages,
                HasPreviousPage = page > 1
            };
        }


        // GET BY ID
        public async Task<Employee?> GetEmployeeAsync(int id)
        {
            return await _context.Employees
                .FirstOrDefaultAsync(x => x.Id == id);
        }


        // CREATE
        public async Task<Employee> CreateEmployeeAsync(
            CreateEmployeeRequest request)
        {
            var employee = new Employee
            {
                Name = request.Name,
                Email = request.Email,
                Phone = request.Phone,
                Department = request.Department
            };

            _context.Employees.Add(employee);

            await _context.SaveChangesAsync();

            return employee;
        }


        // UPDATE
        public async Task<Employee?> UpdateEmployeeAsync(
            int id,
            UpdateEmployeeRequest request)
        {
            var employee = await _context.Employees
                .FirstOrDefaultAsync(x => x.Id == id);

            if (employee == null)
            {
                return null;
            }

            employee.Name = request.Name;
            employee.Email = request.Email;
            employee.Phone = request.Phone;
            employee.Department = request.Department;

            await _context.SaveChangesAsync();

            return employee;
        }


        // DELETE
        public async Task<bool> DeleteEmployeeAsync(int id)
        {
            var employee = await _context.Employees
                .FirstOrDefaultAsync(x => x.Id == id);

            if (employee == null)
            {
                return false;
            }

            _context.Employees.Remove(employee);

            await _context.SaveChangesAsync();

            return true;
        }
    }
}