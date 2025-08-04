using Domain.Common.Auth;
using System.Security.Claims;

namespace Api.Middlewares;

public class CurrentUserMiddleware
{
    private readonly RequestDelegate _next;

    public CurrentUserMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context, ICurrentUserContext userContext)
    {
        try
        {            
            var userIdClaim = context.User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim != null && int.TryParse(userIdClaim.Value, out int userId))
            {
                (userContext as CurrentUserContext)!.UserId = userId;
            }
        }
        catch { }

        await _next(context);
    }
}