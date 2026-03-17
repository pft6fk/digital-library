using Catalog.Application.Common;
using Catalog.Domain.Repositories;

namespace Catalog.Application.Features.Books.Queries;

public sealed record BookListDto(Guid Id, string Title, string? Description, Guid AuthorId);

public sealed record GetBooksPagedQuery(int Page = 1, int PageSize = 10, Guid? AuthorId = null);

public sealed class GetBooksPagedQueryHandler
{
    private readonly IBookRepository _bookRepository;

    public GetBooksPagedQueryHandler(IBookRepository bookRepository) =>
        _bookRepository = bookRepository;

    public async Task<PagedResult<BookListDto>> HandleAsync(GetBooksPagedQuery query, CancellationToken cancellationToken = default)
    {
        var (items, totalCount) = await _bookRepository.GetPagedAsync(query.Page, query.PageSize, query.AuthorId, cancellationToken);
        var dtos = items.Select(b => new BookListDto(b.Id, b.Title, b.Description, b.AuthorId)).ToList();
        return new PagedResult<BookListDto>(dtos, totalCount, query.Page, query.PageSize);
    }
}
