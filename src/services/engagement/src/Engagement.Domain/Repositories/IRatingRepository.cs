using Engagement.Domain.Entities;

namespace Engagement.Domain.Repositories;

public interface IRatingRepository
{
    Task<Rating?> GetByBookAndUserAsync(Guid bookId, Guid userId, CancellationToken cancellationToken = default);
    Task<(double AverageRating, int TotalCount)> GetBookRatingAsync(Guid bookId, CancellationToken cancellationToken = default);
    Task AddAsync(Rating rating, CancellationToken cancellationToken = default);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
