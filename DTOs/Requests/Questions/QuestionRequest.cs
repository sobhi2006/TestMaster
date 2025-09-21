using TestMaster.DTOs.Requests.Answers;
using TestMaster.Enums.Question;

namespace TestMaster.DTOs.Requests.Questions;

public class QuestionRequest
{
    public string? QuestionText { get; set; }
    public QuestionTopic? Topic { get; set; }
    public QuestionType? Type { get; set; }
    public QuestionDifficultyLevel? Difficulty { get; set; }
    public double? MarkPer100 { get; set; }
    public ICollection<AnswerQuestionRequest>? Answers{ get; set; }
}