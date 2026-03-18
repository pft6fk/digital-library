using Engagement.Domain.Entities;
using Engagement.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Engagement.Infrastructure.Persistence.Repositories;

public sealed class RatingRepository : IRatingRepository
{
    private readonly EngagementDbContext _context;

    public RatingRepository(EngagementDbContext context) => _context = context;

    public async Task<Rating?> GetByBookAndUserAsync(Guid bookId, Guid userId, CancellationToken cancellationToken = default) =>
        await _context.Ratings
            .FirstOrDefaultAsync(r => r.BookId == bookId && r.UserId == userId, cancellationToken);

    public async Task<(double AverageRating, int TotalCount)> GetBookRatingAsync(Guid bookId, CancellationToken cancellationToken = default)
    {
        var ratings = await _context.Ratings
            .Where(r => r.BookId == bookId)
            .ToListAsync(cancellationToken);

        if (ratings.Count == 0)
            return (0, 0);

        return (ratings.Average(r => r.Value), ratings.Count);
    }

    public async Task AddAsync(Rating rating, CancellationToken cancellationToken = default) =>
        await _context.Ratings.AddAsync(rating, cancellationToken);

    public async Task SaveChangesAsync(CancellationToken cancellationToken = default) =>
        await _context.SaveChangesAsync(cancellationToken);
}
