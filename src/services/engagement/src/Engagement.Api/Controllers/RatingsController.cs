using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Engagement.Application.Features.Ratings.Commands;
using Engagement.Application.Features.Ratings.Queries;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Engagement.Api.Controllers;

[ApiController]
[Route("api/ratings")]
public sealed class RatingsController : ControllerBase
{
    private readonly GetBookRatingQueryHandler _getBookRatingHandler;
    private readonly AddRatingCommandHandler _addRatingHandler;

    public RatingsController(
        GetBookRatingQueryHandler getBookRatingHandler,
        AddRatingCommandHandler addRatingHandler)
    {
        _getBookRatingHandler = getBookRatingHandler;
        _addRatingHandler = addRatingHandler;
    }

    [HttpGet]
    public async Task<IActionResult> GetBookRating([FromQuery] Guid bookId, CancellationToken cancellationToken)
    {
        Guid? userId = null;
        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier)
                          ?? User.FindFirstValue(JwtRegisteredClaimNames.Sub);
        if (userIdClaim is not null && Guid.TryParse(userIdClaim, out var parsed))
            userId = parsed;

        var rating = await _getBookRatingHandler.HandleAsync(bookId, userId, cancellationToken);
        return Ok(rating);
    }

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> AddRating([FromBody] AddRatingRequest request, CancellationToken cancellationToken)
    {
        var userId = Guid.Parse(
            User.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? User.FindFirstValue(JwtRegisteredClaimNames.Sub)
            ?? throw new UnauthorizedAccessException("User ID not found in token."));

        var command = new AddRatingCommand(request.BookId, userId, request.Value);
        var id = await _addRatingHandler.HandleAsync(command, cancellationToken);
        return StatusCode(201, new { id });
    }
}

public sealed record AddRatingRequest(Guid BookId, int Value);
