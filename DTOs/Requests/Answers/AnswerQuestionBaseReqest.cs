namespace TestMaster.DTOs.Requests.Answers;

public class AnswerQuestionRequest
{
    public Guid QuestionId { get; set; }
    public string? AnswerText { get; set; }
    public bool? IsCorrect { get; set; }
}