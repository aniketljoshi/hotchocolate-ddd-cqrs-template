using System.Net.Http.Json;
using System.Text.Json.Nodes;
using HotChocolateDddCqrsTemplate.Integration.Tests.Fixtures;

namespace HotChocolateDddCqrsTemplate.Integration.Tests.GraphQL;

[Collection(GraphQLCollection.Name)]
public sealed class AuthorizationTests(GraphQLTestFixture fixture)
{
    private const string CostPriceQuery = """
        query {
          products(pageNumber: 1, pageSize: 1) {
            items {
              id
              name
              costPrice
              costPriceCurrency
            }
          }
        }
        """;

    [Fact]
    public async Task CostPrice_WithoutAuth_ShouldReturnNullWithAuthError()
    {
        await fixture.ResetAsync();

        // Anonymous request — no X-Test-Role header
        var response = await fixture.Client.PostAsJsonAsync("/graphql", new { query = CostPriceQuery });
        response.EnsureSuccessStatusCode();

        var payload = JsonNode.Parse(await response.Content.ReadAsStringAsync())!;

        // The query itself should succeed (no top-level errors that block the whole response)
        var items = payload["data"]?["products"]?["items"]?.AsArray();
        Assert.NotNull(items);
        Assert.NotEmpty(items);

        // CostPrice should be null for unauthorized users
        var firstProduct = items[0]!;
        Assert.Null(firstProduct["costPrice"]);
        Assert.Null(firstProduct["costPriceCurrency"]);

        // HotChocolate should return AUTH_NOT_AUTHORIZED errors
        var errors = payload["errors"]?.AsArray();
        Assert.NotNull(errors);
        Assert.NotEmpty(errors);
        Assert.Contains(errors, error =>
            error?["extensions"]?["code"]?.GetValue<string>() == "AUTH_NOT_AUTHORIZED");
    }

    [Fact]
    public async Task CostPrice_WithInventoryManagerRole_ShouldReturnValue()
    {
        await fixture.ResetAsync();

        // Authenticated request with inventory-manager role
        using var authenticatedClient = fixture.CreateAuthenticatedClient("inventory-manager");
        var response = await authenticatedClient.PostAsJsonAsync("/graphql", new { query = CostPriceQuery });
        response.EnsureSuccessStatusCode();

        var payload = JsonNode.Parse(await response.Content.ReadAsStringAsync())!;

        // No errors expected
        Assert.Null(payload["errors"]);

        var items = payload["data"]?["products"]?["items"]?.AsArray();
        Assert.NotNull(items);
        Assert.NotEmpty(items);

        // CostPrice should be visible to authorized users
        var firstProduct = items[0]!;
        Assert.NotNull(firstProduct["costPrice"]);
        Assert.True(firstProduct["costPrice"]!.GetValue<decimal>() > 0);
        Assert.NotNull(firstProduct["costPriceCurrency"]);
    }
}
