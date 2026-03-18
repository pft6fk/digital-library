using Engagement.Application.Features.Ratings.Queries;
using Engagement.Domain.Entities;
using Engagement.Domain.Repositories;
using NSubstitute;

namespace Engagement.Application.Tests;

public class GetBookRatingQueryHandlerTests
{
    private readonly IRatingRepository _ratingRepository = Substitute.For<IRatingRepository>();
    private readonly GetBookRatingQueryHandler _handler;

    public GetBookRatingQueryHandlerTests()
    {
        _handler = new GetBookRatingQueryHandler(_ratingRepository);
    }

    [Fact]
    public async Task HandleAsync_WithRatings_ReturnsAverageAndCount()
    {
        var bookId = Guid.NewGuid();
        _ratingRepository.GetBookRatingAsync(bookId, Arg.Any<CancellationToken>())
            .Returns((3.5, 10));

        var result = await _handler.HandleAsync(bookId);

        Assert.Equal(3.5, result.AverageRating);
        Assert.Equal(10, result.TotalCount);
        Assert.Null(result.UserRating);
    }

    [Fact]
    public async Task HandleAsync_WithNoRatings_ReturnsZero()
    {
        var bookId = Guid.NewGuid();
        _ratingRepository.GetBookRatingAsync(bookId, Arg.Any<CancellationToken>())
            .Returns((0.0, 0));

        var result = await _handler.HandleAsync(bookId);

        Assert.Equal(0, result.AverageRating);
        Assert.Equal(0, result.TotalCount);
    }

    [Fact]
    public async Task HandleAsync_WithUserId_ReturnsUserRating()
    {
        var bookId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var userRating = Rating.Create(bookId, userId, 4);

        _ratingRepository.GetBookRatingAsync(bookId, Arg.Any<CancellationToken>())
            .Returns((4.0, 1));
        _ratingRepository.GetByBookAndUserAsync(bookId, userId, Arg.Any<CancellationToken>())
            .Returns(userRating);

        var result = await _handler.HandleAsync(bookId, userId);

        Assert.Equal(4, result.UserRating);
    }

    [Fact]
    public async Task HandleAsync_WithUserIdButNoRating_ReturnsNullUserRating()
    {
        var bookId = Guid.NewGuid();
        var userId = Guid.NewGuid();

        _ratingRepository.GetBookRatingAsync(bookId, Arg.Any<CancellationToken>())
            .Returns((3.0, 5));
        _ratingRepository.GetByBookAndUserAsync(bookId, userId, Arg.Any<CancellationToken>())
            .Returns((Rating?)null);

        var result = await _handler.HandleAsync(bookId, userId);

        Assert.Null(result.UserRating);
    }
}
