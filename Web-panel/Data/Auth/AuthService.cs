using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Diplom.Data;
using Diplom.Data.DbContext;
using Diplom.Data.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Diplom.Data.Auth;

public sealed class AuthService(
    IDbContextFactory<AppDbContext> dbContextFactory,
    IOptions<JwtOptions> jwtOptionsAccessor)
{
    private const string CookieAuthenticationType = "DiplomCookie";

    private readonly JwtOptions jwtOptions = jwtOptionsAccessor.Value;
    private readonly PasswordHasher<User> passwordHasher = new();

    public async Task<AuthResponse?> LoginAsync(LoginRequest request)
    {
        var user = await FindValidUserAsync(request);
        return user is null ? null : CreateAuthResponse(user);
    }

    public async Task<WebLoginResult?> LoginForWebAsync(LoginRequest request)
    {
        var user = await FindValidUserAsync(request);

        if (user is null)
            return null;

        return new WebLoginResult(CreateAuthResponse(user), CreatePrincipal(user));
    }

    public async Task<bool> SetPasswordAsync(SetPasswordRequest request)
    {
        var email = NormalizeEmail(request.Email);

        if (email is null || string.IsNullOrWhiteSpace(request.Password))
            return false;

        await using var dbContext = await dbContextFactory.CreateDbContextAsync();
        var user = await dbContext.Users
            .FirstOrDefaultAsync(user => user.Email != null && user.Email.ToLower() == email);

        if (user is null)
            return false;

        user.PasswordHash = passwordHasher.HashPassword(user, request.Password);
        await dbContext.SaveChangesAsync();
        return true;
    }

    private async Task<User?> FindValidUserAsync(LoginRequest request)
    {
        var email = NormalizeEmail(request.Email);

        if (email is null || string.IsNullOrWhiteSpace(request.Password))
            return null;

        await using var dbContext = await dbContextFactory.CreateDbContextAsync();

        var user = await dbContext.Users
            .AsNoTracking()
            .Include(user => user.Role)
            .FirstOrDefaultAsync(user => user.Email != null && user.Email.ToLower() == email);

        return user is null || !VerifyPassword(user, request.Password)
            ? null
            : user;
    }

    private bool VerifyPassword(User user, string password)
    {
        try
        {
            var result = passwordHasher.VerifyHashedPassword(user, user.PasswordHash, password);
            return result is PasswordVerificationResult.Success or PasswordVerificationResult.SuccessRehashNeeded;
        }
        catch (FormatException)
        {
            return false;
        }
    }

    private AuthResponse CreateAuthResponse(User user)
    {
        var issuedAt = DateTime.UtcNow;
        var expiresAt = issuedAt.AddMinutes(jwtOptions.ExpirationMinutes);
        var userDto = CreateUserDto(user);

        var token = new JwtSecurityToken(
            issuer: jwtOptions.Issuer,
            audience: jwtOptions.Audience,
            claims: CreateClaims(user, userDto),
            notBefore: issuedAt,
            expires: expiresAt,
            signingCredentials: new SigningCredentials(
                jwtOptions.CreateSecurityKey(),
                SecurityAlgorithms.HmacSha256));

        var accessToken = new JwtSecurityTokenHandler().WriteToken(token);

        return new AuthResponse(accessToken, expiresAt, userDto);
    }

    private static ClaimsPrincipal CreatePrincipal(User user)
    {
        var userDto = CreateUserDto(user);
        var identity = new ClaimsIdentity(CreateClaims(user, userDto), CookieAuthenticationType);

        return new ClaimsPrincipal(identity);
    }

    private static List<Claim> CreateClaims(User user, AuthUserDto userDto) =>
    [
        new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
        new(ClaimTypes.NameIdentifier, user.Id.ToString()),
        new(ClaimTypes.Email, userDto.Email),
        new(ClaimTypes.Name, userDto.FullName),
        new(ClaimTypes.Role, userDto.Role),
        new("roleId", user.RoleId.ToString())
    ];

    private static AuthUserDto CreateUserDto(User user) =>
        new(
            user.Id,
            user.Email ?? "",
            DisplayText.UserName(user.Id, user.FullName),
            user.Role?.Name ?? "User");

    private static string? NormalizeEmail(string? email)
    {
        var normalizedEmail = email?.Trim();
        return string.IsNullOrWhiteSpace(normalizedEmail)
            ? null
            : normalizedEmail.ToLowerInvariant();
    }
}
