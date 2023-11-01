using Employees.Data;
using Employees.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Employees.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmpRoleController : ControllerBase
    {
        private readonly DataContext _context;
        private IConfiguration configuration;
        public EmpRoleController(DataContext dataContext, IConfiguration iConfig)
        {
            _context = dataContext;
            configuration = iConfig;
        }

        [HttpGet("Show-roles"),Authorize(Roles="Supervisor")]
        public async Task<ActionResult> ShowRoles()
        {
            return Ok(_context.Roles.ToList());
        }
        [HttpGet("Show-EmpRoles"),Authorize(Roles ="Supervisor,Manager")]
        public async Task<ActionResult> ShowEmpRoles()
        {
            return Ok(_context.EmpRoles.ToList());
        }

        [HttpPost("Add-roles"),Authorize(Roles="Supervisor")]
        public async Task<ActionResult> Addroles(Roles role)
        {
            _context.Roles.Add(role);
            await _context.SaveChangesAsync();
            return Ok("Created");
        }
        [HttpPost("Add-EmpRole"),Authorize(Roles ="Supervisor,Manager")]
        public async Task<ActionResult> AddUserRoles(string name,UserRoles role)
        {
            List<Employee1> results = await _context.Employees.ToListAsync();
            Employee1 e = results.FirstOrDefault(x => x.Name == name);
            if(e==null)
            {
                return NotFound("Employee of this id doesnt exist");
            }
            else
            {
                bool decision = _context.EmpRoles.Any(x => x.Emp_Id == e.Emp_Id && x.Role_Id==(int)role);
                if(decision==true)
                {
                    return BadRequest("Employee already has this role");
                }
                else
                {
                    EmpRole emprole = new EmpRole { 
                    Emp_Id = e.Emp_Id,  
                    Role_Id= (int)role
                    };
                    _context.EmpRoles.Add(emprole);
                    await _context.SaveChangesAsync();
                    return Ok("Emp-Role added.");
                }
            }
           
        }

        [HttpDelete("Delete-EmpRole"),Authorize(Roles ="Supervisor")]
        public async Task<ActionResult> DelSingleEmpRole(string name,UserRoles role)
        {
            List<Employee1> results = await _context.Employees.ToListAsync();
            Employee1 e = results.FirstOrDefault(x => x.Name == name);
            if (e == null) //to check if this name emp exists
            {
                return NotFound("Employee of this id doesnt exist");
            }
            else
            {
                bool decision = _context.EmpRoles.Any(x => x.Emp_Id == e.Emp_Id && x.Role_Id == (int)role); // to check if this emp has this role
                if(decision==false)
                {
                    return NotFound("Employee doesnt have this role");
                }
                else //we have emp with the same role_id
                {
                    List<EmpRole> erlist = await _context.EmpRoles.ToListAsync();
                    EmpRole emprole = erlist.FirstOrDefault(x=>x.Emp_Id==e.Emp_Id && x.Role_Id==(int)role);
                   _context.EmpRoles.Remove(emprole);
                    await _context.SaveChangesAsync();
                    return Ok("Deleted");
                    
                }
            }
        }

        [HttpDelete("Delete-All-EmpRoles"),Authorize(Roles ="Supervisor")]
        public async Task<ActionResult> DelEmpRoles()
        {
            List<EmpRole> results = new List<EmpRole>(await _context.EmpRoles.ToListAsync());
            if (results.Count == 0)
            {
                return BadRequest("No emp-roles to be deleted.");
            }
            else
            {
                await _context.Database.ExecuteSqlRawAsync("TRUNCATE TABLE [EmpRoles]");
                return Ok("Deleted all emp-roles.");
            }
        }
       
    }
}
