using Engagement.Domain.Entities;

namespace Engagement.Domain.Repositories;

public interface ICommentRepository
{
    Task<IReadOnlyList<Comment>> GetByBookIdAsync(Guid bookId, CancellationToken cancellationToken = default);
    Task AddAsync(Comment comment, CancellationToken cancellationToken = default);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
