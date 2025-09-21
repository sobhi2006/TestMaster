using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using TestMaster.DTOs.Requests.Users;
using TestMaster.DTOs.Responses;
using TestMaster.Entities;
using TestMaster.Enums;
using TestMaster.Exceptions;
using TestMaster.Services.UserService;

namespace TestMaster.Controllers;

[ApiController]
[Route("api/v{version:apiVersion}/[Controller]")]
[ApiVersion("1.0")]
[ApiVersion("2.0")]
[Tags("Users")]
public class UsersController(IUserService userService) : ControllerBase
{
    private Guid _currentUserId
        => Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);

     private UserRole _currentUserRole
        => Enum.Parse<UserRole>(User.FindFirst(ClaimTypes.Role)?.Value!);

    [HttpPost]
    [Authorize(Roles = "Admin", Policy = "Activate")]
    [MapToApiVersion("1.0")]
    [Consumes("application/json")]
    [ProducesResponseType<ActionResult<UserResponse>>(StatusCodes.Status201Created)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status403Forbidden)]
    [ProducesResponseType<QuestionBankResponse>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status500InternalServerError)]
    [EndpointName("AddNewUserAsyncV1")]
    [EndpointSummary("Creates a new User")]
    [EndpointDescription("Creates a new User and returns the created result.")]
    public async Task<ActionResult<UserResponse>> AddNewUserAsync(UserRequest request)
    {
        System.Console.WriteLine("from add new user in controller");
        var UserResponse = await userService.AddNewUserAsync(request, _currentUserId);
        return Created("Added Successfully", UserResponse);
    }

    [HttpPut("{UserId:guid}")]
    [Authorize(Roles = "Admin", Policy = "Activate")]
    [MapToApiVersion("1.0")]
    [Consumes("application/json")]
    [ProducesResponseType<ActionResult<UserResponse>>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status403Forbidden)]
    [ProducesResponseType<QuestionBankResponse>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status500InternalServerError)]
    [EndpointName("UpdateUserAsyncV1")]
    [EndpointSummary("Update a User")]
    [EndpointDescription("Update a User and returns the updated result.")]
    public async Task<ActionResult<UserResponse>> UpdateUserAsync(Guid UserId, UserRequest request)
    {
        User User = new()
        {
            Id = UserId,
            Email = request.Email!,
            Password = request.Password!,
            Age = (int)request.Age!,
            FName = request.FName!,
            LName = request.LName!,
            IsActive = (bool)request.IsActive!,
            RefreshToken = null,
            RefreshTokenExpiryTime = DateTime.MinValue,
            Role = (UserRole)request.Role!,
            CreatedByUserId = _currentUserId
        };
        var UserResponse = await userService.UpdateUserAsync(User, _currentUserId);
        return Ok(UserResponse);
    }

    [HttpDelete("{Id:guid}")]
    [Authorize(Roles = "Admin", Policy = "Activate")]
    [MapToApiVersion("1.0")]
    [Consumes("application/json")]
    [ProducesResponseType<QuestionBankResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status403Forbidden)]
    [ProducesResponseType<QuestionBankResponse>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status500InternalServerError)]
    [EndpointName("DeleteUserAsyncV1")]
    [EndpointSummary("Delete a User by id")]
    [EndpointDescription("Delete a User and returns the deleted result.")]
    public async Task<IActionResult> DeleteUserAsync(Guid Id)
    {
        if (_currentUserId == Id)
            return BadRequest("you can't delete your-self");

        await userService.DeleteUserAsync(Id);
        return Ok("User Deleted Successfully");
    }

    [HttpGet("{id:guid}")]
    [Authorize(Roles = "Admin, Instructor", Policy = "Activate")] // where Instructor can do operation on student only.
    [MapToApiVersion("1.0")]
    [Consumes("application/json")]
    [ProducesResponseType<ActionResult<UserResponse>>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status403Forbidden)]
    [ProducesResponseType<QuestionBankResponse>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status500InternalServerError)]
    [EndpointName("GetUserAsyncV1")]
    [EndpointSummary("Retrieves a User by id")]
    [EndpointDescription("Retrieves a User and returns the retrieve result, , instructor retrieve only students.")]
    public async Task<ActionResult<UserResponse>> GetUserAsync(Guid Id)
    {
        var UserResponse = await userService.GetUserAsync(Id, _currentUserRole);
        return Ok(UserResponse);
    }

    [HttpGet]
    [Authorize(Roles = "Admin, Instructor", Policy = "Activate")] // where Instructor can do operation on student only.
    [EnableRateLimiting(policyName: "SlidingWindow")]
    [MapToApiVersion("1.0")]
    [Consumes("application/json")]
    [ProducesResponseType<QuestionBankResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status403Forbidden)]
    [ProducesResponseType<QuestionBankResponse>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status429TooManyRequests)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status500InternalServerError)]
    [EndpointName("GetUsersAsync")]
    [EndpointSummary("Retrieve users by pagination")]
    [EndpointDescription("Retrieve users by pagination byDefault retrieve first page and contains 10 users, instructor retrieve only students.")]
    public async Task<ActionResult<IEnumerable<UserResponse>>> GetUsersAsync([FromQuery] int Page = 1, int PageSize = 10)
    {
        if (!(int.TryParse(Page.ToString(), out _) && int.TryParse(PageSize.ToString(), out _)))
            throw new BusinessRuleException("Page and PageSize must be int", StatusCodes.Status400BadRequest);

        Page = Math.Max(1, Page);
        PageSize = Math.Clamp(PageSize, 1, 100);
        var UserResponse = await userService.GetUsersAsync(_currentUserRole, Page, PageSize);
        return Ok(UserResponse);
    }
}
