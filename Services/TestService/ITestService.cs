using TestMaster.DTOs.Requests.Evaluations;
using TestMaster.DTOs.Requests.Tests;
using TestMaster.DTOs.Responses;

namespace TestMaster.Services.TestService;

public interface ITestService
{
    public Task<TestResponse> AddNewTestAsync(TestRequest request, Guid CurrentUserId);
    public Task<TestResponse> UpdateTestAsync(Guid TestId, TestRequest request, Guid CurrentUserId);
    public Task<TestResponse> GetTestAsync(Guid TestId);
    public Task<IEnumerable<TestResponse>> GetTestsAsync(int Page = 1, int SizePage = 10);
    public Task DeleteTestAsync(Guid TestId);

    public Task<AssignedTestResponse> AddNewAssignedTestAsync(Guid TestId, Guid UserId, Guid CurrentUserId);
    public Task DeleteAssignedTestAsync(Guid TestId, Guid UserId);

    public Task<EvaluationsResponse> AddNewEvaluationAsync(EvaluationRequest request, Guid CurrentUserId);
    public Task DeleteEvaluationAsync(Guid TestId, Guid UserId);
    public Task<IEnumerable<EvaluationsResponse>> GetEvaluationsAsync(Guid UserId);

    public Task<byte[]> GetQuizAsync(TestResponse testResponse);
    public Task<byte[]> GetTestWithSolutionsAsync(TestResponse testResponse);
}