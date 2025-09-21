namespace TestMaster.Entities;

public class Evaluation
{
    public Guid Id { get; set; }
    public Guid CreatedByUserId { get; set; }
    public User CreatedByUser { get; set; } = null!;
    public Guid EvaluationUserId { get; set; }
    public Guid TestId { get; set; }
    public Test Test { get; set; } = null!;
    public int TotalScore { get; set; }
    public double Score { get; set; }
    public string Feedback { get; set; } = string.Empty;
    public DateTime EvaluatedAt { get; set; }

}