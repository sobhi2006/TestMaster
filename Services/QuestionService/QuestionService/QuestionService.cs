using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using TestMaster.Data;
using TestMaster.DTOs.Requests.Questions;
using TestMaster.DTOs.Responses;
using TestMaster.Entities;
using TestMaster.Enums.Question;
using TestMaster.Exceptions;

namespace TestMaster.Services.QuestionService;

public class QuestionService(AppDbContext context, IMemoryCache memoryCache) : IQuestionService
{
    private string _keyCachingDataQ = "Questions";
    public async Task<QuestionResponse> AddNewQuestionAsync(QuestionRequest request, Guid CurrentUserId)
    {
        if (await context.Questions.AnyAsync(q => q.QuestionText == request.QuestionText))
            throw new BusinessRuleException("Invalid Question, it was writed.", StatusCodes.Status400BadRequest);


        Guid NewQuestionId = Guid.NewGuid();
        Question question = new()
        {
            Id = NewQuestionId,
            QuestionText = request.QuestionText!,
            MarkPer100 = (double)request.MarkPer100!,
            Difficulty = (QuestionDifficultyLevel)request.Difficulty!,
            CreatedAt = DateTime.UtcNow,
            Type = (QuestionType)request.Type!,
            CreatedByUserId = CurrentUserId,
            Answers = request.Answers!.Select(a => new AnswerQuestion
            {
                Id = Guid.NewGuid(),
                AnswerText = a.AnswerText!,
                IsCorrect = (bool)a.IsCorrect!,
                QuestionId = NewQuestionId
            }).ToList()
        };

        await context.Questions.AddAsync(question);
        await context.SaveChangesAsync();
        memoryCache.Remove(_keyCachingDataQ);

        return QuestionResponse.FromEntity(question);
    }

    public async Task DeleteQuestion(Guid QuestionId)
    {
        if (await context.QuestionBanksQuestion.AnyAsync(qbq => qbq.QuestionId == QuestionId))
            throw new BusinessRuleException("Invalid Delete Question, it has found in QuestionBank."
                                            , StatusCodes.Status400BadRequest);

        var question = await context.Questions.FirstOrDefaultAsync(q => q.Id == QuestionId) ??
            throw new BusinessRuleException("Question isn't found.", StatusCodes.Status404NotFound);

        context.Questions.Remove(question);
        await context.SaveChangesAsync();
        memoryCache.Remove(_keyCachingDataQ);
    }

    public async Task<QuestionResponse> GetQuestionAsync(Guid QuestionId)
    {
        Question question = await context.Questions.Include(q => q.Answers)
                                                   .AsNoTracking()
                                                   .FirstOrDefaultAsync(q => q.Id == QuestionId)
                        ?? throw new BusinessRuleException("Question is not found", StatusCodes.Status404NotFound);

        return QuestionResponse.FromEntity(question);
    }

    public async Task<IEnumerable<QuestionResponse>> GetQuestionAsync(QuestionType QuestionType)
    {
        List<Question> questions = await context.Questions.Where(q => q.Type == QuestionType)
                                                          .Include(q => q.Answers)
                                                          .AsNoTracking()
                                                          .ToListAsync();
        return questions.Select(QuestionResponse.FromEntity);
    }

    public async Task<IEnumerable<QuestionResponse>> GetQuestionAsync(QuestionTopic QuestionTopic)
    {
        List<Question> questions = await context.Questions.Where(q => q.Topic == QuestionTopic)
                                                          .Include(q => q.Answers)
                                                          .AsNoTracking()
                                                          .ToListAsync();
        return questions.Select(QuestionResponse.FromEntity);
    }

    public async Task<IEnumerable<QuestionResponse>> GetQuestionsAsync(int Page = 1, int PageSize = 10)
    {
        return await memoryCache.GetOrCreate(_keyCachingDataQ, async entry =>
        {
            entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5);
            entry.Size = 1;

            List<Question> questions = await context.Questions.Include(q => q.Answers)
                                                              .AsNoTracking()
                                                              .Skip((Page - 1) * PageSize)
                                                              .Take(PageSize)
                                                              .ToListAsync();
            return questions.Select(QuestionResponse.FromEntity);
        })!;
    }

    public async Task<QuestionResponse> UpdateQuestionAsync(Guid QuestionId, QuestionRequest request, Guid CurrentUserId)
    {
        var question = await context.Questions.FirstOrDefaultAsync(u => u.Id == QuestionId) ??
            throw new BusinessRuleException("Question isn't found.", StatusCodes.Status404NotFound);

        if (await context.Questions.AnyAsync(q => q.QuestionText == request.QuestionText && q.Id != QuestionId))
            throw new BusinessRuleException("Invalid Question text, it is used.", StatusCodes.Status400BadRequest);

        var OldAnswers = await context.Answers.Where(a => a.QuestionId == QuestionId).ToListAsync();
        context.Answers.RemoveRange(OldAnswers);

        await context.SaveChangesAsync();
        System.Console.WriteLine("after delete old answers");

        question.QuestionText = request.QuestionText!;
        question.MarkPer100 = double.Parse(request.MarkPer100.ToString()!);
        question.Difficulty = Enum.Parse<QuestionDifficultyLevel>(request.Difficulty!.ToString()!);
        question.CreatedAt = DateTime.UtcNow;
        question.Type = (QuestionType)request.Type!;
        question.CreatedByUserId = CurrentUserId;

        await context.SaveChangesAsync();

        question.Answers = request.Answers!.Select(a => new AnswerQuestion
        {
            Id = Guid.NewGuid(),
            AnswerText = a.AnswerText!,
            IsCorrect = (bool)a.IsCorrect!,
            QuestionId = QuestionId
        }).ToList();

        foreach (var answer in question.Answers)
        {
            await context.Answers.AddAsync(answer);
        }
        await context.SaveChangesAsync();

        memoryCache.Remove(_keyCachingDataQ);

        return QuestionResponse.FromEntity(question);
    }
}
