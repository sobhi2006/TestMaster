using System.Text;
using System.Transactions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using TestMaster.Data;
using TestMaster.DTOs.Requests.Evaluations;
using TestMaster.DTOs.Requests.Tests;
using TestMaster.DTOs.Responses;
using TestMaster.Entities;
using TestMaster.Exceptions;
using TestMaster.Services.QuestionService;

namespace TestMaster.Services.TestService;

public class TestService(AppDbContext context, IQuestionService questionService, IMemoryCache memoryCache) : ITestService
{
    private string _keyCachingDataT = "Tests";
    private string _keyCachingDataE = "Evaluations";
    public async Task<TestResponse> AddNewTestAsync(TestRequest request, Guid CurrentUserId)
    {
        if (await context.Tests.AnyAsync(t => t.QuestionBankId == request.QuestionBankId))
            throw new BusinessRuleException("Invalid Question Bank, it is used.", StatusCodes.Status400BadRequest);

        if (!await context.QuestionBanks.AnyAsync(qb => qb.Id == request.QuestionBankId))
            throw new BusinessRuleException("Invalid Question Bank, not found.", StatusCodes.Status400BadRequest);

        Test test = new()
        {
            Id = Guid.NewGuid(),
            Title = request.Title!,
            Description = request.Description!,
            Duration = (TimeSpan)request.Duration!,
            QuestionBankId = (Guid)request.QuestionBankId!,
            CreatedAt = DateTime.UtcNow,
            CreatedByUserId = CurrentUserId,
            IsPublished = false
        };

        await context.Tests.AddAsync(test);
        await context.SaveChangesAsync();
        memoryCache.Remove(_keyCachingDataT);

        return TestResponse.FromEntity(test);
    }

    public async Task DeleteTestAsync(Guid TestId)
    {
        var Test = await context.Tests.FirstOrDefaultAsync(u => u.Id == TestId) ??
            throw new BusinessRuleException("Test isn't found.", StatusCodes.Status404NotFound);

        context.Tests.Remove(Test);
        await context.SaveChangesAsync();
        memoryCache.Remove(_keyCachingDataT);
    }

    public async Task<TestResponse> GetTestAsync(Guid TestId)
    {
        Test Test = await context.Tests
                            .FirstOrDefaultAsync(t => t.Id == TestId)
                        ?? throw new BusinessRuleException("Test is not found", StatusCodes.Status404NotFound);
        return TestResponse.FromEntity(Test);
    }

    public async Task<IEnumerable<TestResponse>> GetTestsAsync(int Page = 1, int SizePage = 10)
    {
        return await memoryCache.GetOrCreate(_keyCachingDataT, async entry =>
        {
            entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5);
            entry.Size = 1;

            List<Test> Test = await context.Tests
                                .Skip((Page - 1) * SizePage).Take(SizePage)
                                .ToListAsync();
            return Test.Select(TestResponse.FromEntity);
        })!;
    }

    public async Task<TestResponse> UpdateTestAsync(Guid TestId, TestRequest request, Guid CurrentUserId)
    {
        var Test = await context.Tests.FirstOrDefaultAsync(t => t.Id == TestId) ??
            throw new BusinessRuleException("Test isn't found.", StatusCodes.Status404NotFound);

        if (await context.Tests.AnyAsync(t => t.QuestionBankId == request.QuestionBankId && t.Id != TestId))
            throw new BusinessRuleException("Invalid Question Bank, it is used.", StatusCodes.Status400BadRequest);

        if (!await context.QuestionBanks.AnyAsync(qb => qb.Id == request.QuestionBankId))
            throw new BusinessRuleException("Invalid Question Bank, not found.", StatusCodes.Status400BadRequest);

        Test.Title = request.Title!;
        Test.Description = request.Description!;
        Test.Duration = (TimeSpan)request.Duration!;
        Test.QuestionBankId = (Guid)request.QuestionBankId!;

        await context.SaveChangesAsync();
        memoryCache.Remove(_keyCachingDataT);

        return TestResponse.FromEntity(Test);
    }

    public async Task<AssignedTestResponse> AddNewAssignedTestAsync(Guid TestId, Guid UserId, Guid CurrentUserId)
    {
        if(!await context.Users.AnyAsync(u => u.Id == UserId))
            throw new BusinessRuleException("User isn't found.", StatusCodes.Status404NotFound);

        if(!await context.Tests.AnyAsync(u => u.Id == TestId))
            throw new BusinessRuleException("Test isn't found.", StatusCodes.Status404NotFound);

        if (await context.AssignedTests.AnyAsync(a => a.AssignedUserId == UserId
                                                      && a.TestId == TestId))
            throw new BusinessRuleException("Invalid Assigned Test, it was assigned for this student."
                                            , StatusCodes.Status400BadRequest);

        AssignedTest assignedTest = new()
        {
            Id = Guid.NewGuid(),
            AssignedUserId = UserId,
            CreatedByUserId = CurrentUserId,
            TestId = TestId,
            AssignedAt = DateTime.UtcNow,
        };

        using var Scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

        await context.AssignedTests.AddAsync(assignedTest);
        var Test = await context.Tests.FirstOrDefaultAsync(t => t.Id == assignedTest.TestId);
        Test!.IsPublished = true;
        await context.SaveChangesAsync();

        Scope.Complete();

        return AssignedTestResponse.FromEntity(assignedTest);
    }

    public async Task DeleteAssignedTestAsync(Guid TestId, Guid UserId)
    {
        if(!await context.Users.AnyAsync(u => u.Id == UserId))
            throw new BusinessRuleException("User isn't found.", StatusCodes.Status404NotFound);

        if(!await context.Tests.AnyAsync(u => u.Id == TestId))
            throw new BusinessRuleException("Test isn't found.", StatusCodes.Status404NotFound);

        var assignedTest = await context.AssignedTests.FirstOrDefaultAsync(a => a.AssignedUserId == UserId
                                                                     && a.TestId == TestId) ??
            throw new BusinessRuleException("this student doesn't have assigned test.", StatusCodes.Status404NotFound);

        context.AssignedTests.Remove(assignedTest);
        await context.SaveChangesAsync();
    }

    public async Task<EvaluationsResponse> AddNewEvaluationAsync(EvaluationRequest request, Guid CurrentUserId)
    {
        if(!await context.Users.AnyAsync(u => u.Id == request.EvaluationUserId))
            throw new BusinessRuleException("User isn't found.", StatusCodes.Status404NotFound);

        if(!await context.Tests.AnyAsync(u => u.Id == request.TestId))
            throw new BusinessRuleException("Test isn't found.", StatusCodes.Status404NotFound);

        if (await context.Evaluations.AnyAsync(a => a.EvaluationUserId == request.EvaluationUserId
                                                      && a.TestId == request.TestId))
            throw new BusinessRuleException("Invalid Evaluation Test, it was evaluated for this student."
                                            , StatusCodes.Status400BadRequest);

        Evaluation evaluation = new()
        {
            Id = Guid.NewGuid(),
            Feedback = request.Feedback!,
            Score = (double)request.Score!,
            TestId = (Guid)request.TestId!,
            EvaluationUserId = (Guid)request.EvaluationUserId!,
            EvaluatedAt = DateTime.UtcNow,
            CreatedByUserId = CurrentUserId,
            TotalScore = (int)request.TotalScore!
        };

        await context.Evaluations.AddAsync(evaluation);
        await context.SaveChangesAsync();
        memoryCache.Remove(_keyCachingDataE);

        return EvaluationsResponse.FromEntity(evaluation);
    }

    public async Task DeleteEvaluationAsync(Guid TestId, Guid UserId)
    {
        if(!await context.Users.AnyAsync(u => u.Id == UserId))
            throw new BusinessRuleException("User isn't found.", StatusCodes.Status404NotFound);

        if(!await context.Tests.AnyAsync(u => u.Id == TestId))
            throw new BusinessRuleException("Test isn't found.", StatusCodes.Status404NotFound);

        var evaluationTest = await context.Evaluations.FirstOrDefaultAsync(a => a.EvaluationUserId == UserId
                                                                     && a.TestId == TestId) ??
            throw new BusinessRuleException("this student doesn't have evaluation on this test."
                                            , StatusCodes.Status404NotFound);

        context.Evaluations.Remove(evaluationTest);
        memoryCache.Remove(_keyCachingDataE);

        await context.SaveChangesAsync();
    }

    public async Task<IEnumerable<EvaluationsResponse>> GetEvaluationsAsync(Guid UserId)
    {
        return await memoryCache.GetOrCreate(_keyCachingDataE, async entry =>
        {
            entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5);
            entry.Size = 1;

            List<Evaluation> evaluations = await context.Evaluations.Where(e => e.EvaluationUserId == UserId)
                                .ToListAsync();
            return evaluations.Select(EvaluationsResponse.FromEntity);
        })!;
    }

    public async Task<byte[]> GetQuizAsync(TestResponse testResponse)
    {
        var csvBuilder = new StringBuilder();
        csvBuilder.AppendLine("\t\t\t\t" + testResponse.Title);
        csvBuilder.AppendLine();
        csvBuilder.AppendLine("Description : " + testResponse.Description);
        csvBuilder.AppendLine("Duration : " + testResponse.Duration);
        csvBuilder.AppendLine();

        var QuestionIDs = await context.QuestionBanksQuestion.Where(qbq => qbq.QuestionBankId == testResponse.QuestionBankId)
                                                       .Select(q => q.QuestionId).ToListAsync();

        int RowNumber = 1;
        foreach (var QuestionId in QuestionIDs)
        {
            char option = 'A';
            var QuestionResponse = await questionService.GetQuestionAsync(QuestionId);
            csvBuilder.AppendLine(QuestionResponse.MarkPer100 + " Mark =>" + "\t" + RowNumber + "-) " +
                                                                QuestionResponse.QuestionText);

            switch (QuestionResponse.Answers.Count)
            {
                case 1:
                    csvBuilder.AppendLine("\t\t   " + option + "-) ..........");
                        option++;
                    break;
                default:
                    foreach (var Answer in QuestionResponse.Answers)
                    {
                        csvBuilder.AppendLine("\t\t   " + option + "-) " + Answer.AnswerText);
                        option++;
                    }
                    break;
            }
            csvBuilder.AppendLine();
            RowNumber++;
        }
        csvBuilder.AppendLine("\t\t\t\tTHE END \n\t\t\t\tGood Luck");

        var fileBytes = Encoding.UTF8.GetBytes(csvBuilder.ToString());

        return fileBytes;
    }

    public async Task<byte[]> GetTestWithSolutionsAsync(TestResponse testResponse)
    {
        var csvBuilder = new StringBuilder();
        csvBuilder.AppendLine("\t\t\t\t" + testResponse.Title);
        csvBuilder.AppendLine();
        csvBuilder.AppendLine("Description : " + testResponse.Description);
        csvBuilder.AppendLine("Duration : " + testResponse.Duration);
        csvBuilder.AppendLine();

        var QuestionIDs = await context.QuestionBanksQuestion.Where(qbq => qbq.QuestionBankId == testResponse.QuestionBankId)
                                                       .Select(q => q.QuestionId).ToListAsync();

        int RowNumber = 1;
        foreach (var QuestionId in QuestionIDs)
        {
            var QuestionResponse = await questionService.GetQuestionAsync(QuestionId);
            csvBuilder.AppendLine("\t" + RowNumber + "-) " +
                                                                    QuestionResponse.QuestionText);

            var CorrectAnswer = QuestionResponse.Answers.Where(a => a.IsCorrect).FirstOrDefault()!.AnswerText.ToString();

            csvBuilder.AppendLine(QuestionResponse.MarkPer100 + " Mark =>" + "\t " + CorrectAnswer);
            csvBuilder.AppendLine();
            RowNumber++;
        }
        csvBuilder.AppendLine("\t\t\t\tTHE END \n\t\t\t\tGood Luck");

        var fileBytes = Encoding.UTF8.GetBytes(csvBuilder.ToString());

        return fileBytes;
    }
}
