using FluentValidation;
using JelleSmart.ExamSystem.Core.Interfaces.Repositories;
using JelleSmart.ExamSystem.Core.ViewModels;

namespace JelleSmart.ExamSystem.Service.Validators
{
    public class SubjectGradeValidator : AbstractValidator<SubjectGradeViewModel>
    {
        public SubjectGradeValidator(ISubjectGradeRepository subjectGradeRepository)
        {
            RuleFor(sg => sg.SubjectId)
                .NotEmpty().WithMessage("Subject selection is required");

            RuleFor(sg => sg.GradeId)
                .NotEmpty().WithMessage("Grade selection is required");
        }
    }
}
