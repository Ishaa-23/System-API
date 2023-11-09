using Employees.Data;
using Employees.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Linq;
using System.Diagnostics.Eventing.Reader;

namespace Employees.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeeController : ControllerBase
    {
       

        private readonly DataContext _context;
        private IConfiguration configuration;
        public EmployeeController(DataContext dataContext, IConfiguration iConfig) 
        { 
            _context= dataContext;
            configuration = iConfig;
        }

        public static User user = new User();
        public string token;
        [HttpPost("Register")]
        public ActionResult<User> Register(UserDto request, string role)
        {
            string passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);
            user.Username = request.UserName;
            user.PasswordHash = passwordHash;
            user.Role = role;
            return Ok(user);
        }
        [HttpGet("Login")]
        public ActionResult<User> Login(string Username,string Password)
        {
            if (user.Username != Username)
            {
                return BadRequest("User not found");
            }
            if (!BCrypt.Net.BCrypt.Verify(Password, user.PasswordHash))
            {
                return BadRequest("Wrong password");
            }

            token = CreateToken(user);
            return Ok(token);
        }

       
        [HttpGet("Get-permission-counter"), Authorize(Roles = "Supervisor,Manager")]
        public async Task<ActionResult> GetPermissionCounter(string name)
        {
            var emp = _context.Employees.Where(x => x.Name == name).FirstOrDefault();
            if (emp == null)
            {
                return NotFound("No employee of such name exists");
            }
            else
            {
                return Ok("Permission counter is-" + emp.Permission_Counter);
            }
        }

        [HttpGet("Display-Info"),Authorize(Roles="Supervisor")]
        public async Task<ActionResult<List<Employee1>>> GetEmpInfo(UserRoles role, string name)
        {
            List<Employee1> results = await _context.Employees.ToListAsync();
            Employee1 emp = results.FirstOrDefault(x => x.Name == name);
            if (emp == null)
            {
                return NotFound("Employee of this name doesnt exist");
            }
            else if (emp.isPermission == false)
            {
                return BadRequest("No operation allowed.");
            }
            else
            {

                if ((int)role == 3)
                {

                    return Ok(emp);

                }
                else if ((int)role == 2)
                {
                    bool decision = _context.EmpRoles.Any(x => x.Emp_Id == emp.Emp_Id && x.Role_Id == 2);
                    if (decision == false)
                    {
                        return NotFound("Doesnt match role");
                    }
                    else
                    {
                        List<Employee1> sameTeam = results.Where(x => x.Department == emp.Department).ToList(); //only same team
                        return Ok(sameTeam.Select(x => new { x.Emp_Id, x.Age, x.Name, x.Department, x.Salary }));
                    }
                }


                else if ((int)role == 1)
                {
                    bool decision = _context.EmpRoles.Any(x => x.Emp_Id == emp.Emp_Id && x.Role_Id == 1);
                    if (decision == false)
                    {
                        return NotFound("Doesnt match role");
                    }
                    else
                    {
                        return Ok(_context.Employees.ToList());
                    }

                }
                else
                {
                    return BadRequest();
                }
            }


        }


        [HttpPost("Add-Employees"),Authorize(Roles="Supervisor,Manager")]
        public async Task<ActionResult> AddEmp(Employee1 emp)
        {
            DateTime dateTime = DateTime.Now; // Replace with your DateTime value

            string formattedDate = dateTime.ToString("MMMM dd, yyyy HH:mm:ss tt");
            var newEmp = new Employee1 {
                Name = emp.Name,
                Age = emp.Age,
                Salary = emp.Salary,
                Department = emp.Department,
                LastModified = formattedDate,
                isActive= true,
                isPermission=true,
                isDeleted=false

            };
            _context.Employees.Add(newEmp);
            await _context.SaveChangesAsync();
            return Ok("Created.");

        }
       
        
      

      
        [HttpPut("Update"), Authorize(Roles = "Supervisor,Manager")]
        public async Task<ActionResult<List<Employee1>>> Update(Employee1 newEmp)
        {
            var emp = await _context.Employees.FindAsync(newEmp.Emp_Id);
            if(emp==null)
            {
                return BadRequest("Employee doesnt exist.");
            }
            else if(emp.isPermission==false)
            {
                return NotFound("No operation allowed.");
            }
            else
            {
                DateTime dateTime = DateTime.Now; // Replace with your DateTime value

                string formattedDate = dateTime.ToString("MMMM dd, yyyy HH:mm:ss tt");
                emp.Age = newEmp.Age;
                emp.Name= newEmp.Name;
               // emp.isDeleted = newEmp.isDeleted;
                //emp.isActive = newEmp.isActive;
                //emp.isPermission= newEmp.isPermission;
                emp.Salary = newEmp.Salary;
                emp.Department = newEmp.Department; 
               emp.LastModified = formattedDate;

                await _context.SaveChangesAsync();
                return Ok("Updated successfully.");
            }

        }

        [HttpPut("Soft-delete"),Authorize(Roles ="Supervisor,Manager")]
        public async Task<ActionResult> SoftDelete(int id)
        {
            var emp = await _context.Employees.FindAsync(id);
            if (emp == null)
            {
                return BadRequest("No employee of this id exists.");
            }
            else if(emp.isPermission==false)
            {
                return BadRequest("No operation allowed");
            }
            else
            {
                DateTime dateTime = DateTime.Now; // Replace with your DateTime value

                string formattedDate = dateTime.ToString("MMMM dd, yyyy HH:mm:ss tt");
                emp.isActive= false;
                emp.LastModified = formattedDate;
                await _context.SaveChangesAsync();
                return Ok("Soft-deleted");

            }
        }

      

       
        [HttpPut("Permission-grant"), Authorize(Roles = "Supervisor,Manager")]
        public async Task<ActionResult> Grant(int id)
        {
            var emp = await _context.Employees.FindAsync(id);
            if (emp == null)
            {
                return NotFound("No employee of this id exists.");
            }
            else
            {
                emp.Permission_Counter = emp.Permission_Counter + 1;
                emp.isPermission = true;
                DateTime dateTime = DateTime.Now; // Replace with your DateTime value
                string formattedDate = dateTime.ToString("MMMM dd, yyyy HH:mm:ss tt");
                emp.LastModified = formattedDate;
                await _context.SaveChangesAsync();
                return Ok(await _context.Employees.ToListAsync());
            }
        }
      

        [HttpPut("Permission-revoke"), Authorize(Roles = "Supervisor,Manager")]
        public async Task<ActionResult> Revoke(int id)
        {
            var emp = await _context.Employees.FindAsync(id);
            if (emp == null)
            {
                return NotFound("No employee of this id exists.");
            }
            else
            {
                emp.Permission_Counter = emp.Permission_Counter + 1;
                emp.isPermission = false;
                DateTime dateTime = DateTime.Now; // Replace with your DateTime value
                string formattedDate = dateTime.ToString("MMMM dd, yyyy HH:mm:ss tt");
                emp.LastModified = formattedDate;
                await _context.SaveChangesAsync();
                return Ok(await _context.Employees.ToListAsync());
            }
        }
        [HttpDelete("Delete"), Authorize(Roles = "Supervisor,Manager")]
        public async Task<ActionResult> Delete(int id)
        {
            var emp = await _context.Employees.FindAsync(id);
            if (emp == null)
            {
                return BadRequest("No employee of this id exists.");
            }
            else if (emp.isPermission == false)
            {
                return BadRequest("No operation allowed");
            }
            else
            {
                _context.Employees.Remove(emp);
                await _context.SaveChangesAsync();
                return Ok("Deleted");
            }
        }

        [HttpDelete("DeleteAll"),Authorize(Roles="Supervisor")]
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
       
      

       [HttpPost]
        public string CreateToken(User user)
        {

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
                 configuration.GetSection("AppSettings:Token").Value!));

            var roles = new[] { "Supervisor", "Manager", "Employee" };
            var roleClaims = new[] { new Claim(ClaimTypes.Role, user.Role) };
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(roleClaims),
                Expires = DateTime.UtcNow.AddHours(1), // Token expiration time
                SigningCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature)
            };
            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);

            // Serialize the token to a string
            var tokenString = tokenHandler.WriteToken(token);
            return tokenString;

        }
    }
}
