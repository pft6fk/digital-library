using Engagement.Application.Features.Ratings.Commands;
using Engagement.Domain.Entities;
using Engagement.Domain.Repositories;
using NSubstitute;

namespace Engagement.Application.Tests;

public class AddRatingCommandHandlerTests
{
    private readonly IRatingRepository _ratingRepository = Substitute.For<IRatingRepository>();
    private readonly AddRatingCommandHandler _handler;

    public AddRatingCommandHandlerTests()
    {
        _handler = new AddRatingCommandHandler(_ratingRepository);
    }

    [Fact]
    public async Task HandleAsync_NewRating_CreatesAndReturnsId()
    {
        var bookId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        _ratingRepository.GetByBookAndUserAsync(bookId, userId, Arg.Any<CancellationToken>())
            .Returns((Rating?)null);

        var command = new AddRatingCommand(bookId, userId, 4);
        var result = await _handler.HandleAsync(command);

        Assert.NotEqual(Guid.Empty, result);
        await _ratingRepository.Received(1).AddAsync(Arg.Any<Rating>(), Arg.Any<CancellationToken>());
        await _ratingRepository.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task HandleAsync_ExistingRating_UpdatesAndReturnsExistingId()
    {
        var bookId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var existing = Rating.Create(bookId, userId, 3);
        _ratingRepository.GetByBookAndUserAsync(bookId, userId, Arg.Any<CancellationToken>())
            .Returns(existing);

        var command = new AddRatingCommand(bookId, userId, 5);
        var result = await _handler.HandleAsync(command);

        Assert.Equal(existing.Id, result);
        Assert.Equal(5, existing.Value);
        await _ratingRepository.DidNotReceive().AddAsync(Arg.Any<Rating>(), Arg.Any<CancellationToken>());
        await _ratingRepository.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task HandleAsync_WithInvalidValue_ThrowsArgumentException()
    {
        var bookId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        _ratingRepository.GetByBookAndUserAsync(bookId, userId, Arg.Any<CancellationToken>())
            .Returns((Rating?)null);

        var command = new AddRatingCommand(bookId, userId, 0);

        await Assert.ThrowsAsync<ArgumentException>(() => _handler.HandleAsync(command));
    }
}
