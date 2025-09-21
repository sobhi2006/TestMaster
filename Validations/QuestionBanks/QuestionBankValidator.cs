using FluentValidation;
using TestMaster.DTOs.Requests.QuestionBank;
using TestMaster.Enums.Question;

public class QuestionBankValidator : AbstractValidator<QuestionBankRequest>
{
    public QuestionBankValidator()
    {
        RuleFor(p => p.Difficulty)
                .NotNull()
                .WithMessage("Difficulty level is required.")
                .IsInEnum()
                .WithMessage($@"Difficult level must be in {QuestionDifficultyLevel.Easy}
                , {QuestionDifficultyLevel.Medium},
                {QuestionDifficultyLevel.Hard},or {QuestionDifficultyLevel.Expert}.");

        RuleFor(p => p.Title)
               .NotNull()
               .WithMessage("Title can't be null")
               .NotEmpty()
               .WithMessage("Title can't be empty")
               .Length(3, 20)
               .WithMessage("Title can't be less than 3 character or over 20 character");

        RuleFor(p => p.Description)
                .NotNull()
                .WithMessage("Description can't be null")
                .NotEmpty()
                .WithMessage("Description can't be empty")
                .Length(3, 100)
                .WithMessage("Description can't be less than 3 character or over 100 character");

        RuleFor(p => p.QuestionIDs)
                .NotNull()
                .WithMessage("Questions of QuestionBank can't be null");

        RuleForEach(p => p.QuestionIDs)
                .NotNull()
                .WithMessage("Question in QuestionBank can't be null");
    }
}