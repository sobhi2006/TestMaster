namespace TestMaster.DTOs.Requests.Tests;

public class TestRequest
{
    public string? Title { get; set; }
    public string? Description { get; set; }
    public TimeSpan? Duration { get; set; }
    public Guid? QuestionBankId { get; set; }

}