using TestMaster.Enums.Question;

namespace TestMaster.Entities;

public class QuestionBank
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public Guid CreatedByUserId { get; set; }
    public User CreatedByUser { get; set; } = null!;
    public QuestionDifficultyLevel Difficulty { get; set; }
    public DateTime CreateAt { get; set; }
    public ICollection<QuestionBankQuestion> QuestionBankQuestions { get; set; } = [];
}