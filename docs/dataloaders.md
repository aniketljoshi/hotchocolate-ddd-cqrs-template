# DataLoaders

Nested GraphQL fields can easily trigger N+1 queries.

In this template, `products` returns `ProductDto` items and each product resolves a nested `category` field. Without batching, ten products can produce ten category lookups.

`CategoryByIdDataLoader` solves that by:

1. collecting category ids requested during the GraphQL execution
2. issuing one repository query for the batch
3. hydrating the nested `category` field from the batched result

The integration test `ProductsQuery_ShouldBatchCategoryLoadingIntoSingleDatabaseQuery` verifies the behavior by counting SQL reads against the `categories` table and asserting the total is exactly `1`.
