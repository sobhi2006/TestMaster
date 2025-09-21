using TestMaster.Enums.Question;

namespace TestMaster.DTOs.Requests.QuestionBank;

public class QuestionBankRequest
{
    public string? Title { get; set; }
    public string? Description { get; set; }
    public QuestionDifficultyLevel? Difficulty { get; set; }
    public ICollection<Guid>? QuestionIDs { get; set; }

}