using HotChocolateDddCqrsTemplate.Application.Common.Interfaces;
using HotChocolateDddCqrsTemplate.Domain.Catalog;
using HotChocolateDddCqrsTemplate.Infrastructure.Outbox;
using Microsoft.EntityFrameworkCore;

namespace HotChocolateDddCqrsTemplate.Infrastructure.Persistence;

public sealed class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
    : DbContext(options), IApplicationDbContext
{
    public DbSet<Product> Products => Set<Product>();

    public DbSet<Category> Categories => Set<Category>();

    public DbSet<OutboxMessage> OutboxMessages => Set<OutboxMessage>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}
