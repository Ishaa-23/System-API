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
        public DbSet<Roles> Roles { get; set; }
        public DbSet<EmpRole> EmpRoles { get; set; }    
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<EmpRole>()
             .HasKey(ur => ur.EmpRole_Id);

            modelBuilder.Entity<EmpRole>()
        .HasOne(ur => ur.Employee1)
        .WithMany()
        .HasForeignKey(ur => ur.Emp_Id);
            modelBuilder.Entity<EmpRole>()
       .HasOne(ur => ur.Role)
       .WithMany()
       .HasForeignKey(ur => ur.Role_Id);

        }



    }
}
