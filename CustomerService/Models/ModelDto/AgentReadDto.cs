
using System.ComponentModel.DataAnnotations;

namespace CustomerService.Models.ModelDto
{
    public class AgentReadDto
    {
        [Required]
        public string? FirstName { get; set; }
        [Required]
        public string? LastName { get; set; }
        [Required]
        public string? Email { get; set; }
    }
}
