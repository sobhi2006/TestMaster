using System.Transactions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using TestMaster.Data;
using TestMaster.DTOs.Requests.QuestionBank;
using TestMaster.DTOs.Responses;
using TestMaster.Entities;
using TestMaster.Enums.Question;
using TestMaster.Exceptions;

namespace TestMaster.Services.QuestionService.QuestionBankService;

public class QuestionBankService(AppDbContext context, IMemoryCache memoryCache) : IQuestionBankService
{
    private string _keyCachingDataQB = "QuestionBanks";
    public async Task<QuestionBankResponse> AddNewQuestionBankAsync(QuestionBankRequest request, Guid CurrentUserId)
    {
        if (await context.QuestionBanks.AnyAsync(qb => qb.Title == request.Title))
            throw new BusinessRuleException("Invalid Question Bank, it was writed.", StatusCodes.Status400BadRequest);

        var ExistingQuestions = context.Questions
                                        .Where(q => request.QuestionIDs!.Contains(q.Id))
                                        .Select(q => q.Id);
        var MissingQuestions = request.QuestionIDs!.Except(ExistingQuestions);

        if (MissingQuestions.Any())
            throw new BusinessRuleException($@"Invalid QuestionId, one or more of these IDs not found :
                             [{string.Join(", ", MissingQuestions)}], .", StatusCodes.Status400BadRequest);

        Guid NewQuestionBankId = Guid.NewGuid();
        QuestionBank questionBank = new()
        {
            Id = NewQuestionBankId,
            Title = request.Title!,
            Description = request.Description!,
            Difficulty = (QuestionDifficultyLevel)request.Difficulty!,
            CreateAt = DateTime.UtcNow,
            CreatedByUserId = CurrentUserId,
            QuestionBankQuestions = request.QuestionIDs!.Select(q => new QuestionBankQuestion
            {
                Id = Guid.NewGuid(),
                QuestionBankId = NewQuestionBankId,
                QuestionId = q
            }).ToList()
        };

        await context.QuestionBanks.AddAsync(questionBank);
        await context.SaveChangesAsync();
        memoryCache.Remove(_keyCachingDataQB);

        return QuestionBankResponse.FromEntity(questionBank);
    }

    public async Task DeleteQuestionBank(Guid QuestionBankId)
    {
        var QuestionBankQuestion = context.QuestionBanksQuestion.Where(qbq => qbq.QuestionBankId == QuestionBankId);
        var questionBank = await context.QuestionBanks.FirstOrDefaultAsync(qb => qb.Id == QuestionBankId) ??
            throw new BusinessRuleException("Question Bank isn't found.", StatusCodes.Status404NotFound);

        context.QuestionBanks.Remove(questionBank);
        context.QuestionBanksQuestion.RemoveRange(QuestionBankQuestion);
        await context.SaveChangesAsync();
        memoryCache.Remove(_keyCachingDataQB);
    }

    public async Task<QuestionBankResponse> GetQuestionBankAsync(Guid QuestionBankId)
    {
        QuestionBank questionBank = await context.QuestionBanks.Include(qb => qb.QuestionBankQuestions)
                                                               .FirstOrDefaultAsync(qb => qb.Id == QuestionBankId)
                        ?? throw new BusinessRuleException("Question Bank is not found", StatusCodes.Status404NotFound);

        return QuestionBankResponse.FromEntity(questionBank);
    }

    public async Task<IEnumerable<QuestionBankResponse>> GetQuestionBanksAsync(int Page = 1, int PageSize = 10)
    {
        return await memoryCache.GetOrCreate(_keyCachingDataQB, async entry =>
        {
            entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5);
            entry.Size = 1;

            List<QuestionBank> questionBanks = await context.QuestionBanks.Include(qb => qb.QuestionBankQuestions)
                                                                          .Skip((Page - 1) * PageSize)
                                                                          .Take(PageSize)
                                                                          .ToListAsync();
            return questionBanks.Select(QuestionBankResponse.FromEntity);
        })!;
    }

    public async Task<QuestionBankResponse> UpdateQuestionBankAsync(Guid QuestionBankId, QuestionBankRequest request, Guid CurrentUserId)
    {
        var questionBank = await context.QuestionBanks.Include(qb => qb.QuestionBankQuestions)
                                                      .FirstOrDefaultAsync(u => u.Id == QuestionBankId) ??
            throw new BusinessRuleException("Question Bank isn't found.", StatusCodes.Status404NotFound);

        if (await context.QuestionBanks.AnyAsync(qb => qb.Title == request.Title && qb.Id != QuestionBankId))
            throw new BusinessRuleException("Invalid Question Bank, it was writed.", StatusCodes.Status400BadRequest);

        using var Scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

        await this.DeleteQuestionBank(QuestionBankId);
        questionBank = new QuestionBank
        {
            Id = QuestionBankId,
            Title = request.Title!,
            Description = request.Description!,
            Difficulty = (QuestionDifficultyLevel)request.Difficulty!,
            CreateAt = DateTime.UtcNow,
            CreatedByUserId = CurrentUserId,
            QuestionBankQuestions = request.QuestionIDs!.Select(q => new QuestionBankQuestion { QuestionId = q })
                                                        .ToList()
        };
        await context.QuestionBanks.AddAsync(questionBank);
        await context.SaveChangesAsync();
        Scope.Complete();

        memoryCache.Remove(_keyCachingDataQB);

        return QuestionBankResponse.FromEntity(questionBank);
    }
}
