namespace TestMaster.DTOs.Requests.Evaluations;

public class EvaluationRequest
{
    public Guid? EvaluationUserId { get; set; }
    public Guid? TestId { get; set; }
    public int? TotalScore { get; set; }
    public double? Score { get; set; }
    public string? Feedback { get; set; }
}