using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;

namespace Blog.Api.Auth;

public class BasicAuthenticationHandler : AuthenticationHandler<BasicAuthenticationOptions>
{
    public BasicAuthenticationHandler(
        IOptionsMonitor<BasicAuthenticationOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder,
        ISystemClock clock)
        : base(options, logger, encoder, clock)
    {
    }

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        var authHeaderStr = Request.Headers.Authorization.FirstOrDefault();
        if (authHeaderStr == null)
        {
            return Task.FromResult(AuthenticateResult.NoResult());
        }

        if (!AuthenticationHeaderValue.TryParse(authHeaderStr, out var authHeader))
        {
            return Task.FromResult(AuthenticateResult.Fail("Invalid authorization header"));
        }

        if (!authHeader.Scheme.Equals(Scheme.Name, StringComparison.OrdinalIgnoreCase))
        {
            return Task.FromResult(AuthenticateResult.Fail("Invalid authentication scheme"));
        }

        if (string.IsNullOrEmpty(authHeader.Parameter))
        {
            return Task.FromResult(AuthenticateResult.NoResult());
        }

        if (!Authenticate(authHeader.Parameter, Options, out var username))
        {
            return Task.FromResult(AuthenticateResult.Fail("Invalid username or password"));
        }

        return Task.FromResult(AuthenticateResult.Success(CreateAuthTicket(username!, Scheme.Name)));
    }

    private static bool Authenticate(string token, BasicAuthenticationOptions options, out string? username)
    {
        var credentialsStr = Encoding.UTF8.GetString(Convert.FromBase64String(token));
        var credentials = credentialsStr.Split(':');
        username = credentials[0];
        var password = credentials[1];

        if (username != options.Username || password != options.Password)
        {
            return false;
        }

        return true;
    }

    private static AuthenticationTicket CreateAuthTicket(string username, string authScheme)
    {
        var claims = new[] { new Claim("name", username), new Claim(ClaimTypes.Role, "Admin") };
        var identity = new ClaimsIdentity(claims, authScheme);
        var claimsPrincipal = new ClaimsPrincipal(identity);
        return new AuthenticationTicket(claimsPrincipal, authScheme);
    }
}