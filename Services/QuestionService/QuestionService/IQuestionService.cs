using TestMaster.DTOs.Requests.Questions;
using TestMaster.DTOs.Responses;
using TestMaster.Enums.Question;

namespace TestMaster.Services.QuestionService;

public interface IQuestionService
{
    public Task<QuestionResponse> AddNewQuestionAsync(QuestionRequest request, Guid CurrentUserId);
    public Task<QuestionResponse> UpdateQuestionAsync(Guid QuestionId, QuestionRequest request, Guid CurrentUserId);
    public Task<QuestionResponse> GetQuestionAsync(Guid QuestionId);
    public Task<IEnumerable<QuestionResponse>> GetQuestionAsync(QuestionType QuestionType);
    public Task<IEnumerable<QuestionResponse>> GetQuestionAsync(QuestionTopic QuestionTopic);
    public Task<IEnumerable<QuestionResponse>> GetQuestionsAsync(int Page = 1, int PageSize = 10);
    public Task DeleteQuestion(Guid QuestionId);
}