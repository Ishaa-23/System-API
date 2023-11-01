using System.Text.Json.Serialization;

namespace Employees.Models
{
    public class EmpRole
    {
        public int EmpRole_Id { get; set; } 
        public int Emp_Id { get; set; }
        public int Role_Id { get; set; }
        [JsonIgnore]
       public Employee1 Employee1 { get; set; }
        [JsonIgnore]
        public Roles Role {  get; set; }    
    }
}
