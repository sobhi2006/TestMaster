using TestMaster.Entities;

namespace TestMaster.DTOs.Responses;

public class AssignedTestResponse
{
    public Guid Id { get; set; }
    public Guid TestId { get; set; }
    public Guid UserId { get; set; }
    public DateTime AssignedAt { get; set; }

    private AssignedTestResponse() { }

    public static AssignedTestResponse FromEntity(AssignedTest assignedTest)
    {
        return new()
        {
            Id = assignedTest.Id,
            TestId = assignedTest.TestId,
            UserId = assignedTest.CreatedByUserId,
            AssignedAt = assignedTest.AssignedAt
        };
    }
}