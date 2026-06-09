using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace Diplom.Data.Auth;

public sealed class JwtOptions
{
    public const string SectionName = "Jwt";

    public string Issuer { get; set; } = "";
    public string Audience { get; set; } = "";
    public string SecretKey { get; set; } = "";
    public int ExpirationMinutes { get; set; } = 60;

    public SymmetricSecurityKey CreateSecurityKey()
    {
        if (string.IsNullOrWhiteSpace(SecretKey) || SecretKey.Length < 32)
            throw new InvalidOperationException("JWT SecretKey must contain at least 32 characters.");

        return new SymmetricSecurityKey(Encoding.UTF8.GetBytes(SecretKey));
    }

    public TokenValidationParameters CreateValidationParameters() =>
        new()
        {
            ValidateIssuer = true,
            ValidIssuer = Issuer,
            ValidateAudience = true,
            ValidAudience = Audience,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = CreateSecurityKey(),
            ValidateLifetime = true,
            ClockSkew = TimeSpan.FromMinutes(1)
        };
}
