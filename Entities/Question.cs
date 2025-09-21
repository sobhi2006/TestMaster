using TestMaster.Enums.Question;

namespace TestMaster.Entities;

public class Question
{
    public Guid Id { get; set; }
    public string QuestionText { get; set; } = string.Empty;
    public QuestionTopic Topic { get; set; }
    public QuestionType Type { get; set; }
    public QuestionDifficultyLevel Difficulty { get; set; }
    public double MarkPer100 { get; set; }
    public DateTime CreatedAt { get; set; }
    public Guid CreatedByUserId { get; set; }
    public User CreatedByUser { get; set; } = null!;
    public ICollection<AnswerQuestion> Answers { get; set; } = [];
    public ICollection<QuestionBankQuestion> QuestionBankQuestions { get; set; } = [];
}