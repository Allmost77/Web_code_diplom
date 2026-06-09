using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Diplom.Data.Auth;

public static class AuthEndpointExtensions
{
    public static RouteGroupBuilder MapAuthApi(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/auth").WithTags("Auth");

        group.MapPost("/login", async Task<Results<Ok<AuthResponse>, UnauthorizedHttpResult>> (
            LoginRequest request,
            AuthService authService) =>
        {
            var response = await authService.LoginAsync(request);
            return response is null ? TypedResults.Unauthorized() : TypedResults.Ok(response);
        });

        group.MapPost("/web-login", async Task<Results<Ok<AuthResponse>, UnauthorizedHttpResult>> (
            LoginRequest request,
            AuthService authService,
            HttpContext httpContext) =>
        {
            var result = await authService.LoginForWebAsync(request);

            if (result is null)
                return TypedResults.Unauthorized();

            await httpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                result.Principal,
                new AuthenticationProperties
                {
                    IsPersistent = true,
                    ExpiresUtc = result.Response.ExpiresAt
                });

            return TypedResults.Ok(result.Response);
        });

        group.MapPost("/logout", async Task<NoContent> (HttpContext httpContext) =>
        {
            await httpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return TypedResults.NoContent();
        }).RequireAuthorization();

        group.MapGet("/me", (ClaimsPrincipal user) =>
        {
            var id = int.TryParse(user.FindFirstValue(ClaimTypes.NameIdentifier), out var userId)
                ? userId
                : 0;

            return TypedResults.Ok(new AuthUserDto(
                id,
                user.FindFirstValue(ClaimTypes.Email) ?? "",
                user.FindFirstValue(ClaimTypes.Name) ?? "",
                user.FindFirstValue(ClaimTypes.Role) ?? ""));
        }).RequireAuthorization();

        return group;
    }

    public static RouteGroupBuilder MapDevelopmentAuthApi(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/auth/dev").WithTags("Auth");

        group.MapPost("/set-password", async Task<Results<NoContent, NotFound>> (
            SetPasswordRequest request,
            AuthService authService) =>
        {
            var updated = await authService.SetPasswordAsync(request);
            return updated ? TypedResults.NoContent() : TypedResults.NotFound();
        });

        return group;
    }
}
