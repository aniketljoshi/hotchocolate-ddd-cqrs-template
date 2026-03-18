using HotChocolateDddCqrsTemplate.Api;
using HotChocolateDddCqrsTemplate.Application;
using HotChocolateDddCqrsTemplate.Infrastructure;
using HotChocolateDddCqrsTemplate.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

var connectionString =
    builder.Configuration.GetConnectionString("CatalogDb") ??
    "Host=localhost;Port=55432;Database=hotchocolate_template;Username=postgres;Password=postgres";

builder.Services
    .AddApplication()
    .AddInfrastructure(builder.Configuration, connectionString)
    .AddApi();

var app = builder.Build();

await EnsureDatabaseAsync(app.Services);

app.MapGet("/", () => Results.Redirect("/graphql"));
app.MapGraphQL("/graphql");

app.Run();

static async Task EnsureDatabaseAsync(IServiceProvider services)
{
    using var scope = services.CreateScope();
    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    await dbContext.Database.EnsureCreatedAsync();
}

public partial class Program;
