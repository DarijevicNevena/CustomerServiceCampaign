using System.ComponentModel.DataAnnotations;

namespace CustomerService.Models.ModelDto
{
    public class PurchaseWriteDto
    {
        [Required]
        public int CustomerId { get; set; }
        [Required]
        public string CampaignName { get; set; }
        [Required]
        public int Price { get; set; }
        [Required]
        public int Discount { get; set; }
        public int AgentId { get; internal set; }
    }
}
