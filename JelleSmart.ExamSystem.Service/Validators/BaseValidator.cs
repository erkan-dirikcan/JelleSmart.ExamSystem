using FluentValidation;

namespace JelleSmart.ExamSystem.Service.Validators
{
    public abstract class BaseValidator<T> : AbstractValidator<T> where T : class
    {
        protected BaseValidator()
        {
            // Common validation rules can be added here if needed
        }
    }
}
