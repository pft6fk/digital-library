using Catalog.Domain.Repositories;

namespace Catalog.Application.Features.Authors.Queries;

public sealed record AuthorDto(Guid Id, string FullName, string? Bio, DateTime? BirthDate);

public sealed class GetAuthorsListQueryHandler
{
    private readonly IAuthorRepository _authorRepository;

    public GetAuthorsListQueryHandler(IAuthorRepository authorRepository) =>
        _authorRepository = authorRepository;

    public async Task<IReadOnlyList<AuthorDto>> HandleAsync(CancellationToken cancellationToken = default)
    {
        var authors = await _authorRepository.GetAllAsync(cancellationToken);
        return authors.Select(a => new AuthorDto(a.Id, a.FullName, a.Bio, a.BirthDate)).ToList();
    }
}
