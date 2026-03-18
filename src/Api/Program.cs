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

await InitializeDatabaseAsync(app.Services, app.Configuration);

app.MapGet("/healthz", () => Results.Ok(new { status = "ok" }));
app.MapGet("/", () => Results.Redirect("/graphql"));
app.MapGraphQL("/graphql");

app.Run();

static async Task InitializeDatabaseAsync(IServiceProvider services, IConfiguration configuration)
{
    using var scope = services.CreateScope();
    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    await dbContext.Database.EnsureCreatedAsync();

    if (configuration.GetValue<bool>("SampleData:Enabled"))
    {
        await SampleCatalogData.SeedAsync(dbContext);
    }
}

public partial class Program;
