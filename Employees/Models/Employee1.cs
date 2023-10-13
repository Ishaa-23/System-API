namespace Employees.Models
{
    public class Employee1
    {
        public int Id { get; set; } 
        public string Name { get; set; }
        public int Age { get; set; }
        public string Role { get; set; } = "Employee";
        public bool isDeleted {  get; set; } = false;
        public bool isActive { get; set; }  = true; 
        public bool isPermission {  get; set; } = true;    
        
      
    }
}
