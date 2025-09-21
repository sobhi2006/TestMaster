using TestMaster.Entities;

namespace TestMaster.DTOs.Responses;

public class AnswerQuestionResponse
{
    public string AnswerText { get; set; } = "";
    public bool IsCorrect { get; set; }

    private AnswerQuestionResponse() { }

    public static AnswerQuestionResponse FromEntity(AnswerQuestion answerQuestion)
    {
        return new()
        {
            AnswerText = answerQuestion.AnswerText!,
            IsCorrect = answerQuestion.IsCorrect,
        };
    }
}