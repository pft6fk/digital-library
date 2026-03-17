using Catalog.Domain.Entities;
using Catalog.Domain.Repositories;

namespace Catalog.Application.Features.Books.Commands;

public sealed record CreateBookCommand(string Title, string? Description, Guid AuthorId);

public sealed class CreateBookCommandHandler
{
    private readonly IBookRepository _bookRepository;
    private readonly IAuthorRepository _authorRepository;

    public CreateBookCommandHandler(IBookRepository bookRepository, IAuthorRepository authorRepository)
    {
        _bookRepository = bookRepository;
        _authorRepository = authorRepository;
    }

    public async Task<Guid> HandleAsync(CreateBookCommand command, CancellationToken cancellationToken = default)
    {
        var author = await _authorRepository.GetByIdAsync(command.AuthorId, cancellationToken);
        if (author is null)
            throw new InvalidOperationException($"Author with id '{command.AuthorId}' not found.");

        var book = Book.Create(command.Title, command.Description, command.AuthorId);
        await _bookRepository.AddAsync(book, cancellationToken);
        await _bookRepository.SaveChangesAsync(cancellationToken);
        return book.Id;
    }
}
