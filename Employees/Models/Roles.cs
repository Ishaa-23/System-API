using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Employees.Models
{
    public enum UserRoles
    {
        Supervisor=1,
        Manager=2,
        Employee=3
    }
    public class Roles
    {
        

        [Key]
        public int Role_Id {  get; set; }   
        public string RoleName { get; set; }   
       
    }
}
