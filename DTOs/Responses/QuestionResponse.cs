using TestMaster.Entities;
using TestMaster.Enums.Question;

namespace TestMaster.DTOs.Responses;

public class QuestionResponse
{
    public Guid Id { get; set; }
    public string QuestionText { get; set; } = "";
    public QuestionType Type { get; set; }
    public QuestionTopic Topic { get; set; }
    public QuestionDifficultyLevel Difficulty { get; set; }
    public double MarkPer100 { get; set; }
    public DateTime CreatedAt { get; set; }
    public ICollection<AnswerQuestionResponse> Answers { get; set; } = [];

    private QuestionResponse() { }

    public static QuestionResponse FromEntity(Question question)
    {
        return new()
        {
            Id = question.Id,
            Difficulty = question.Difficulty,
            Topic = question.Topic,
            QuestionText = question.QuestionText!,
            MarkPer100 = question.MarkPer100,
            Type = question.Type,
            Answers = question.Answers!.Select(AnswerQuestionResponse.FromEntity).ToList(),
            CreatedAt = question.CreatedAt
        };
    }
}