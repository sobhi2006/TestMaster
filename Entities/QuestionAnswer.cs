namespace TestMaster.Entities;

public class AnswerQuestion
{
    public Guid Id { get; set; }
    public string AnswerText { get; set; } = string.Empty;
    public bool IsCorrect { get; set; } = false;
    public Guid QuestionId { get; set; }
    public Question Question { get; set; } = null!;
}