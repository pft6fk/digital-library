using Catalog.Application.Features.Books.Commands;
using Catalog.Application.Features.Books.Queries;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Catalog.Api.Controllers;

[ApiController]
[Route("api/books")]
public sealed class BooksController : ControllerBase
{
    private readonly GetBooksPagedQueryHandler _getBooksHandler;
    private readonly GetBookByIdQueryHandler _getBookByIdHandler;
    private readonly CreateBookCommandHandler _createBookHandler;

    public BooksController(
        GetBooksPagedQueryHandler getBooksHandler,
        GetBookByIdQueryHandler getBookByIdHandler,
        CreateBookCommandHandler createBookHandler)
    {
        _getBooksHandler = getBooksHandler;
        _getBookByIdHandler = getBookByIdHandler;
        _createBookHandler = createBookHandler;
    }

    [HttpGet]
    public async Task<IActionResult> GetPaged(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] Guid? authorId = null,
        CancellationToken cancellationToken = default)
    {
        var query = new GetBooksPagedQuery(page, pageSize, authorId);
        var result = await _getBooksHandler.HandleAsync(query, cancellationToken);
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var book = await _getBookByIdHandler.HandleAsync(id, cancellationToken);
        return book is null ? NotFound() : Ok(book);
    }

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateBookRequest request, CancellationToken cancellationToken)
    {
        var command = new CreateBookCommand(request.Title, request.Description, request.AuthorId);
        var id = await _createBookHandler.HandleAsync(command, cancellationToken);
        return StatusCode(201, new { id });
    }
}

public sealed record CreateBookRequest(string Title, string? Description, Guid AuthorId);
