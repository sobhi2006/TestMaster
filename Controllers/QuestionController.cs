using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using TestMaster.DTOs.Requests.Questions;
using TestMaster.DTOs.Responses;
using TestMaster.Enums.Question;
using TestMaster.Exceptions;
using TestMaster.Services.QuestionService;

namespace TestMaster.Controllers;

[ApiController]
[Route("api/v{version:apiVersion}/[Controller]")]
[ApiVersion("1.0")]
[ApiVersion("2.0")]
[Tags("Questions")]
public class QuestionsController(IQuestionService questionService) : ControllerBase
{
    private Guid _currentUserId
        => Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);

    [HttpPost]
    [Authorize(Roles = "Admin, Instructor", Policy = "Activate")]
    [MapToApiVersion("1.0")]
    [Consumes("application/json")]
    [ProducesResponseType<QuestionResponse>(StatusCodes.Status201Created)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status403Forbidden)]
    [ProducesResponseType<QuestionBankResponse>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status500InternalServerError)]
    [EndpointName("AddQuestionAsync")]
    [EndpointSummary("Creates a new Question ")]
    [EndpointDescription("Creates a new Question  and returns the created result.")]
    public async Task<ActionResult<QuestionResponse>> AddQuestionAsync([FromBody] QuestionRequest request)
    {
        var questionResponse = await questionService.AddNewQuestionAsync(request, _currentUserId);
        return Created("Added Successfully." ,questionResponse);
    }

    [HttpGet("by-Type")]
    [Authorize(Roles = "Admin, Instructor", Policy = "Activate")]
    [MapToApiVersion("1.0")]
    [Consumes("application/json")]
    [ProducesResponseType<IActionResult>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status403Forbidden)]
    [ProducesResponseType<QuestionBankResponse>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status500InternalServerError)]
    [EndpointName("GetQuestionsByTypeAsync")]
    [EndpointSummary("retrieve a Question by type question")]
    [EndpointDescription("retrieve a Question by type question from query string, and returns the retrieved result.")]
    public async Task<IActionResult> GetQuestionsByTypeAsync([FromQuery] string Type)
    {
        if (!Enum.TryParse<QuestionType>(Type, out QuestionType TypeQuestion))
        {
            return BadRequest($@"Invalid Type, Type must be {QuestionType.FillInTheBlank},
                                    {QuestionType.MultipleChoice}, or {QuestionType.TrueFalse}");
        }
        var QuestionResponse = await questionService.GetQuestionAsync(TypeQuestion);
        return Ok(QuestionResponse);
    }

    [HttpGet("by-topic")]
    [Authorize(Roles = "Admin, Instructor", Policy = "Activate")]
    [MapToApiVersion("1.0")]
    [Consumes("application/json")]
    [ProducesResponseType<IActionResult>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<QuestionBankResponse>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status403Forbidden)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status500InternalServerError)]
    [EndpointName("GetQuestionsByTopicAsync")]
    [EndpointSummary("retrieve a Question by topic question.")]
    [EndpointDescription("retrieve a Question by topic question and returns the retrieved result.")]
    public async Task<IActionResult> GetQuestionsByTopicAsync([FromQuery] string Topic)
    {
        if (!Enum.TryParse<QuestionTopic>(Topic, out QuestionTopic questionTopic))
        {
            return BadRequest($@"Invalid Type, Type must be {QuestionTopic.Math},
                                    {QuestionTopic.History}, {QuestionTopic.Science}, or {QuestionTopic.Arabic}");
        }
        var QuestionResponse = await questionService.GetQuestionAsync(questionTopic);
        return Ok(QuestionResponse);
    }

    [HttpGet("{QuestionId:guid}")]
    [Authorize(Roles = "Admin, Instructor", Policy = "Activate")]
    [MapToApiVersion("1.0")]
    [Consumes("application/json")]
    [ProducesResponseType<ActionResult<QuestionResponse>>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status403Forbidden)]
    [ProducesResponseType<QuestionBankResponse>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status500InternalServerError)]
    [EndpointName("GetQuestionsByIdAsync")]
    [EndpointSummary("retrieve a Question by Id question")]
    [EndpointDescription("retrieve a Question by Id question, and returns the retrieved result.")]
    public async Task<ActionResult<QuestionResponse>> GetQuestionsByIdAsync(Guid QuestionId)
    {
        var QuestionResponse = await questionService.GetQuestionAsync(QuestionId);
        return Ok(QuestionResponse);
    }

    [HttpGet]
    [Authorize(Roles = "Admin, Instructor", Policy = "Activate")]
    [EnableRateLimiting(policyName: "SlidingWindow")]
    [MapToApiVersion("1.0")]
    [Consumes("application/json")]
    [ProducesResponseType<QuestionResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status403Forbidden)]
    [ProducesResponseType<QuestionBankResponse>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status429TooManyRequests)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status500InternalServerError)]
    [EndpointName("GetQuestionsAsync")]
    [EndpointSummary("retrieve Questions by pagination")]
    [EndpointDescription("retrieve Questions by pagination by default return one page contains 10 question.")]
    public async Task<ActionResult<IEnumerable<QuestionResponse>>> GetQuestionsAsync(
        [FromQuery] int Page = 1, int PageSize = 10)
    {
        if (!(int.TryParse(Page.ToString(), out _) && int.TryParse(PageSize.ToString(), out _)))
            throw new BusinessRuleException("Page and PageSize must be int", StatusCodes.Status400BadRequest);

        Page = Math.Max(1, Page);
        PageSize = Math.Clamp(PageSize, 1, 100);
        var QuestionResponse = await questionService.GetQuestionsAsync(Page, PageSize);
        return Ok(QuestionResponse);
    }

    [HttpPut("{QuestionId:guid}")]
    [Authorize(Roles = "Admin, Instructor", Policy = "Activate")]
    [MapToApiVersion("1.0")]
    [Consumes("application/json")]
    [ProducesResponseType<QuestionResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status403Forbidden)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<QuestionBankResponse>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status500InternalServerError)]
    [EndpointName("UpdateQuestionAsync")]
    [EndpointSummary("Update a Question ")]
    [EndpointDescription("Update a Question by Id, and returns the updated result.")]
    public async Task<ActionResult<QuestionResponse>> UpdateQuestionAsync(Guid QuestionId, QuestionRequest request)
    {
        var questionResponse = await questionService.UpdateQuestionAsync(QuestionId, request, _currentUserId);
        return Ok(questionResponse);
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "Admin,Instructor", Policy = "Activate")]
    [MapToApiVersion("1.0")]
    [Consumes("application/json")]
    [ProducesResponseType<IActionResult>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status403Forbidden)]
    [ProducesResponseType<QuestionBankResponse>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status500InternalServerError)]
    [EndpointName("DeleteQuestionAsync")]
    [EndpointSummary("Delete a Question")]
    [EndpointDescription("Delete a Question by Id, and returns the deleted result.")]
    public async Task<IActionResult> DeleteQuestionAsync(Guid id)
    {
        await questionService.DeleteQuestion(id);
        return Ok("Question Deleted Successfully");
    }
}
