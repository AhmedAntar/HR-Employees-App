using Microsoft.EntityFrameworkCore;
using EmployeesApp.Models;

namespace EmployeesApp.Models
{
    public class HRDatabaseContext : DbContext
    {
        public DbSet<Department> Departments { set; get; }
        public DbSet<Employee> Employees { set; get; }

        protected override void OnConfiguring(DbContextOptionsBuilder options) => 

            options.UseSqlServer("Data Source=(localdb)\\ProjectModels;Initial Catalog=Employees;Integrated Security=True;");
    }
}
