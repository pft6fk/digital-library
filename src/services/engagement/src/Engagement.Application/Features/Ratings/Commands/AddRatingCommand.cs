using Engagement.Domain.Entities;
using Engagement.Domain.Repositories;

namespace Engagement.Application.Features.Ratings.Commands;

public sealed record AddRatingCommand(Guid BookId, Guid UserId, int Value);

public sealed class AddRatingCommandHandler
{
    private readonly IRatingRepository _ratingRepository;

    public AddRatingCommandHandler(IRatingRepository ratingRepository) =>
        _ratingRepository = ratingRepository;

    public async Task<Guid> HandleAsync(AddRatingCommand command, CancellationToken cancellationToken = default)
    {
        var existing = await _ratingRepository.GetByBookAndUserAsync(command.BookId, command.UserId, cancellationToken);

        if (existing is not null)
        {
            existing.Update(command.Value);
            await _ratingRepository.SaveChangesAsync(cancellationToken);
            return existing.Id;
        }

        var rating = Rating.Create(command.BookId, command.UserId, command.Value);
        await _ratingRepository.AddAsync(rating, cancellationToken);
        await _ratingRepository.SaveChangesAsync(cancellationToken);
        return rating.Id;
    }
}
