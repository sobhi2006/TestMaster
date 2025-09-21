using TestMaster.DTOs.Requests.QuestionBank;
using TestMaster.DTOs.Responses;

namespace TestMaster.Services.QuestionService.QuestionBankService;

public interface IQuestionBankService
{
    public Task<QuestionBankResponse> AddNewQuestionBankAsync(QuestionBankRequest request, Guid CurrentUserId);
    public Task<QuestionBankResponse> UpdateQuestionBankAsync(Guid QuestionBankId, QuestionBankRequest request, Guid CurrentUserId);
    public Task<QuestionBankResponse> GetQuestionBankAsync(Guid QuestionBankId);
    public Task<IEnumerable<QuestionBankResponse>> GetQuestionBanksAsync(int Page = 1, int PageSize = 10);
    public Task DeleteQuestionBank(Guid QuestionBankId);
}