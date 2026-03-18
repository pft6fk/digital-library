using Engagement.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Engagement.Infrastructure.Persistence.Configurations;

public sealed class RatingConfiguration : IEntityTypeConfiguration<Rating>
{
    public void Configure(EntityTypeBuilder<Rating> builder)
    {
        builder.ToTable("ratings");
        builder.HasKey(r => r.Id);
        builder.Property(r => r.Id).ValueGeneratedNever();
        builder.Property(r => r.BookId).IsRequired();
        builder.Property(r => r.UserId).IsRequired();
        builder.Property(r => r.Value).IsRequired();
        builder.Property(r => r.CreatedAt).IsRequired();
        builder.HasIndex(r => r.BookId);
        builder.HasIndex(r => new { r.BookId, r.UserId }).IsUnique();
    }
}
