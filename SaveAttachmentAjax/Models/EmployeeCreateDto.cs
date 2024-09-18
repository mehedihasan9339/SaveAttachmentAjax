using Microsoft.AspNetCore.Http;

namespace SaveAttachmentAjax.Models
{
    public class EmployeeCreateDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public IFormFile Photo { get; set; }
    }
}
