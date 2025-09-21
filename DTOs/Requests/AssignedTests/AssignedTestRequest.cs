namespace TestMaster.DTOs.Requests.AssignedTests;

public class AssignedTestRequest
{
    public Guid? TestId { get; set; }
    public Guid? AssignedUserId { get; set; }
}