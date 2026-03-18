using Engagement.Domain.Repositories;

namespace Engagement.Application.Features.Ratings.Queries;

public sealed record BookRatingDto(double AverageRating, int TotalCount, int? UserRating);

public sealed class GetBookRatingQueryHandler
{
    private readonly IRatingRepository _ratingRepository;

    public GetBookRatingQueryHandler(IRatingRepository ratingRepository) =>
        _ratingRepository = ratingRepository;

    public async Task<BookRatingDto> HandleAsync(Guid bookId, Guid? userId = null, CancellationToken cancellationToken = default)
    {
        var (average, count) = await _ratingRepository.GetBookRatingAsync(bookId, cancellationToken);

        int? userRating = null;
        if (userId.HasValue && userId.Value != Guid.Empty)
        {
            var existing = await _ratingRepository.GetByBookAndUserAsync(bookId, userId.Value, cancellationToken);
            userRating = existing?.Value;
        }

        return new BookRatingDto(Math.Round(average, 1), count, userRating);
    }
}
