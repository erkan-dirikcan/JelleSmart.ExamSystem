using FluentValidation;
using JelleSmart.ExamSystem.Core.Interfaces.Repositories;
using JelleSmart.ExamSystem.Core.ViewModels;

namespace JelleSmart.ExamSystem.Service.Validators
{
    public class UnitValidator : AbstractValidator<UnitViewModel>
    {
        public UnitValidator(IUnitRepository unitRepository)
        {
            RuleFor(u => u.Name)
                .NotEmpty().WithMessage("Unit name is required")
                .MaximumLength(200).WithMessage("Unit name cannot exceed 200 characters");

            RuleFor(u => u.Order)
                .GreaterThan(0).WithMessage("Order must be greater than 0");

            RuleFor(u => u.GradeId)
                .NotEmpty().WithMessage("Grade selection is required");
        }
    }
}
