using HotChocolateDddCqrsTemplate.Domain.Catalog;
using NetArchTest.Rules;

namespace HotChocolateDddCqrsTemplate.Architecture.Tests;

public sealed class LayerDependencyTests
{
    [Fact]
    public void Domain_ShouldNotDependOnFrameworkPackages()
    {
        var result = Types.InAssembly(typeof(Product).Assembly)
            .Should()
            .NotHaveDependencyOnAny(
                "MediatR",
                "HotChocolate",
                "Microsoft.EntityFrameworkCore",
                "FluentValidation",
                "ErrorOr")
            .GetResult();

        AssertArchitecture(result);
    }

    [Fact]
    public void Application_ShouldNotDependOnApiLayer()
    {
        var result = Types.InAssembly(typeof(Application.DependencyInjection).Assembly)
            .Should()
            .NotHaveDependencyOn("HotChocolateDddCqrsTemplate.Api")
            .GetResult();

        AssertArchitecture(result);
    }

    [Fact]
    public void Infrastructure_ShouldNotDependOnApiLayer()
    {
        var result = Types.InAssembly(typeof(Infrastructure.DependencyInjection).Assembly)
            .Should()
            .NotHaveDependencyOn("HotChocolateDddCqrsTemplate.Api")
            .GetResult();

        AssertArchitecture(result);
    }

    private static void AssertArchitecture(TestResult result)
    {
        Assert.True(
            result.IsSuccessful,
            "Architecture rule failed for: " + string.Join(", ", result.FailingTypes ?? []));
    }
}
