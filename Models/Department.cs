namespace MyFirstApi.Models
{
    public class Department
    {
        public int Id { get; set; }

        public string Name { get; set; } = "";

        // One Department → Many Employees
        public ICollection<Employee> Employees { get; set; }
            = new List<Employee>();
    }
}