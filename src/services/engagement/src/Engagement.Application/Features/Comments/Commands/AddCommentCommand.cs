using Engagement.Domain.Entities;
using Engagement.Domain.Repositories;

namespace Engagement.Application.Features.Comments.Commands;

public sealed record AddCommentCommand(Guid BookId, Guid UserId, string UserName, string Text);

public sealed class AddCommentCommandHandler
{
    private readonly ICommentRepository _commentRepository;

    public AddCommentCommandHandler(ICommentRepository commentRepository) =>
        _commentRepository = commentRepository;

    public async Task<Guid> HandleAsync(AddCommentCommand command, CancellationToken cancellationToken = default)
    {
        var comment = Comment.Create(command.BookId, command.UserId, command.UserName, command.Text);
        await _commentRepository.AddAsync(comment, cancellationToken);
        await _commentRepository.SaveChangesAsync(cancellationToken);
        return comment.Id;
    }
}
