using Catalog.Domain.Repositories;

namespace Catalog.Application.Features.Books.Queries;

public sealed record BookDetailDto(Guid Id, string Title, string? Description, Guid AuthorId, string AuthorName);

public sealed class GetBookByIdQueryHandler
{
    private readonly IBookRepository _bookRepository;
    private readonly IAuthorRepository _authorRepository;

    public GetBookByIdQueryHandler(IBookRepository bookRepository, IAuthorRepository authorRepository)
    {
        _bookRepository = bookRepository;
        _authorRepository = authorRepository;
    }

    public async Task<BookDetailDto?> HandleAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var book = await _bookRepository.GetByIdAsync(id, cancellationToken);
        if (book is null) return null;

        var author = await _authorRepository.GetByIdAsync(book.AuthorId, cancellationToken);
        return new BookDetailDto(book.Id, book.Title, book.Description, book.AuthorId, author?.FullName ?? "Unknown");
    }
}
