using TestMaster.Entities;

namespace TestMaster.DTOs.Responses;

public class TestResponse
{
    public Guid Id { get; set; }
    public string Title { get; set; } = "";
    public string Description { get; set; } = "";
    public Guid CreatedByUserId { get; set; }
    public Guid QuestionBankId { get; set; }
    public TimeSpan Duration { get; set; }
    public bool IsPublished { get; set; }
    public DateTime CreatedAt { get; set; }

    private TestResponse() { }

    public static TestResponse FromEntity(Test test)
    {
        return new()
        {
            Id = test.Id,
            Description = test.Description!,
            Duration = test.Duration,
            CreatedByUserId = test.CreatedByUserId,
            QuestionBankId = test.QuestionBankId,
            Title = test.Title!,
            IsPublished = test.IsPublished,
            CreatedAt = test.CreatedAt
        };
    }
}