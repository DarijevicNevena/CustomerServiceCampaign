using System.ComponentModel.DataAnnotations;

namespace CustomerService.Models.ModelDto
{
    public class CampaignWriteDto
    {
        [Required(ErrorMessage = "Campaign Name is required.")]
        public string? CampaignName { get; set; }

        [Required(ErrorMessage = "Start Date is required.")]
        public DateTime StartDate { get; set; }
    }
}
