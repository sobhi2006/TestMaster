using FluentValidation;
using TestMaster.DTOs.Requests.Answers;

namespace TestMaster.Validations.Answers;
public class AnswerQuestionValidator : AbstractValidator<AnswerQuestionRequest>
{
    public AnswerQuestionValidator()
    {
        RuleFor(p => p.AnswerText)
                .NotNull()
                .WithMessage("Answer Text can't be null.")
                .NotEmpty()
                .WithMessage("Answer Text can't be empty.");

        RuleFor(p => p.IsCorrect)
                .NotNull()
                .WithMessage("Is correct can't be null")
                .Must(v => bool.TryParse(v.ToString(), out _))
                .WithMessage("Is Correct must be true or false.");
    }
}