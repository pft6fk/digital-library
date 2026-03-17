using Catalog.Domain.Entities;

namespace Catalog.Domain.Repositories;

public interface IBookRepository
{
    Task<(IReadOnlyList<Book> Items, int TotalCount)> GetPagedAsync(int page, int pageSize, Guid? authorId = null, CancellationToken cancellationToken = default);
    Task<Book?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task AddAsync(Book book, CancellationToken cancellationToken = default);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
