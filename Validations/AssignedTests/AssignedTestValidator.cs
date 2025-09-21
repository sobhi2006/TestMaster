using FluentValidation;
namespace TestMaster.DTOs.Requests.AssignedTests;

public class AssignedTestValidator : AbstractValidator<AssignedTestRequest>
{
    public AssignedTestValidator()
    {
        RuleFor(p => p.TestId)
                .NotNull()
                .WithMessage("TestId can't be null");

        RuleFor(p => p.AssignedUserId)
                .NotNull()
                .WithMessage("UserId can't be null");
    }
}