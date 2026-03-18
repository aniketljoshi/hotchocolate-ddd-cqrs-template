using HotChocolateDddCqrsTemplate.Application.Common.Interfaces;
using HotChocolateDddCqrsTemplate.Domain.Catalog.Repositories;
using HotChocolateDddCqrsTemplate.Infrastructure.Observability;
using HotChocolateDddCqrsTemplate.Infrastructure.Outbox;
using HotChocolateDddCqrsTemplate.Infrastructure.Persistence;
using HotChocolateDddCqrsTemplate.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace HotChocolateDddCqrsTemplate.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, string connectionString)
    {
        services.AddSingleton<CatalogQueryCounter>();
        services.AddSingleton<CatalogQueryCounterInterceptor>();

        services.AddDbContext<ApplicationDbContext>((serviceProvider, options) =>
        {
            options.UseNpgsql(connectionString);
            options.AddInterceptors(serviceProvider.GetRequiredService<CatalogQueryCounterInterceptor>());
        });

        services.AddScoped<IApplicationDbContext>(serviceProvider => serviceProvider.GetRequiredService<ApplicationDbContext>());
        services.AddScoped<IOutboxWriter, OutboxWriter>();
        services.AddScoped<IProductRepository, ProductRepository>();
        services.AddScoped<ICategoryRepository, CategoryRepository>();

        services.AddHostedService<OutboxProcessor>();

        return services;
    }
}
