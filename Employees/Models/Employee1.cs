using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Employees.Models
{
    public class Employee1
    {
        [Key]
        public int Emp_Id { get; set; }
        
        public string Name { get; set; }
        public int Age { get; set; }
       
        public float Salary { get; set; }
        public string Department { get; set; }
        [JsonIgnore]
        [DefaultValue(false)]
        public bool isDeleted {  get; set; }
        [JsonIgnore]
        [DefaultValue(true)]   
        public bool isActive { get; set; }

        [JsonIgnore]
        [DefaultValue(true)]
        public bool isPermission {  get; set; }
        
        public string LastModified { get; set; }
        [JsonIgnore]
        public int Permission_Counter {  get; set; }   
       
    }
}
