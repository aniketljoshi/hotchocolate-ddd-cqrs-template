# Release Guide

## Prerequisites

- NuGet package id reserved or available: `AniketJoshi.HotChocolateDdd.Template`
- Repository secret configured: `NUGET_API_KEY`
- GitHub Actions enabled for the repository
- For manual dispatches, provide an explicit semantic version like `0.1.0`

## Release flow

1. Ensure `main` is green in CI.
2. Update docs or release notes if needed.
3. Create and push a version tag:

```bash
git tag v0.1.0
git push origin v0.1.0
```

4. The `release.yml` workflow will:
   - restore, build, and test the solution
   - pack `HotChocolateDddCqrsTemplate.TemplatePack.csproj`
   - push the `.nupkg` to NuGet.org

You can also run the workflow manually and provide the same package version without creating a tag first.

## Manual verification

After the workflow completes:

```bash
dotnet new install AniketJoshi.HotChocolateDdd.Template
dotnet new hc-ddd -n SampleService
dotnet build SampleService/SampleService.sln
```
