using Employees.Data;
using Employees.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Employees.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeeController : ControllerBase
    {
        private readonly DataContext _context;
        public EmployeeController(DataContext dataContext) 
        { 
            _context= dataContext;
        }
       
        [HttpGet]
        public async Task<ActionResult<List<Employee1>>> Get()
        {
            return Ok(await _context.Employees.ToListAsync());
        }
        [HttpPost("add")]  
        public async Task<ActionResult<List<Employee1>>> Add(Employee1 emp)
        {
            
            _context.Employees.Add(emp);
            await _context.SaveChangesAsync();
            return Ok(await _context.Employees.Select(x => new { x.Id, x.Name, x.Age, x.Role}).ToListAsync());
        }
        [HttpPut("update")]
        public async Task<ActionResult<List<Employee1>>> Update(Employee1 newEmp)
        {
            var emp = await _context.Employees.FindAsync(newEmp.Id);
            if(emp==null)
            {
                return BadRequest();
            }
            else
            {
                emp.Age = newEmp.Age;
                emp.Name= newEmp.Name;
                emp.Role = newEmp.Role;
                emp.isDeleted = newEmp.isDeleted;
                emp.isActive = newEmp.isActive;
                emp.isPermission= newEmp.isPermission;  
               
                await _context.SaveChangesAsync();
                return Ok(await _context.Employees.Select(x => new { x.Id, x.Name, x.Age,x.Role }).ToListAsync());
            }

        }

        [HttpPut("soft-delete")]
        public async Task<ActionResult<List<Employee1>>> SoftDelete(int id)
        {
            var emp = await _context.Employees.FindAsync(id);
            if(emp==null)
            {
                return BadRequest("No employee of this id.");
            }
            else
            {
                emp.isActive = false;
                await _context.SaveChangesAsync();
                return Ok(await _context.Employees.Select(x => new { x.Id, x.Name, x.Age, x.Role }).ToListAsync());
            }
           
        }


        [HttpDelete("delete")]
        public async Task<ActionResult> Delete(int id)
        {
            var emp = await _context.Employees.FindAsync(id);
            if (emp == null)
            {
                return BadRequest("No employee of this id exists.");
            }
            else
            {
                _context.Employees.Remove(emp);
                await _context.SaveChangesAsync();
                return Ok("Deleted");
            }
        }
        [HttpDelete("DeleteAll")]
        public async Task<ActionResult> Truncate()
        {
            List<Employee1> results = new List<Employee1>(await _context.Employees.ToListAsync());
            if(results.Count==0)
            {
                return BadRequest("No record to be deleted.");
            }
            else
            {
                await _context.Database.ExecuteSqlRawAsync("TRUNCATE TABLE [Employees]");
                return Ok("Deleted all employees.");
            }
        }
    }
}
