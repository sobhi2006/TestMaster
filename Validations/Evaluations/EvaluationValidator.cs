
using FluentValidation;
using TestMaster.DTOs.Requests.Evaluations;

namespace TestMaster.Validations.Evaluations;
public class EvaluationValidator : AbstractValidator<EvaluationRequest>
{
    public EvaluationValidator()
    {
        RuleFor(p => p.Feedback)
                .NotNull()
                .WithMessage("Feedback can't be null")
                .NotEmpty()
                .WithMessage("Feedback can't be empty");

        RuleFor(p => p.Score)
                .NotNull()
                .WithMessage("Max score is required.")
                .GreaterThanOrEqualTo(0)
                .WithMessage("Mark must be greater than or equal 0.");

        RuleFor(p => p.TotalScore)
                .NotNull()
                .WithMessage("Total Score is required.")
                .GreaterThanOrEqualTo(1)
                .WithMessage("Total Score must be greater than or equal 1.");

        RuleFor(p => p.TestId)
                .NotNull()
                .WithMessage("TestId can't be null");

        RuleFor(p => p.EvaluationUserId)
                .NotNull()
                .WithMessage("UserId can't be null");
    }
}