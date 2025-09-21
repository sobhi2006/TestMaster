using TestMaster.Entities;
using TestMaster.Enums.Question;

namespace TestMaster.DTOs.Responses;

public class QuestionBankResponse
{
    public Guid Id { get; set; }
    public string Title { get; set; } = "";
    public string Description { get; set; } = "";
    public Guid CreatedByUserId { get; set; }
    public QuestionDifficultyLevel Difficulty { get; set; }
    public DateTime CreateAt { get; set; }
    public ICollection<Guid> QuestionIDs { get; set; } = [];

    private QuestionBankResponse() { }

     public static QuestionBankResponse FromEntity(QuestionBank questionBank)
    {
        return new()
        {
            Id = questionBank.Id,
            Difficulty = questionBank.Difficulty,
            Description = questionBank.Description!,
            QuestionIDs = questionBank.QuestionBankQuestions.Select(q => q.QuestionId).ToList()!,
            CreatedByUserId = questionBank.CreatedByUserId,
            Title = questionBank.Title!,
            CreateAt = questionBank.CreateAt
        };
    }
}