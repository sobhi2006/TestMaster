using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using TestMaster.DTOs.Requests.Evaluations;
using TestMaster.DTOs.Requests.Tests;
using TestMaster.DTOs.Responses;
using TestMaster.Exceptions;
using TestMaster.Services.TestService;

namespace TestMaster.Controllers;

[ApiController]
[Route("api/v{version:apiVersion}/[Controller]")]
[ApiVersion("1.0")]
[ApiVersion("2.0")]
[Tags("Tests")]

public class TestsController(ITestService testService) : ControllerBase
{
    private Guid _currentUserId
        => Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);

    [HttpPost]
    [Authorize(Roles = "Admin, Instructor", Policy = "Activate")]
    [MapToApiVersion("1.0")]
    [Consumes("application/json")]
    [ProducesResponseType<ActionResult<TestResponse>>(StatusCodes.Status201Created)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status403Forbidden)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status500InternalServerError)]
    [EndpointName("AddTestAsync")]
    [EndpointSummary("Creates a new test")]
    [EndpointDescription("Creates a new tests and returns the created result.")]
    public async Task<ActionResult<TestResponse>> AddTestAsync(TestRequest request)
    {
        var TestResponse = await testService.AddNewTestAsync(request, _currentUserId);
        return Created("Added Successfully", TestResponse);
    }

    [HttpPut("{TestId:guid}")]
    [Authorize(Roles = "Admin, Instructor", Policy = "Activate")]
    [MapToApiVersion("1.0")]
    [Consumes("application/json")]
    [ProducesResponseType<ActionResult<TestResponse>>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status403Forbidden)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status500InternalServerError)]
    [EndpointName("UpdateTestAsync")]
    [EndpointSummary("Update a test")]
    [EndpointDescription("Update a test by its id ,and returns the updated result.")]
    public async Task<ActionResult<TestResponse>> UpdateTestAsync(Guid TestId, TestRequest request)
    {
        var TestResponse = await testService.UpdateTestAsync(TestId, request, _currentUserId);
        return Ok(TestResponse);
    }

    [HttpGet("{id:guid}")]
    [Authorize(Roles = "Admin, Instructor", Policy = "Activate")]
    [MapToApiVersion("1.0")]
    [Consumes("application/json")]
    [ProducesResponseType<ActionResult<TestResponse>>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status403Forbidden)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status500InternalServerError)]
    [EndpointName("GetTestAsync")]
    [EndpointSummary("Retrieves a test by id")]
    [EndpointDescription("Retrieves a test by id and returns the retrieved result.")]
    public async Task<ActionResult<TestResponse>> GetTestAsync(Guid id)
    {
        var TestResponse = await testService.GetTestAsync(id);
        return Ok(TestResponse);
    }

    [HttpGet]
    [Authorize(Roles = "Admin, Instructor", Policy = "Activate")]
    [EnableRateLimiting(policyName: "SlidingWindow")]
    [MapToApiVersion("1.0")]
    [Consumes("application/json")]
    [ProducesResponseType<IEnumerable<ActionResult<TestResponse>>>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status403Forbidden)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status429TooManyRequests)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status500InternalServerError)]
    [EndpointName("GetTestsAsync")]
    [EndpointSummary("Retrieve tests by pagination")]
    [EndpointDescription("Retrieve tests by pagination byDefault return first page contains 10 test.")]
    public async Task<ActionResult<IEnumerable<TestResponse>>> GetTestsAsync([FromQuery] int Page = 1, int PageSize = 10)
    {
        if (!(int.TryParse(Page.ToString(), out _) && int.TryParse(PageSize.ToString(), out _)))
            throw new BusinessRuleException("Page and PageSize must be int", StatusCodes.Status400BadRequest);

        Page = Math.Max(1, Page);
        PageSize = Math.Clamp(PageSize, 1, 100);
        var TestResponse = await testService.GetTestsAsync(Page, PageSize);
        return Ok(TestResponse);
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "Admin, Instructor", Policy = "Activate")]
    [MapToApiVersion("1.0")]
    [Consumes("application/json")]
    [ProducesResponseType<IActionResult>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status403Forbidden)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status500InternalServerError)]
    [EndpointName("DeleteTestAsync")]
    [EndpointSummary("Delete Test by id.")]
    [EndpointDescription("Delete test by, and returns the created result.")]
    public async Task<IActionResult> DeleteTestAsync(Guid id)
    {
        await testService.DeleteTestAsync(id);
        return Ok("User Deleted Successfully");
    }

    [HttpPut("{TestId:guid}/assigned/users/{UserId:guid}")]
    [Authorize(Roles = "Admin, Instructor", Policy = "Activate")]
    [MapToApiVersion("1.0")]
    [Consumes("application/json")]
    [ProducesResponseType<AssignedTestResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status403Forbidden)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status500InternalServerError)]
    [EndpointName("AssignedTestAsync")]
    [EndpointSummary("Assigned Test for student to present it later.")]
    [EndpointDescription("Assigned Test for student to present it later and return assigned result.")]
    public async Task<ActionResult<AssignedTestResponse>> AssignedTestAsync(Guid TestId, Guid UserId)
    {
        var AssignedTestResponse = await testService.AddNewAssignedTestAsync(TestId, UserId, _currentUserId);
        return Ok(AssignedTestResponse);
    }

    [HttpPut("{TestId:guid}/unassigned/users/{UserId:guid}")]
    [Authorize(Roles = "Admin, Instructor", Policy = "Activate")]
    [MapToApiVersion("1.0")]
    [Consumes("application/json")]
    [ProducesResponseType<TestResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status403Forbidden)]
    [ProducesResponseType<QuestionBankResponse>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status500InternalServerError)]
    [EndpointName("UnAssignedTestAsync")]
    [EndpointSummary("UnAssigned Test for student")]
    [EndpointDescription("Unassigned Test for student, and returns the unassigned result.")]
    public async Task<ActionResult<TestResponse>> UnAssignedTestAsync(Guid TestId, Guid UserId)
    {
        await testService.DeleteAssignedTestAsync(TestId, UserId);
        return Ok("Unassigned test Successfully");
    }

    [HttpPost("evaluations")]
    [Authorize(Roles = "Admin, Instructor", Policy = "Activate")]
    [MapToApiVersion("1.0")]
    [Consumes("application/json")]
    [ProducesResponseType<TestResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status403Forbidden)]
    [ProducesResponseType<QuestionBankResponse>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status500InternalServerError)]
    [EndpointName("AddEvaluationAsync")]
    [EndpointSummary("Add Evaluation for test for student (mark,....).")]
    [EndpointDescription("Add Evaluation for test for student (mark,....), and returns the evaluated result.")]
    public async Task<ActionResult<TestResponse>> AddEvaluationAsync(EvaluationRequest request)
    {
        var EvaluationResponse = await testService.AddNewEvaluationAsync(request, _currentUserId);
        return Ok(EvaluationResponse);
    }

    [HttpDelete("{TestId:guid}/evaluations/users/{UserId:guid}")]
    [Authorize(Roles = "Admin, Instructor", Policy = "Activate")]
    [MapToApiVersion("1.0")]
    [Consumes("application/json")]
    [ProducesResponseType<IActionResult>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status403Forbidden)]
    [ProducesResponseType<QuestionBankResponse>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status500InternalServerError)]
    [EndpointName("DeleteEvaluationAsync")]
    [EndpointSummary("Delete Evaluation by testId and userId")]
    [EndpointDescription("Delete Evaluation by testId and userId, and returns the deleted result.")]
    public async Task<IActionResult> DeleteEvaluationAsync(Guid TestId, Guid UserId)
    {
        await testService.DeleteEvaluationAsync(TestId, UserId);
        return Ok("Deleted evaluation Successfully");
    }

    [HttpGet("evaluations/users/{UserId:guid}")]
    [Authorize(Roles = "Admin, Instructor", Policy = "Activate")]
    [EnableRateLimiting(policyName: "SlidingWindow")]
    [MapToApiVersion("1.0")]
    [Consumes("application/json")]
    [ProducesResponseType<ActionResult<IEnumerable<EvaluationsResponse>>>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status403Forbidden)]
    [ProducesResponseType<QuestionBankResponse>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status429TooManyRequests)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status500InternalServerError)]
    [EndpointName("GetEvaluationTestByIdAsync")]
    [EndpointSummary("Get Evaluation all Test By UserId")]
    [EndpointDescription("Get Evaluation all Test By UserId, and return retrieved result.")]
    public async Task<ActionResult<IEnumerable<EvaluationsResponse>>> GetEvaluationTestByIdAsync(Guid UserId)
    {
        var EvaluationsTest = await testService.GetEvaluationsAsync(UserId);
        return Ok(EvaluationsTest);
    }

    [HttpGet("{TestId}/quiz")]
    [Authorize(Policy = "Activate")]
    [EnableRateLimiting(policyName: "SlidingWindow")]
    [MapToApiVersion("1.0")]
    [Consumes("application/json")]
    [ProducesResponseType<TestResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status403Forbidden)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status429TooManyRequests)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<QuestionBankResponse>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status500InternalServerError)]
    [EndpointName("GetQuizAsync")]
    [EndpointSummary("Get Quiz for student where test was assigned to him.")]
    [EndpointDescription("Get Quiz for student where test was assigned to him, and returns file with extension (.csv).")]
    public async Task<IActionResult> GetQuizAsync(Guid TestId)
    {
        var TestResponse = await testService.GetTestAsync(TestId);
        await this.AssignedTestAsync(TestId, _currentUserId);
        var FileBytes = await testService.GetQuizAsync(TestResponse);
        return File(FileBytes, "text/csv", $"{TestResponse.Title}.csv");
    }

    [HttpGet("{TestId:guid}")]
    [Authorize(Roles = "Admin, Instructor", Policy = "Activate")]
    [EnableRateLimiting(policyName: "SlidingWindow")]
    [MapToApiVersion("2.0")]
    [Consumes("application/json")]
    [ProducesResponseType<IActionResult>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status403Forbidden)]
    [ProducesResponseType<QuestionBankResponse>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status429TooManyRequests)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status500InternalServerError)]
    [EndpointName("GetTestWithSolutionsAsync")]
    [EndpointSummary("Retrieves a test by id on file with extension (.csv)")]
    [EndpointDescription(@"Retrieves a test by id on file with extension (.csv) with solution and marks on every question,
                        and returns the retrieved result.")]
    public async Task<IActionResult> GetTestWithSolutionsAsync(Guid TestId)
    {
        var TestResponse = await testService.GetTestAsync(TestId);
        var FileBytes = await testService.GetTestWithSolutionsAsync(TestResponse);
        return File(FileBytes, "text/csv", $"{TestResponse.Title}.csv");
    }
}