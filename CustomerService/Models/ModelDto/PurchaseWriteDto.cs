using System.ComponentModel.DataAnnotations;

namespace CustomerService.Models.ModelDto
{
    public class PurchaseWriteDto
    {
        [Required(ErrorMessage = "Customer Id is required.")]
        public int CustomerId { get; set; }

        [Required(ErrorMessage = "Campaign Name is required.")]
        public string? CampaignName { get; set; }

        [Required(ErrorMessage = "Price is required.")]
        public int Price { get; set; }

        [Required(ErrorMessage = "Discount is required.")]
        public int Discount { get; set; }

        public int AgentId { get; internal set; }
    }
}
