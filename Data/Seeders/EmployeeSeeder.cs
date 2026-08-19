using MyFirstApi.Models;

namespace MyFirstApi.Data.Seeders
{
    public class EmployeeSeeder
    {
        private readonly AppDbContext _context;

        public EmployeeSeeder(AppDbContext context)
        {
            _context = context;
        }

        public async Task SeedAsync(int count)
        {
            const int batchSize = 5000;

            var random = new Random();

            string[] firstNames =
            {
                "Rahim",
                "Karim",
                "Hasan",
                "Jamal",
                "Kamal",
                "Rakib",
                "Sakib",
                "Tanvir",
                "Fahim",
                "Nayeem"
            };

            string[] lastNames =
            {
                "Ahmed",
                "Hossain",
                "Khan",
                "Rahman",
                "Islam",
                "Chowdhury"
            };

            string[] departments =
            {
                "IT",
                "HR",
                "Finance",
                "Marketing",
                "Sales",
                "Operations"
            };

            for (int batch = 0; batch < count; batch += batchSize)
            {
                int currentBatchSize =
                    Math.Min(batchSize, count - batch);

                var employees =
                    new List<Employee>(currentBatchSize);

                for (int i = 1; i <= currentBatchSize; i++)
                {
                    int number = batch + i;

                    string firstName =
                        firstNames[random.Next(firstNames.Length)];

                    string lastName =
                        lastNames[random.Next(lastNames.Length)];

                    employees.Add(new Employee
                    {
                        Name = $"{firstName} {lastName} {number}",

                        Email =
                            $"{firstName.ToLower()}.{lastName.ToLower()}{number}@example.com",

                        Phone =
                            $"017{random.Next(10000000, 99999999)}",

                        Department =
                            departments[random.Next(departments.Length)]
                    });
                }

                await _context.Employees.AddRangeAsync(employees);

                await _context.SaveChangesAsync();

                _context.ChangeTracker.Clear();

                Console.WriteLine(
                    $"{Math.Min(batch + batchSize, count)} / {count}"
                );
            }
        }
    }
}