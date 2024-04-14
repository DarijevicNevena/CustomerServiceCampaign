using System.ComponentModel.DataAnnotations;

namespace CustomerService.Models.ModelDto
{
    public class CampaignReadDto
    {
        [Required]
        public string CampaignName { get; set; }
        [Required]
        public DateTime StartDate { get; set; }
        [Required]
        public DateTime EndDate { get; set; }
    }
}
