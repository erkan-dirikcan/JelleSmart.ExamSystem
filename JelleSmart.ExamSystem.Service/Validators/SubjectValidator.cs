using FluentValidation;
using JelleSmart.ExamSystem.Core.Interfaces.Repositories;
using JelleSmart.ExamSystem.Core.ViewModels;

namespace JelleSmart.ExamSystem.Service.Validators
{
    public class SubjectValidator : AbstractValidator<SubjectViewModel>
    {
        public SubjectValidator(ISubjectRepository subjectRepository)
        {
            RuleFor(s => s.Name)
                .NotEmpty().WithMessage("Subject name is required")
                .MaximumLength(100).WithMessage("Subject name cannot exceed 100 characters");

            RuleFor(s => s.Description)
                .MaximumLength(500).WithMessage("Description cannot exceed 500 characters");

            RuleFor(s => s.IconClass)
                .MaximumLength(100).WithMessage("Icon class cannot exceed 100 characters");
        }
    }
}
