using Catalog.Domain.Entities;
using Catalog.Domain.Repositories;

namespace Catalog.Application.Features.Authors.Commands;

public sealed record CreateAuthorCommand(string FullName, string? Bio, DateTime? BirthDate);

public sealed class CreateAuthorCommandHandler
{
    private readonly IAuthorRepository _authorRepository;

    public CreateAuthorCommandHandler(IAuthorRepository authorRepository) =>
        _authorRepository = authorRepository;

    public async Task<Guid> HandleAsync(CreateAuthorCommand command, CancellationToken cancellationToken = default)
    {
        var author = Author.Create(command.FullName, command.Bio, command.BirthDate);
        await _authorRepository.AddAsync(author, cancellationToken);
        await _authorRepository.SaveChangesAsync(cancellationToken);
        return author.Id;
    }
}
