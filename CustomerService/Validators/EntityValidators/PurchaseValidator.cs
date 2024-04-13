using CustomerService.Models;
using CustomerService.Services;
using FluentValidation;
using FluentValidation.Results;

namespace CustomerService.Validators.EntityValidators
{
    public class PurchaseValidator : AbstractValidator<Purchase>, IValidator<Purchase>
    {
        private readonly IAgentService _agentService;
        //private readonly ICustomerService _customerService;
        private readonly ICampaignService _campaignService;

        public PurchaseValidator(
        IAgentService agentService,
        ICampaignService campaignService) : this()
        {
            _agentService = agentService;
            _campaignService = campaignService;

            SetupRules();
        }

        public PurchaseValidator()
        {
            SetupRules();
        }

        private void SetupRules()
        {
            RuleFor(purchase => purchase.AgentId)
                .NotEmpty().WithMessage("Agent ID is required.");

            RuleFor(purchase => purchase.CustomerId)
                .NotEmpty().WithMessage("Customer ID is required.");

            RuleFor(purchase => purchase.CampaignId)
                .NotEmpty().WithMessage("Campaign ID is required.");

            RuleFor(purchase => purchase.Price)
                .NotEmpty().WithMessage("Price is required.")
                .GreaterThan(0).WithMessage("Price must be greater than 0.");

            RuleFor(purchase => purchase.Discount)
                .NotEmpty().WithMessage("Discount is required.")
                .GreaterThan(0).WithMessage("Discount must be greater than 0.");

            RuleFor(purchase => purchase.Date)
                .NotEmpty().WithMessage("Date is required.");

            RuleFor(purchase => purchase.Discount).LessThan(purchase => purchase.Price).WithMessage("Discount must be less than Price.");
        }

        public async Task<ValidationResult> ValidateAsync(Purchase purchase)
        {
            var validationResult = new ValidationResult();

            var agent = await _agentService.GetAgentByIdAsync(purchase.AgentId);
            if (agent == null)
            {
                validationResult.Errors.Add(new ValidationFailure("AgentId", "Agent does not exist."));
            }

            /*
            var customerExists = _customerService.CustomerExists(purchase.CustomerId);
            if (!customerExists)
            {
                validationResult.Errors.Add(new ValidationFailure("CustomerId", "Customer does not exist."));
            }
            */

            var campaign = await _campaignService.GetCampaignByIdAsync(purchase.CampaignId);
            if (campaign == null)
            {
                validationResult.Errors.Add(new ValidationFailure("CampaignId", "Campaign does not exist."));
            }


            return validationResult;
        }
    }
}
