using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace HotChocolateDddCqrsTemplate.Integration.Tests.Fixtures;

/// <summary>
/// Test authentication handler that authenticates requests containing the
/// <c>X-Test-Role</c> header. The header value is added as a "role" claim,
/// which allows integration tests to exercise field-level authorization
/// policies without a real identity provider.
/// </summary>
public sealed class TestAuthHandler(
    IOptionsMonitor<AuthenticationSchemeOptions> options,
    ILoggerFactory logger,
    UrlEncoder encoder)
    : AuthenticationHandler<AuthenticationSchemeOptions>(options, logger, encoder)
{
    public const string SchemeName = "TestScheme";
    public const string RoleHeader = "X-Test-Role";

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        if (!Request.Headers.TryGetValue(RoleHeader, out var roleValues) ||
            string.IsNullOrWhiteSpace(roleValues.FirstOrDefault()))
        {
            return Task.FromResult(AuthenticateResult.NoResult());
        }

        var claims = roleValues
            .Where(value => !string.IsNullOrWhiteSpace(value))
            .Select(value => new Claim("role", value!))
            .ToList();

        claims.Add(new Claim(ClaimTypes.Name, "test-user"));

        var identity = new ClaimsIdentity(claims, SchemeName);
        var principal = new ClaimsPrincipal(identity);
        var ticket = new AuthenticationTicket(principal, SchemeName);

        return Task.FromResult(AuthenticateResult.Success(ticket));
    }
}
