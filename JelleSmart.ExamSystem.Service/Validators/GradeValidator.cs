using FluentValidation;
using JelleSmart.ExamSystem.Core.Interfaces.Repositories;
using JelleSmart.ExamSystem.Core.ViewModels;

namespace JelleSmart.ExamSystem.Service.Validators
{
    public class GradeValidator : AbstractValidator<GradeViewModel>
    {
        public GradeValidator(IGradeRepository gradeRepository)
        {
            RuleFor(g => g.Name)
                .NotEmpty().WithMessage("Grade name is required")
                .MaximumLength(50).WithMessage("Grade name cannot exceed 50 characters");

            RuleFor(g => g.Level)
                .InclusiveBetween(1, 12).WithMessage("Grade level must be between 1 and 12");
        }
    }
}
