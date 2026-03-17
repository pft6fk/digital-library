using Catalog.Application.Features.Authors.Commands;
using Catalog.Application.Features.Authors.Queries;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Catalog.Api.Controllers;

[ApiController]
[Route("api/authors")]
public sealed class AuthorsController : ControllerBase
{
    private readonly GetAuthorsListQueryHandler _getAuthorsHandler;
    private readonly CreateAuthorCommandHandler _createAuthorHandler;

    public AuthorsController(GetAuthorsListQueryHandler getAuthorsHandler, CreateAuthorCommandHandler createAuthorHandler)
    {
        _getAuthorsHandler = getAuthorsHandler;
        _createAuthorHandler = createAuthorHandler;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var authors = await _getAuthorsHandler.HandleAsync(cancellationToken);
        return Ok(authors);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var authors = await _getAuthorsHandler.HandleAsync(cancellationToken);
        var author = authors.FirstOrDefault(a => a.Id == id);
        return author is null ? NotFound() : Ok(author);
    }

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateAuthorRequest request, CancellationToken cancellationToken)
    {
        var command = new CreateAuthorCommand(request.FullName, request.Bio, request.BirthDate);
        var id = await _createAuthorHandler.HandleAsync(command, cancellationToken);
        return StatusCode(201, new { id });
    }
}

public sealed record CreateAuthorRequest(string FullName, string? Bio, DateTime? BirthDate);
