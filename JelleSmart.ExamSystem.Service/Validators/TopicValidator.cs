using FluentValidation;
using JelleSmart.ExamSystem.Core.Interfaces.Repositories;
using JelleSmart.ExamSystem.Core.ViewModels;

namespace JelleSmart.ExamSystem.Service.Validators
{
    public class TopicValidator : AbstractValidator<TopicViewModel>
    {
        public TopicValidator(ITopicRepository topicRepository)
        {
            RuleFor(t => t.Name)
                .NotEmpty().WithMessage("Topic name is required")
                .MaximumLength(200).WithMessage("Topic name cannot exceed 200 characters");

            RuleFor(t => t.Order)
                .GreaterThan(0).WithMessage("Order must be greater than 0");

            RuleFor(t => t.UnitId)
                .NotEmpty().WithMessage("Unit selection is required");

            RuleFor(t => t.GradeId)
                .NotEmpty().WithMessage("Grade selection is required");

            RuleFor(t => t.Code)
                .MaximumLength(50).WithMessage("Code cannot exceed 50 characters");
        }
    }
}
