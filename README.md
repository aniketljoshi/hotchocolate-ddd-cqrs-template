# hotchocolate-ddd-cqrs-template

Production-leaning .NET 10 template with HotChocolate v15, DDD, CQRS, MediatR, an outbox-backed domain event pipeline, and OpenTelemetry tracing to Jaeger.

[![.NET](https://img.shields.io/badge/.NET-10.0_LTS-512BD4?logo=dotnet)](https://dotnet.microsoft.com)
[![HotChocolate](https://img.shields.io/badge/HotChocolate-v15-E10098)](https://chillicream.com)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](LICENSE)

GraphQL is the delivery layer.
Business logic stays in the application and domain layers.

## What Is Included

- HotChocolate v15 with thin query and mutation resolvers
- DDD/CQRS structure across `Domain`, `Application`, `Infrastructure`, and `Api`
- Outbox persistence plus background dispatch for domain events
- Category DataLoader with integration coverage for batching
- OpenTelemetry spans from GraphQL through MediatR and outbox processing
- `dotnet new` template packaging and release automation
- Domain, application, architecture, and integration tests

## Use As A Template

After the first NuGet release is published, install the template with:

```bash
dotnet new install AniketJoshi.HotChocolateDdd.Template
dotnet new hc-ddd -n MyCatalogService
```

The generated name replaces `HotChocolateDddCqrsTemplate` across solution, project, and namespace tokens.

## Run The Repository

```bash
docker compose up -d
dotnet run --project src/Api
```

- GraphQL: [http://localhost:5159/graphql](http://localhost:5159/graphql)
- Jaeger: [http://localhost:16686](http://localhost:16686)
- PostgreSQL: `localhost:55432`

## Architecture

```text
GraphQL Request
    |
    v
HotChocolate Resolver
    |
    v
MediatR Pipeline
  |- LoggingBehavior
  |- ValidationBehavior
  '- DomainEventDispatchBehavior
    |
    v
Command / Query Handler
    |
    v
Domain Aggregate
    |
    v
Outbox Message
    |
    v
Background Processor -> Notification Handler
```

## Observability

Development mode exports OTLP traces to Jaeger and includes custom spans for:

- GraphQL root fields
- MediatR request handling
- outbox persistence
- outbox polling and dispatch
- notification handlers

Regenerate the screenshot with:

```powershell
./scripts/capture-jaeger-trace.ps1
```

![Jaeger trace](docs/assets/jaeger-trace.png)

See [docs/operations/observability.md](docs/operations/observability.md).

## Project Structure

```text
src/
|-- Domain/
|-- Application/
|-- Infrastructure/
`-- Api/

tests/
|-- Domain.Tests/
|-- Application.Tests/
|-- Architecture.Tests/
`-- Integration.Tests/
```

## Docs

- [docs/architecture.md](docs/architecture.md)
- [docs/request-flow.md](docs/request-flow.md)
- [docs/outbox-pattern.md](docs/outbox-pattern.md)
- [docs/dataloaders.md](docs/dataloaders.md)
- [docs/operations/observability.md](docs/operations/observability.md)
- [docs/release.md](docs/release.md)
- [docs/decisions/ADR-001-domain-events-not-mediatr.md](docs/decisions/ADR-001-domain-events-not-mediatr.md)

## Template Packaging

Local package validation:

```bash
dotnet pack HotChocolateDddCqrsTemplate.TemplatePack.csproj -c Release -o artifacts /p:PackageVersion=0.1.0-local
dotnet new install ./artifacts/AniketJoshi.HotChocolateDdd.Template.0.1.0-local.nupkg
dotnet new hc-ddd -n SampleService
dotnet build SampleService/SampleService.sln
```

The GitHub release workflow publishes the first package after a `v*` tag push and a configured `NUGET_API_KEY` secret.

Release automation lives in:

- [.github/workflows/ci.yml](.github/workflows/ci.yml)
- [.github/workflows/release.yml](.github/workflows/release.yml)
- [.github/workflows/codeql.yml](.github/workflows/codeql.yml)

## Community

- [CONTRIBUTING.md](CONTRIBUTING.md)
- [SECURITY.md](SECURITY.md)
- issue templates in [.github/ISSUE_TEMPLATE](.github/ISSUE_TEMPLATE)
- PR template in [.github/PULL_REQUEST_TEMPLATE.md](.github/PULL_REQUEST_TEMPLATE.md)

## Roadmap

- [x] Week 1: core structure, Catalog module, README baseline
- [x] Week 2: tests, outbox pipeline, DataLoader, architecture checks
- [x] Week 3: template manifest, package project, CI/release automation
- [x] Launch hardening: community files, CodeQL, OpenTelemetry, Jaeger docs
- [ ] Publish the first NuGet package from GitHub
- [ ] Add auth and a second bounded context
