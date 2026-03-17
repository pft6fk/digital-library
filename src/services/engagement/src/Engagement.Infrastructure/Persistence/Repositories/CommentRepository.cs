using Engagement.Domain.Entities;
using Engagement.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Engagement.Infrastructure.Persistence.Repositories;

public sealed class CommentRepository : ICommentRepository
{
    private readonly EngagementDbContext _context;

    public CommentRepository(EngagementDbContext context) => _context = context;

    public async Task<IReadOnlyList<Comment>> GetByBookIdAsync(Guid bookId, CancellationToken cancellationToken = default) =>
        await _context.Comments
            .Where(c => c.BookId == bookId)
            .OrderByDescending(c => c.CreatedAt)
            .ToListAsync(cancellationToken);

    public async Task AddAsync(Comment comment, CancellationToken cancellationToken = default) =>
        await _context.Comments.AddAsync(comment, cancellationToken);

    public async Task SaveChangesAsync(CancellationToken cancellationToken = default) =>
        await _context.SaveChangesAsync(cancellationToken);
}
