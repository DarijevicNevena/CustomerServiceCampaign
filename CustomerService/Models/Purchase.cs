using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CustomerService.Models
{
    public class Purchase
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

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

        public Agent Agent { get; set; }
        public Campaign Campaign { get; set; }
    }
}
