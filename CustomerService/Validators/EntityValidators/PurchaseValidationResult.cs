using FluentValidation.Results;

namespace CustomerService.Validators.EntityValidators
{
    public class PurchaseValidationResult
    {
        public ValidationResult ValidationResult { get; set; }
        public int? CampaignId { get; set; }

        public PurchaseValidationResult()
        {
            ValidationResult = new ValidationResult();
        }

        public bool IsValid => ValidationResult.Errors.Count == 0;
    }
}
