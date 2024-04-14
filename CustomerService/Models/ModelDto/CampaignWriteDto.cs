using System.ComponentModel.DataAnnotations;

namespace CustomerService.Models.ModelDto
{
    public class CampaignWriteDto
    {
        [Required]
        public string CampaignName { get; set; }
        [Required]
        public DateTime StartDate { get; set; }
    }
}
