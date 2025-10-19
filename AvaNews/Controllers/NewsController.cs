using System.ComponentModel.DataAnnotations;
using AvaNews.Application.Contracts;
using AvaNews.Application.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AvaNews.Controllers;

[ApiController]
[Route("api/[controller]")]
[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
public class NewsController : ControllerBase
{
    private readonly INewsService _service;

    public NewsController(INewsService service)
    {
        _service = service;
    }

    [HttpGet]
    [ProducesResponseType(typeof(List<NewsItemDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetAllNewsAsync(CancellationToken ct)
    {
        return Ok(await _service.GetAllNewsAsync(ct));
    }

    [HttpGet("range")]
    [ProducesResponseType(typeof(List<NewsItemDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetAllNewsWithGivingDayAsync([FromQuery] [Range(1, 365)] int days, CancellationToken ct = default)
    {
        return Ok(await _service.GetAllNewsWithGivingDayAsync(days, ct));
    }

    [HttpGet("instrument/{ticker}")]
    [ProducesResponseType(typeof(List<NewsItemDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetAllNewsPerInstrumentAsync(
        [FromRoute] string ticker,
        [FromQuery] [Range(1, 100)] int limit = 10,
        CancellationToken ct = default)
    {
        return Ok(await _service.GetAllNewsPerInstrumentAsync(ticker, limit, ct));
    }

    [HttpGet("search")]
    [ProducesResponseType(typeof(List<NewsItemDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> SearchAsync([FromQuery] [MinLength(2)] string query, CancellationToken ct)
    {
        return Ok(await _service.SearchAsync(query, ct));
    }

    [AllowAnonymous]
    [HttpGet("latest")]
    [ProducesResponseType(typeof(List<NewsItemDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetLatestNewsAsync([FromQuery] [Range(1, 50)] int limit = 5, CancellationToken ct = default)
    {
        return Ok(await _service.GetLatestNewsAsync(limit, ct));
    }
}