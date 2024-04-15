using System.ComponentModel.DataAnnotations;

namespace CustomerService.Models.ModelDto
{
    public class PurchaseReadDto
    {
        [Required]
        public int AgentId { get; set; }

        [Required]
        public int CustomerId { get; set; }

        [Required]
        public int CampaignId { get; set; }

        [Required]
        public int Price { get; set; }

        [Required]
        public int Discount { get; set; }

        [Required]
        public int PriceWithDiscount { get; set; }

        [Required]
        public DateTime Date { get; set; }
    }
}
