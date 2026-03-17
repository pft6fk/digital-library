using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Engagement.Application.Features.Comments.Commands;
using Engagement.Application.Features.Comments.Queries;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Engagement.Api.Controllers;

[ApiController]
[Route("api/comments")]
public sealed class CommentsController : ControllerBase
{
    private readonly GetCommentsByBookIdQueryHandler _getCommentsHandler;
    private readonly AddCommentCommandHandler _addCommentHandler;

    public CommentsController(
        GetCommentsByBookIdQueryHandler getCommentsHandler,
        AddCommentCommandHandler addCommentHandler)
    {
        _getCommentsHandler = getCommentsHandler;
        _addCommentHandler = addCommentHandler;
    }

    [HttpGet]
    public async Task<IActionResult> GetByBookId([FromQuery] Guid bookId, CancellationToken cancellationToken)
    {
        var comments = await _getCommentsHandler.HandleAsync(bookId, cancellationToken);
        return Ok(comments);
    }

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> Add([FromBody] AddCommentRequest request, CancellationToken cancellationToken)
    {
        var userId = Guid.Parse(
            User.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? User.FindFirstValue(JwtRegisteredClaimNames.Sub)
            ?? throw new UnauthorizedAccessException("User ID not found in token."));

        var userName = User.FindFirstValue(ClaimTypes.Name) ?? "Unknown";

        var command = new AddCommentCommand(request.BookId, userId, userName, request.Text);
        var id = await _addCommentHandler.HandleAsync(command, cancellationToken);
        return StatusCode(201, new { id });
    }
}

public sealed record AddCommentRequest(Guid BookId, string Text);
