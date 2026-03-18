using Catalog.Application.Features.Books.Queries;
using Catalog.Domain.Entities;
using Catalog.Domain.Repositories;
using NSubstitute;

namespace Catalog.Application.Tests;

public class GetBookByIdQueryHandlerTests
{
    private readonly IBookRepository _bookRepository = Substitute.For<IBookRepository>();
    private readonly IAuthorRepository _authorRepository = Substitute.For<IAuthorRepository>();
    private readonly GetBookByIdQueryHandler _handler;

    public GetBookByIdQueryHandlerTests()
    {
        _handler = new GetBookByIdQueryHandler(_bookRepository, _authorRepository);
    }

    [Fact]
    public async Task HandleAsync_WithExistingBook_ReturnsBookDetailWithAuthorName()
    {
        var authorId = Guid.NewGuid();
        var book = Book.Create("War and Peace", "Epic novel", authorId);
        var author = Author.Create("Leo Tolstoy", null, null);

        _bookRepository.GetByIdAsync(book.Id, Arg.Any<CancellationToken>()).Returns(book);
        _authorRepository.GetByIdAsync(authorId, Arg.Any<CancellationToken>()).Returns(author);

        var result = await _handler.HandleAsync(book.Id);

        Assert.NotNull(result);
        Assert.Equal("War and Peace", result!.Title);
        Assert.Equal("Leo Tolstoy", result.AuthorName);
    }

    [Fact]
    public async Task HandleAsync_WithNonExistentBook_ReturnsNull()
    {
        var id = Guid.NewGuid();
        _bookRepository.GetByIdAsync(id, Arg.Any<CancellationToken>()).Returns((Book?)null);

        var result = await _handler.HandleAsync(id);

        Assert.Null(result);
    }

    [Fact]
    public async Task HandleAsync_WithMissingAuthor_ReturnsUnknownAuthorName()
    {
        var authorId = Guid.NewGuid();
        var book = Book.Create("Orphan Book", null, authorId);

        _bookRepository.GetByIdAsync(book.Id, Arg.Any<CancellationToken>()).Returns(book);
        _authorRepository.GetByIdAsync(authorId, Arg.Any<CancellationToken>()).Returns((Author?)null);

        var result = await _handler.HandleAsync(book.Id);

        Assert.NotNull(result);
        Assert.Equal("Unknown", result!.AuthorName);
    }
}
