using Microsoft.AspNetCore.Authentication;

namespace Blog.Api.Auth;

public class BasicAuthenticationOptions : AuthenticationSchemeOptions
{
    public string? Username { get; set; }
    public string? Password { get; set; }
}