using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SaveAttachmentAjax.Data;
using SaveAttachmentAjax.Models;
using System.IO;
using System.Threading.Tasks;

namespace SaveAttachmentAjax.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost("/api/employees")]
        public async Task<IActionResult> CreateEmployee([FromForm] EmployeeCreateDto employeeDto)
        {
            if (employeeDto == null || employeeDto.Photo == null || employeeDto.Photo.Length == 0)
            {
                return BadRequest("Employee data or photo is missing.");
            }

            var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Photos");
            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }

            var fileName = Path.GetFileName(employeeDto.Photo.FileName);
            var filePath = Path.Combine(uploadsFolder, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await employeeDto.Photo.CopyToAsync(stream);
            }

            var employee = new Employee
            {
                Name = employeeDto.Name,
                Phone = employeeDto.Phone,
                Email = employeeDto.Email,
                PhotoPath = fileName
            };

            _context.Employees.Add(employee);
            await _context.SaveChangesAsync();

            return Ok(employee);
        }

        [HttpGet("/api/get-employees")]
        public async Task<IActionResult> GetEmployees()
        {
            var employees = await _context.Employees.ToListAsync();
            return Ok(employees);
        }

        [HttpGet("/api/employees/{id}")]
        public async Task<IActionResult> GetEmployee(int id)
        {
            var employee = await _context.Employees.FindAsync(id);
            if (employee == null)
            {
                return NotFound();
            }
            return Ok(employee);
        }

        [HttpPut("/api/employees/{id}")]
        public async Task<IActionResult> UpdateEmployee(int id, [FromForm] EmployeeCreateDto employeeDto)
        {
            if (id != employeeDto.Id || employeeDto == null)
            {
                return BadRequest();
            }

            var employee = await _context.Employees.FindAsync(id);
            if (employee == null)
            {
                return NotFound();
            }

            // Update employee properties
            employee.Name = employeeDto.Name;
            employee.Phone = employeeDto.Phone;
            employee.Email = employeeDto.Email;

            // Handle photo upload
            if (employeeDto.Photo != null && employeeDto.Photo.Length > 0)
            {
                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Photos");
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                var fileName = Path.GetFileName(employeeDto.Photo.FileName);
                var filePath = Path.Combine(uploadsFolder, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await employeeDto.Photo.CopyToAsync(stream);
                }

                employee.PhotoPath = fileName; // Update with new photo name
            }

            _context.Employees.Update(employee);
            await _context.SaveChangesAsync();

            return Ok(employee);
        }

        [HttpDelete("/api/employees/{id}")]
        public async Task<IActionResult> DeleteEmployee(int id)
        {
            var employee = await _context.Employees.FindAsync(id);
            if (employee == null)
            {
                return NotFound();
            }

            _context.Employees.Remove(employee);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
