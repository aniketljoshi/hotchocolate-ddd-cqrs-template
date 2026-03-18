# API Testing

The repository now supports a full local test loop through Docker Compose.

## Start the stack

```bash
docker compose up --build -d
```

Useful endpoints:

- GraphQL: [http://localhost:5159/graphql](http://localhost:5159/graphql)
- Health: [http://localhost:5159/healthz](http://localhost:5159/healthz)
- Jaeger: [http://localhost:16686](http://localhost:16686)

Development mode seeds a small sample catalog automatically on first startup so the GraphQL operations can be exercised immediately.

## Import the Insomnia collection

1. Open Insomnia.
2. Choose `Create` -> `Import`.
3. Select [docs/api-testing/insomnia/hotchocolate-ddd-cqrs-template.insomnia.json](C:\Users\inaj7\source\repos\web2\hotchocolate-ddd-cqrs-template\docs\api-testing\insomnia\hotchocolate-ddd-cqrs-template.insomnia.json).
4. Import the workspace and keep the default environment values unless you changed local ports.

## Suggested request order

1. `Health Check`
2. `List Categories`
3. `List Products`
4. `Get Product By Id`
5. `Create Product`
6. `Update Product Price`

The collection points at `http://localhost:5159` and includes seeded IDs for one category and one product.

## Seeded data reference

The fixed sample IDs are documented in [docs/api-testing/reference.md](C:\Users\inaj7\source\repos\web2\hotchocolate-ddd-cqrs-template\docs\api-testing\reference.md).

## Stop the stack

```bash
docker compose down -v
```
