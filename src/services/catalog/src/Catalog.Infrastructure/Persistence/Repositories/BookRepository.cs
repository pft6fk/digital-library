using Catalog.Domain.Entities;
using Catalog.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Catalog.Infrastructure.Persistence.Repositories;

public sealed class BookRepository : IBookRepository
{
    private readonly CatalogDbContext _context;

    public BookRepository(CatalogDbContext context) => _context = context;

    public async Task<(IReadOnlyList<Book> Items, int TotalCount)> GetPagedAsync(
        int page, int pageSize, Guid? authorId = null, CancellationToken cancellationToken = default)
    {
        var query = _context.Books.AsQueryable();

        if (authorId.HasValue)
            query = query.Where(b => b.AuthorId == authorId.Value);

        var totalCount = await query.CountAsync(cancellationToken);
        var items = await query
            .OrderBy(b => b.Title)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }

    public async Task<Book?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        await _context.Books.FindAsync([id], cancellationToken);

    public async Task AddAsync(Book book, CancellationToken cancellationToken = default) =>
        await _context.Books.AddAsync(book, cancellationToken);

    public async Task SaveChangesAsync(CancellationToken cancellationToken = default) =>
        await _context.SaveChangesAsync(cancellationToken);
}
