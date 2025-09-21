using TestMaster.Entities;

namespace TestMaster.DTOs.Responses;

public class EvaluationsResponse
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public Guid TestId { get; set; }
    public int TotalScore { get; set; }
    public double Score { get; set; }
    public string Feedback { get; set; } = "";
    public DateTime EvaluatedAt { get; set; }

    private EvaluationsResponse() { }

    public static EvaluationsResponse FromEntity(Evaluation evaluation)
    {
        return new()
        {
            Id = evaluation.Id,
            TestId = evaluation.TestId,
            UserId = evaluation.CreatedByUserId,
            Feedback = evaluation.Feedback!,
            Score = evaluation.Score,
            TotalScore = evaluation.TotalScore,
            EvaluatedAt = evaluation.EvaluatedAt
        };
    }
}