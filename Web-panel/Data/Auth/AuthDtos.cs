using System.Security.Claims;

namespace Diplom.Data.Auth;

public sealed record LoginRequest(string Email, string Password);

public sealed record AuthResponse(string AccessToken, DateTime ExpiresAt, AuthUserDto User);

public sealed record AuthUserDto(int Id, string Email, string FullName, string Role);

public sealed record SetPasswordRequest(string Email, string Password);

public sealed record WebLoginResult(AuthResponse Response, ClaimsPrincipal Principal);
