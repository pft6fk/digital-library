using Engagement.Domain.Repositories;

namespace Engagement.Application.Features.Comments.Queries;

public sealed record CommentDto(Guid Id, Guid BookId, Guid UserId, string UserName, string Text, DateTime CreatedAt);

public sealed class GetCommentsByBookIdQueryHandler
{
    private readonly ICommentRepository _commentRepository;

    public GetCommentsByBookIdQueryHandler(ICommentRepository commentRepository) =>
        _commentRepository = commentRepository;

    public async Task<IReadOnlyList<CommentDto>> HandleAsync(Guid bookId, CancellationToken cancellationToken = default)
    {
        var comments = await _commentRepository.GetByBookIdAsync(bookId, cancellationToken);
        return comments.Select(c => new CommentDto(c.Id, c.BookId, c.UserId, c.UserName, c.Text, c.CreatedAt)).ToList();
    }
}
