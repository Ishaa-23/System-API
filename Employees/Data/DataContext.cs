using Employees.Models;
using Microsoft.EntityFrameworkCore;

namespace Employees.Data
{
    public class DataContext: DbContext
    {
        public DataContext(DbContextOptions<DataContext> options): base(options)
        {

        }
        public DbSet<Employee1> Employees { get; set; }  
    }
}
