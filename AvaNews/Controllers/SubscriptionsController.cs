using System.Security.Claims;
using AvaNews.Application.Contracts;
using AvaNews.Application.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AvaNews.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Policy = "NewsWrite")]
[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
public class SubscriptionsController : ControllerBase
{
    private readonly ISubscriptionService _service;

    public SubscriptionsController(ISubscriptionService service)
    {
        _service = service;
    }

    [HttpPost]
    [ProducesResponseType(typeof(CreateSubscriptionResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<CreateSubscriptionResponse>> Create([FromBody] CreateSubscriptionRequest request, CancellationToken ct)
    {
        var userId = User.Identity?.Name
                     ?? User.FindFirst("sub")?.Value
                     ?? User.FindFirst("oid")?.Value
                     ?? User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                     ?? throw new UnauthorizedAccessException("User id not found in token");

        return Ok(await _service.CreateAsync(userId, request, ct));
    }
}