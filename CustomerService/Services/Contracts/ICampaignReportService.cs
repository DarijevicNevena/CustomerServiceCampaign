namespace CustomerService.Services.Contracts
{
    public interface ICampaignReportService
    {
        Task<string> GenerateCampaignPurchasesReport(int campaignId);
    }
}