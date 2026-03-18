using HotChocolate.Types;
using HotChocolateDddCqrsTemplate.Api.GraphQL.Authorization;
using HotChocolateDddCqrsTemplate.Api.GraphQL.DataLoaders;
using HotChocolateDddCqrsTemplate.Api.GraphQL.Errors;
using HotChocolateDddCqrsTemplate.Api.GraphQL.Mutations;
using HotChocolateDddCqrsTemplate.Api.GraphQL.Queries;
using HotChocolateDddCqrsTemplate.Api.GraphQL.Types;
using Microsoft.Extensions.DependencyInjection;

namespace HotChocolateDddCqrsTemplate.Api;

public static class DependencyInjection
{
    public static IServiceCollection AddApi(this IServiceCollection services)
    {
        services.AddGraphQLServer()
            .AddQueryType(descriptor => descriptor.Name(OperationTypeNames.Query))
            .AddTypeExtension<ProductQueries>()
            .AddMutationType(descriptor => descriptor.Name(OperationTypeNames.Mutation))
            .AddTypeExtension<ProductMutations>()
            .AddType<ProductType>()
            .AddType<CategoryType>()
            .AddType<PagedProductResultType>()
            .AddDataLoader<CategoryByIdDataLoader>()
            .AddDataLoader<ProductByIdDataLoader>()
            .AddErrorFilter<GraphQLErrorFilter>()
            .AddAuthorization();

        return services;
    }
}
