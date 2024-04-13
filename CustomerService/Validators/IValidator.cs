using FluentValidation.Results;

namespace CustomerService.Validators
{
    public interface IValidator<T>
    {
        Task<ValidationResult> ValidateAsync(T entity);
    }
}
