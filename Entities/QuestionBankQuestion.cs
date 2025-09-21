namespace TestMaster.Entities;

public class QuestionBankQuestion
{
    public Guid Id { get; set; }
    public Guid QuestionId { get; set; }
    public Question Question { get; set; } = null!;
    public Guid QuestionBankId { get; set; }
    public QuestionBank QuestionBank { get; set; } = null!;
}