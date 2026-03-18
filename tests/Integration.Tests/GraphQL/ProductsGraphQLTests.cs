using System.Net.Http.Json;
using System.Text.Json.Nodes;
using HotChocolateDddCqrsTemplate.Domain.Catalog.Events;
using HotChocolateDddCqrsTemplate.Infrastructure.Persistence;
using HotChocolateDddCqrsTemplate.Integration.Tests.Fixtures;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace HotChocolateDddCqrsTemplate.Integration.Tests.GraphQL;

[Collection(GraphQLCollection.Name)]
public sealed class ProductsGraphQLTests(GraphQLTestFixture fixture)
{
    [Fact]
    public async Task ProductsQuery_ShouldBatchCategoryLoadingIntoSingleDatabaseQuery()
    {
        await fixture.ResetAsync();
        fixture.QueryCounter.Reset();

        const string query = """
            query {
              products(pageNumber: 1, pageSize: 10) {
                totalCount
                items {
                  id
                  name
                  category {
                    id
                    name
                  }
                }
              }
            }
            """;

        var response = await fixture.Client.PostAsJsonAsync("/graphql", new { query });
        response.EnsureSuccessStatusCode();

        var payload = JsonNode.Parse(await response.Content.ReadAsStringAsync())!;

        Assert.Null(payload["errors"]);
        Assert.Equal(10, payload["data"]?["products"]?["totalCount"]?.GetValue<int>());
        Assert.Equal(10, payload["data"]?["products"]?["items"]?.AsArray().Count);
        Assert.Equal(1, fixture.QueryCounter.CategoryQueryCount);
    }

    [Fact]
    public async Task CreateProductMutation_ShouldPersistOutboxMessage()
    {
        await fixture.ResetAsync();
        var categoryId = await fixture.GetFirstCategoryIdAsync();

        var mutation = """
            mutation($input: CreateProductInput!) {
              createProduct(input: $input) {
                product {
                  id
                  name
                }
                errors {
                  code
                  message
                }
              }
            }
            """;

        var variables = new
        {
            input = new
            {
                name = "New Product",
                price = 299.0m,
                currency = "USD",
                sku = "NEW-001",
                categoryId
            }
        };

        var response = await fixture.Client.PostAsJsonAsync("/graphql", new { query = mutation, variables });
        response.EnsureSuccessStatusCode();

        var payload = JsonNode.Parse(await response.Content.ReadAsStringAsync())!;

        Assert.Null(payload["errors"]);
        Assert.Equal("New Product", payload["data"]?["createProduct"]?["product"]?["name"]?.GetValue<string>());
        Assert.Equal(0, payload["data"]?["createProduct"]?["errors"]?.AsArray().Count);

        using var scope = fixture.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        Assert.True(await dbContext.OutboxMessages.AnyAsync(message => message.Type == typeof(ProductCreatedDomainEvent).FullName));
    }
}
