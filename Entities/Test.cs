namespace TestMaster.Entities;

public class Test
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public Guid CreatedByUserId { get; set; }
    public User CreatedByUser { get; set; } = null!;
    public Guid QuestionBankId { get; set; }
    public QuestionBank QuestionBank { get; set; } = null!;
    public TimeSpan Duration { get; set; }
    public bool IsPublished { get; set; }
    public DateTime CreatedAt { get; set; }
    public ICollection<Evaluation> Evaluations { get; set; } = [];
    public ICollection<AssignedTest> AssignedTests { get; set; } = [];
}