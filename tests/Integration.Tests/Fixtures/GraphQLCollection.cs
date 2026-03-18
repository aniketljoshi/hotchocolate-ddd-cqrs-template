namespace HotChocolateDddCqrsTemplate.Integration.Tests.Fixtures;

[CollectionDefinition(Name)]
public sealed class GraphQLCollection : ICollectionFixture<GraphQLTestFixture>
{
    public const string Name = "GraphQL";
}
