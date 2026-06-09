using System.IdentityModel.Tokens.Jwt;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Diplom.Data.Auth;

public sealed class JwtAuthenticationHandler(
    IOptionsMonitor<AuthenticationSchemeOptions> options,
    ILoggerFactory logger,
    UrlEncoder encoder,
    IOptions<JwtOptions> jwtOptionsAccessor)
    : AuthenticationHandler<AuthenticationSchemeOptions>(options, logger, encoder)
{
    private readonly JwtOptions jwtOptions = jwtOptionsAccessor.Value;

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        var authorizationHeader = Request.Headers.Authorization.ToString();

        if (string.IsNullOrWhiteSpace(authorizationHeader) ||
            !authorizationHeader.StartsWith(JwtAuthenticationDefaults.BearerPrefix, StringComparison.OrdinalIgnoreCase))
        {
            return Task.FromResult(AuthenticateResult.NoResult());
        }

        var token = authorizationHeader[JwtAuthenticationDefaults.BearerPrefix.Length..].Trim();

        if (string.IsNullOrWhiteSpace(token))
            return Task.FromResult(AuthenticateResult.NoResult());

        try
        {
            var handler = new JwtSecurityTokenHandler();
            var principal = handler.ValidateToken(
                token,
                jwtOptions.CreateValidationParameters(),
                out var validatedToken);

            if (validatedToken is not JwtSecurityToken jwtToken ||
                !jwtToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.Ordinal))
            {
                return Task.FromResult(AuthenticateResult.Fail("Invalid token algorithm."));
            }

            var ticket = new AuthenticationTicket(principal, Scheme.Name);
            return Task.FromResult(AuthenticateResult.Success(ticket));
        }
        catch (SecurityTokenException ex)
        {
            return Task.FromResult(AuthenticateResult.Fail(ex));
        }
        catch (ArgumentException ex)
        {
            return Task.FromResult(AuthenticateResult.Fail(ex));
        }
    }
}
