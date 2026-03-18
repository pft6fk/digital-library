using Catalog.Application.Features.Books.Commands;
using Catalog.Domain.Entities;
using Catalog.Domain.Repositories;
using NSubstitute;

namespace Catalog.Application.Tests;

public class CreateBookCommandHandlerTests
{
    private readonly IBookRepository _bookRepository = Substitute.For<IBookRepository>();
    private readonly IAuthorRepository _authorRepository = Substitute.For<IAuthorRepository>();
    private readonly CreateBookCommandHandler _handler;

    public CreateBookCommandHandlerTests()
    {
        _handler = new CreateBookCommandHandler(_bookRepository, _authorRepository);
    }

    [Fact]
    public async Task HandleAsync_WithValidData_ReturnsBookId()
    {
        var authorId = Guid.NewGuid();
        var author = Author.Create("Author", null, null);
        _authorRepository.GetByIdAsync(authorId, Arg.Any<CancellationToken>()).Returns(author);

        var command = new CreateBookCommand("War and Peace", "Epic novel", authorId);
        var result = await _handler.HandleAsync(command);

        Assert.NotEqual(Guid.Empty, result);
        await _bookRepository.Received(1).AddAsync(Arg.Any<Book>(), Arg.Any<CancellationToken>());
        await _bookRepository.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task HandleAsync_WithNonExistentAuthor_ThrowsInvalidOperationException()
    {
        var authorId = Guid.NewGuid();
        _authorRepository.GetByIdAsync(authorId, Arg.Any<CancellationToken>()).Returns((Author?)null);

        var command = new CreateBookCommand("Title", null, authorId);

        await Assert.ThrowsAsync<InvalidOperationException>(() => _handler.HandleAsync(command));
        await _bookRepository.DidNotReceive().AddAsync(Arg.Any<Book>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task HandleAsync_WithEmptyTitle_ThrowsArgumentException()
    {
        var authorId = Guid.NewGuid();
        var author = Author.Create("Author", null, null);
        _authorRepository.GetByIdAsync(authorId, Arg.Any<CancellationToken>()).Returns(author);

        var command = new CreateBookCommand("", null, authorId);

        await Assert.ThrowsAsync<ArgumentException>(() => _handler.HandleAsync(command));
    }
}
