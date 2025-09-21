using FluentValidation;
using TestMaster.DTOs.Requests.Tests;
namespace TestMaster.Validations.Tests;

public class TestValidator : AbstractValidator<TestRequest>
{
    public TestValidator()
    {
        RuleFor(p => p.Title)
            .NotEmpty()
            .WithMessage("Title can't be empty.")
            .Length(3, 20)
            .WithMessage("Length of Title can't be less 3 character and over 20 character.");

        RuleFor(p => p.Description)
            .NotEmpty()
            .WithMessage("Description can't be empty.")
            .Length(10, 100)
            .WithMessage("Length of Description can't be less 10 character and over 100 character.");

        RuleFor(p => p.Duration)
            .NotNull()
            .WithMessage("Duration is required")
            .InclusiveBetween(new TimeSpan(0, 1, 0), new TimeSpan(10, 0, 0))
            .WithMessage("Duration can't be less 1 Minute and over 10 Hour.");

        RuleFor(p => p.QuestionBankId)
            .NotNull()
            .WithMessage("QuestionBankId can't be null")
            .Must(v => Guid.TryParse(v.ToString(), out _))
            .WithMessage("QuestionBankId must be Guid.");
    }
}