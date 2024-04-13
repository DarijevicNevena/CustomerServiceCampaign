using CustomerService.Models;
using CustomerService.Services.Contracts;
using System.Text;

namespace CustomerService.Services
{
    public class CampaignReportService : ICampaignReportService
    {
        private readonly ICampaignService _campaignService;
        private readonly IPurchaseService _purchaseService;

        public CampaignReportService(ICampaignService campaignService, IPurchaseService purchaseService)
        {
            _campaignService = campaignService;
            _purchaseService = purchaseService;
        }

        public async Task<string> GenerateCampaignPurchasesReport(int campaignId)
        {
            var campaign = await _campaignService.GetCampaignByIdAsync(campaignId);
            if (campaign == null)
            {
                throw new KeyNotFoundException("Campaign not found.");
            }

            //Check if campaign is still opened
            if(campaign.EndDate.Date > DateTime.UtcNow.Date)
            {
                throw new InvalidOperationException("Campaign is still open.");
            }
            // Check if current date is at least one month after campaign end
            TimeSpan timeDifference = DateTime.UtcNow.Date - campaign.EndDate.Date;
            int daysDifference = (int)timeDifference.TotalDays;
            if (daysDifference <= 30)
            {
                throw new InvalidOperationException("You can only obtain a report one month after the campaign end date.");
            }

            var purchases = await _purchaseService.GetPurchasesByCampaignAsync(campaignId);
            if (purchases == null || !purchases.Any())
            {
                throw new Exception("No purchase data found for the specified campaign.");
            }

            return ConvertToCsv(purchases);
        }

        private string ConvertToCsv(IEnumerable<Purchase> purchases)
        {
            var csvBuilder = new StringBuilder();
            csvBuilder.AppendLine("Id,AgentFirstName,AgentLastName,AgentEmail,CampaignName,Price,Discount,PriceWithDiscount,PurchaseDate,CustomerId");

            foreach (var purchase in purchases)
            {
                csvBuilder.AppendLine($"{purchase.Id}," +
                                      $"{purchase.Agent?.FirstName}," +
                                      $"{purchase.Agent?.LastName}," +
                                      $"{purchase.Agent?.Email}," +
                                      $"{purchase.Campaign?.CampaignName}," +
                                      $"{purchase.Price.ToString("F2")}," +
                                      $"{purchase.Discount.ToString("F2")}," +
                                      $"{purchase.PriceWithDiscount.ToString("F2")}," +
                                      $"{purchase.Date.ToString("MM/dd/yyyy hh:mm:ss tt")}," +
                                      $"{purchase.CustomerId}");
            }

            return csvBuilder.ToString();
        }
    }
}
