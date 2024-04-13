using FluentValidation;
using FluentValidation.Results;

namespace CustomerService.Validators
{
    public class Validator<T> : IValidator<T>
    {
        private readonly AbstractValidator<T> _validator;

        public Validator(AbstractValidator<T> validator)
        {
            _validator = validator;
        }

        public Task<ValidationResult> ValidateAsync(T entity)
        {
            return _validator.ValidateAsync(entity);
        }
    }
}
