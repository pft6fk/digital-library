using Engagement.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Engagement.Infrastructure.Persistence.Configurations;

public sealed class CommentConfiguration : IEntityTypeConfiguration<Comment>
{
    public void Configure(EntityTypeBuilder<Comment> builder)
    {
        builder.ToTable("comments");
        builder.HasKey(c => c.Id);
        builder.Property(c => c.Id).ValueGeneratedNever();
        builder.Property(c => c.BookId).IsRequired();
        builder.Property(c => c.UserId).IsRequired();
        builder.Property(c => c.UserName).IsRequired().HasMaxLength(100);
        builder.Property(c => c.Text).IsRequired().HasMaxLength(2000);
        builder.Property(c => c.CreatedAt).IsRequired();
        builder.HasIndex(c => c.BookId);
    }
}
