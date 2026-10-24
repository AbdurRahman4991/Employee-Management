using Microsoft.EntityFrameworkCore;
using MyFirstApi.Models;

namespace MyFirstApi.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<Employee> Employees { get; set; }
        public DbSet<User> Users {get; set;}
        public DbSet<Department> Departments { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Employee>()
                .HasIndex(x => x.Email)
                .IsUnique();

            modelBuilder.Entity<Employee>()
                .HasOne(e => e.Department)
                .WithMany(d => d.Employees)
                .HasForeignKey(e => e.DepartmentId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }


//     protected override void OnModelCreating(ModelBuilder modelBuilder)
// {
//     modelBuilder.Entity<OrganizationUnit>()
//         .HasOne(x => x.Parent)
//         .WithMany(x => x.Children)
//         .HasForeignKey(x => x.ParentId)
//         .OnDelete(DeleteBehavior.Restrict);
// }
}