using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using TestMaster.DTOs.Requests.QuestionBank;
using TestMaster.DTOs.Responses;
using TestMaster.Exceptions;
using TestMaster.Services.QuestionService.QuestionBankService;

namespace TestMaster.Controllers;

[ApiController]
[Route("api/v{version:apiVersion}/[Controller]")]
[ApiVersion("1.0")]
[ApiVersion("2.0")]
[Tags("QuestionBanks")]
public class QuestionBanksController(IQuestionBankService questionBankService) : ControllerBase
{
    private Guid _currentUserId
        => Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);

    [HttpPost]
    [Authorize(Roles = "Admin, Instructor", Policy = "Activate")]
    [MapToApiVersion("1.0")]
    [Consumes("application/json")]
    [ProducesResponseType<QuestionBankResponse>(StatusCodes.Status201Created)]
    [ProducesResponseType<QuestionBankResponse>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status500InternalServerError)]
    [EndpointName("AddQuestionBankAsyncV1")]
    [EndpointSummary("Creates a new Question Bank")]
    [EndpointDescription("Creates a new Question Bank and returns the created result.")]
    public async Task<ActionResult<QuestionBankResponse>> AddQuestionBankAsync(QuestionBankRequest request)
    {
        var QuestionBankResponse = await questionBankService.AddNewQuestionBankAsync(request, _currentUserId);
        return Created("Added Successfully", QuestionBankResponse);
    }

    [HttpGet("{QuestionBankId:guid}")]
    [Authorize(Roles = "Admin, Instructor", Policy = "Activate")]
    [MapToApiVersion("1.0")]
    [Consumes("application/json")]
    [ProducesResponseType<ActionResult<QuestionBankResponse>>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<QuestionBankResponse>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status403Forbidden)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status500InternalServerError)]
    [EndpointName("GetQuestionBankByIdAsyncV1")]
    [EndpointSummary("Get Question Bank By Id .")]
    [EndpointDescription("Get Question Bank by Id and return content (Questions, answers......).")]
    public async Task<ActionResult<QuestionBankResponse>> GetQuestionBankByIdAsync(Guid QuestionBankId)
    {
        var QuestionBankResponse = await questionBankService.GetQuestionBankAsync(QuestionBankId);
        return Ok(QuestionBankResponse);
    }

    [HttpGet]
    [Authorize(Roles = "Admin, Instructor", Policy = "Activate")]
    [EnableRateLimiting(policyName: "SlidingWindow")]
    [MapToApiVersion("1.0")]
    [Consumes("application/json")]
    [ProducesResponseType<ActionResult<IEnumerable<QuestionBankResponse>>>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status403Forbidden)]
    [ProducesResponseType<QuestionBankResponse>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status429TooManyRequests)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status500InternalServerError)]
    [EndpointName("GetQuestionBanksAsyncV1")]
    [EndpointSummary("Get Question Banks and return it.")]
    [EndpointDescription("Get Question Banks by default first page and has ten Question banks.")]
    public async Task<ActionResult<IEnumerable<QuestionBankResponse>>> GetQuestionBanksAsync(
        [FromQuery] int Page = 1, int PageSize = 10)
    {
        if (!(int.TryParse(Page.ToString(), out _) && int.TryParse(PageSize.ToString(), out _)))
            throw new BusinessRuleException("Page and PageSize must be int", StatusCodes.Status400BadRequest);

        Page = Math.Max(1, Page);
        PageSize = Math.Clamp(PageSize, 1, 100);
        var QuestionBankResponse = await questionBankService.GetQuestionBanksAsync(Page, PageSize);
        return Ok(QuestionBankResponse);
    }

    [HttpPut("{QuestionBankId:guid}")]
    [Authorize(Roles = "Admin, Instructor", Policy = "Activate")]
    [MapToApiVersion("1.0")]
    [Consumes("application/json")]
    [ProducesResponseType<ActionResult<QuestionBankResponse>>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status403Forbidden)]
    [ProducesResponseType<QuestionBankResponse>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status500InternalServerError)]
    [EndpointName("UpdateQuestionBankAsyncV1")]
    [EndpointSummary("Update a Question.")]
    [EndpointDescription("Update a Question and returns the updated result.")]
    public async Task<ActionResult<QuestionBankResponse>> UpdateQuestionBankAsync(Guid QuestionBankId, QuestionBankRequest request)
    {
        var QuestionBankResponse = await questionBankService.UpdateQuestionBankAsync(QuestionBankId, request, _currentUserId);
        return Ok(QuestionBankResponse);
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "Admin,Instructor", Policy = "Activate")]
    [MapToApiVersion("1.0")]
    [Consumes("application/json")]
    [ProducesResponseType<IActionResult>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<QuestionBankResponse>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status403Forbidden)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status500InternalServerError)]
    [EndpointName("DeleteQuestionBankV1")]
    [EndpointSummary("Delete Question Bank by its Id.")]
    [EndpointDescription("Delete Question Bank by its Id, and returns insurance to deleted it.")]
    public async Task<IActionResult> DeleteQuestionBank(Guid id)
    {
        await questionBankService.DeleteQuestionBank(id);
        return Ok("Question Bank Deleted Successfully");
    }
}
