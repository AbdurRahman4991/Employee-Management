using MyFirstApi.Data.Seeders;

namespace MyFirstApi.Commands
{
    public class SeedEmployeesCommand
    {
        private readonly EmployeeSeeder _employeeSeeder;

        public SeedEmployeesCommand(
            EmployeeSeeder employeeSeeder)
        {
            _employeeSeeder = employeeSeeder;
        }

        public async Task ExecuteAsync(int count)
        {
            Console.WriteLine(
                $"Seeding {count} employees..."
            );

            await _employeeSeeder.SeedAsync(count);

            Console.WriteLine(
                "Employee seeding completed."
            );
        }
    }
}