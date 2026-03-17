using Catalog.Domain.Entities;
using Catalog.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Catalog.Infrastructure.Persistence.Repositories;

public sealed class AuthorRepository : IAuthorRepository
{
    private readonly CatalogDbContext _context;

    public AuthorRepository(CatalogDbContext context) => _context = context;

    public async Task<IReadOnlyList<Author>> GetAllAsync(CancellationToken cancellationToken = default) =>
        await _context.Authors.OrderBy(a => a.FullName).ToListAsync(cancellationToken);

    public async Task<Author?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        await _context.Authors.FindAsync([id], cancellationToken);

    public async Task AddAsync(Author author, CancellationToken cancellationToken = default) =>
        await _context.Authors.AddAsync(author, cancellationToken);

    public async Task SaveChangesAsync(CancellationToken cancellationToken = default) =>
        await _context.SaveChangesAsync(cancellationToken);
}
