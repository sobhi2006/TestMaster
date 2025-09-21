using FluentValidation;
using TestMaster.DTOs.Requests.Questions;
using TestMaster.Enums.Question;
using TestMaster.Validations.Answers;

namespace TestMaster.Validations.Questions;
public class QuestionValidator : AbstractValidator<QuestionRequest>
{
    public QuestionValidator()
    {
        RuleFor(p => p.Difficulty)
                .NotNull()
                .WithMessage("Difficulty level can't be null")
                .IsInEnum()
                .WithMessage($@"Difficult level must be in {QuestionDifficultyLevel.Easy}
                        , {QuestionDifficultyLevel.Medium},
                        {QuestionDifficultyLevel.Hard},or {QuestionDifficultyLevel.Expert}.");

        RuleFor(p => p.Type)
                .NotNull()
                .WithMessage("Type question can't be null")
                .IsInEnum()
                .WithMessage($@"Type must be in {QuestionType.FillInTheBlank}
                , {QuestionType.MultipleChoice},
                ,or {QuestionType.TrueFalse}.");

        RuleFor(p => p.QuestionText)
                .NotNull()
                .WithMessage("QuestionText can't be null")
                .NotEmpty()
                .WithMessage("QuestionText can't be empty");

        RuleFor(p => p.Topic)
                .NotNull()
                .WithMessage("Topic Question can't be null")
                .IsInEnum()
                .WithMessage($@"Topic must be in {QuestionTopic.Math}
                , {QuestionTopic.Science},
                , {QuestionTopic.History}, or {QuestionTopic.Arabic}.");

        RuleFor(p => p.MarkPer100)
                .NotNull()
                .WithMessage("Mark is required.")
                .InclusiveBetween(0.1, 100)
                .WithMessage("Mark must be between (0.1 - 100).");


        RuleForEach(p => p.Answers)
                .NotNull()
                .WithMessage("Answers of Question can't be null")
                .SetValidator(new AnswerQuestionValidator());

        When(x => x.Type == QuestionType.TrueFalse && x.Answers != null, () =>
        {
            RuleFor(p => p.Answers)
                    .Must(v => v != null && v.Count == 2)
                    .WithMessage("Question of TrueFalse need two answers only.");

            RuleFor(p => p.Answers)
                    .Must(v => v != null
                               && v.Count(a => a.IsCorrect == true) == 1
                               && v.Count(a => a.IsCorrect == false) == 1)
                    .WithMessage("Question of True, False need one answer correct only.");
        });

        When(x => x.Type == QuestionType.MultipleChoice, () =>
        {
            RuleFor(p => p.Answers)
                    .Must(v => v != null && v.Count == 4)
                    .WithMessage("Question of Multiple Choice need four answers only.");

            RuleFor(p => p.Answers)
                    .Must(v => v != null
                               && v.Count(a => a.IsCorrect == true) == 1
                               && v.Count(a => a.IsCorrect == false) == 3)
                    .WithMessage("Question of Multiple Choice need one answer correct only.");
        });

        When(x => x.Type == QuestionType.FillInTheBlank, () =>
        {
            RuleFor(p => p.Answers)
                    .Must(v => v != null && v.Count == 1)
                    .WithMessage("Question of Fill In TheBlank Choice need one answer only.");

            RuleFor(p => p.Answers)
                    .Must(v => v != null && v.Count(a => a.IsCorrect == true) == 1)
                    .WithMessage("Question of Fill In TheBlank Choice need one answer correct only.");
        });

    }
}
