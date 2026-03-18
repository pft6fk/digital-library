using Catalog.Application.Features.Books.Queries;
using Catalog.Domain.Entities;
using Catalog.Domain.Repositories;
using NSubstitute;

namespace Catalog.Application.Tests;

public class GetBooksPagedQueryHandlerTests
{
    private readonly IBookRepository _bookRepository = Substitute.For<IBookRepository>();
    private readonly GetBooksPagedQueryHandler _handler;

    public GetBooksPagedQueryHandlerTests()
    {
        _handler = new GetBooksPagedQueryHandler(_bookRepository);
    }

    [Fact]
    public async Task HandleAsync_ReturnsPaginatedResults()
    {
        var authorId = Guid.NewGuid();
        var books = new List<Book>
        {
            Book.Create("Book 1", null, authorId),
            Book.Create("Book 2", "Desc", authorId),
        };
        _bookRepository.GetPagedAsync(1, 10, null, Arg.Any<CancellationToken>())
            .Returns((books.AsReadOnly(), 2));

        var query = new GetBooksPagedQuery(1, 10);
        var result = await _handler.HandleAsync(query);

        Assert.Equal(2, result.TotalCount);
        Assert.Equal(2, result.Items.Count);
        Assert.Equal(1, result.Page);
        Assert.Equal(10, result.PageSize);
        Assert.Equal(1, result.TotalPages);
    }

    [Fact]
    public async Task HandleAsync_WithAuthorFilter_PassesAuthorId()
    {
        var authorId = Guid.NewGuid();
        _bookRepository.GetPagedAsync(1, 10, authorId, Arg.Any<CancellationToken>())
            .Returns((new List<Book>().AsReadOnly(), 0));

        var query = new GetBooksPagedQuery(1, 10, authorId);
        var result = await _handler.HandleAsync(query);

        Assert.Empty(result.Items);
        Assert.Equal(0, result.TotalCount);
    }

    [Fact]
    public async Task HandleAsync_MapsFieldsCorrectly()
    {
        var authorId = Guid.NewGuid();
        var book = Book.Create("Test Book", "Test Description", authorId);
        _bookRepository.GetPagedAsync(1, 10, null, Arg.Any<CancellationToken>())
            .Returns((new List<Book> { book }.AsReadOnly(), 1));

        var result = await _handler.HandleAsync(new GetBooksPagedQuery(1, 10));

        var dto = result.Items[0];
        Assert.Equal("Test Book", dto.Title);
        Assert.Equal("Test Description", dto.Description);
        Assert.Equal(authorId, dto.AuthorId);
    }
}
