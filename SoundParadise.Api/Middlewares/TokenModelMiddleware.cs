using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using SoundParadise.Api.Data;
using SoundParadise.Api.Services;

namespace SoundParadise.Api.Middlewares;

/// <summary>
///     Token expiration middleware.
/// </summary>
public class TokenExpirationMiddleware
{
    private readonly RequestDelegate _next;

    /// <summary>
    ///     Constructor for token expiration middleware
    /// </summary>
    /// <param name="next">Request Delegate.</param>
    public TokenExpirationMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    /// <summary>
    ///     Invoke.
    /// </summary>
    /// <param name="context">Http context.</param>
    /// <param name="tokenService">Token service.</param>
    /// <param name="serviceProvider">Service provider.</param>
    /// <returns>Task object.</returns>
    public async Task InvokeAsync(HttpContext context, TokenService tokenService, IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<SoundParadiseDbContext>();

        var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
        if (token != null)
        {
            var principal = tokenService.ValidateToken(token);
            var userIdClaim = principal?.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim != null && Guid.TryParse(userIdClaim.Value, out var userId))
            {
                var user = dbContext.Users
                    .Include(u => u.TokenJournals)
                    .ThenInclude(tj => tj.Token)
                    .FirstOrDefault(u => u.Id == userId);

                if (user != null)
                {
                    var currentTokenJournal =
                        user.TokenJournals.FirstOrDefault(tj => tj.Token.Token == token);
                    if (currentTokenJournal != null)
                    {
                        if (currentTokenJournal.Token.ExpirationDate.Date <= DateTime.UtcNow.Date)
                        {
                            currentTokenJournal.IsActive = false;
                            currentTokenJournal.DeactivatedAt = DateTime.UtcNow;
                            await dbContext.SaveChangesAsync();

                            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                            context.Response.ContentType = "application/json";
                            await context.Response.WriteAsync(JsonConvert.SerializeObject(new
                            {
                                message = "Token expired"
                            }));
                            return;
                        }

                        if (!currentTokenJournal.IsActive)
                        {
                            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                            context.Response.ContentType = "application/json";
                            await context.Response.WriteAsync(JsonConvert.SerializeObject(new
                            {
                                message = "Token deactivated"
                            }));
                            return;
                        }
                    }
                    else
                    {
                        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                        context.Response.ContentType = "application/json";
                        await context.Response.WriteAsync(JsonConvert.SerializeObject(new
                        {
                            message = "Invalid token"
                        }));
                        return;
                    }
                }
                else
                {
                    context.Response.StatusCode = StatusCodes.Status404NotFound;
                    context.Response.ContentType = "application/json";
                    await context.Response.WriteAsync(JsonConvert.SerializeObject(new
                    {
                        message = "User not found"
                    }));
                    return;
                }
            }
        }

        await _next(context);
    }
}