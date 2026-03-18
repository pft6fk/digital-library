using Engagement.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Engagement.Infrastructure.Persistence;

public sealed class EngagementDbContext : DbContext
{
    public DbSet<Comment> Comments => Set<Comment>();
    public DbSet<Rating> Ratings => Set<Rating>();

    public EngagementDbContext(DbContextOptions<EngagementDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("engagement");
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(EngagementDbContext).Assembly);
    }
}
