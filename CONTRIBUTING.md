# Contributing

Thanks for helping improve the template.

## Local setup

```bash
docker-compose up -d
dotnet restore HotChocolateDddCqrsTemplate.sln
dotnet test HotChocolateDddCqrsTemplate.sln
```

## Working agreement

- Keep GraphQL resolvers thin.
- Keep domain events framework-agnostic.
- Prefer additive, well-tested changes.
- Update docs and sample requests when behavior changes.

## Template validation

Before changing packaging or naming behavior, validate the template flow:

```bash
dotnet pack HotChocolateDddCqrsTemplate.TemplatePack.csproj -c Release -o artifacts /p:PackageVersion=0.0.0-local
dotnet new install ./artifacts/AniketJoshi.HotChocolateDdd.Template.0.0.0-local.nupkg
dotnet new hc-ddd -n SampleService
dotnet build SampleService/SampleService.sln
```

## Pull requests

- Keep PRs focused.
- Include verification notes.
- Call out any template-breaking rename or packaging impact.
