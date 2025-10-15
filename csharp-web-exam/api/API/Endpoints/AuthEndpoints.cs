using api.Application.DTOs;
using api.Application.Interfaces;
using log4net;
using Microsoft.AspNetCore.Mvc;

namespace api.API.Endpoints;

public static class AuthEndpoints
{
    private static readonly ILog _log = LogManager.GetLogger(typeof(AuthEndpoints));

    public static void MapAuthEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/auth")
            .WithTags("Authentication")
            .WithOpenApi();

        group.MapPost("/register", async (
            [FromBody] RegisterRequest request,
            IAuthService authService) =>
        {
            _log.Info($"Register endpoint called for user: {request.Username}");

            try
            {
                LoginResponse response = await authService.RegisterAsync(request);
                _log.Info($"User registered successfully: {request.Username}");
                return Results.Ok(response);
            }
            catch (InvalidOperationException ex)
            {
                _log.Warn($"Registration failed: {ex.Message}");
                return Results.BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _log.Error("Error in register endpoint", ex);
                return Results.Problem("An error occurred during registration");
            }
        })
        .WithName("Register")
        .WithSummary("Register a new user")
        .WithDescription("Creates a new user account and returns a JWT token")
        .Produces<LoginResponse>(200)
        .Produces(400);

        group.MapPost("/login", async (
            [FromBody] LoginRequest request,
            IAuthService authService) =>
        {
            _log.Info($"Login endpoint called for user: {request.Username}");

            try
            {
                LoginResponse? response = await authService.LoginAsync(request);

                if (response == null)
                {
                    _log.Warn($"Login failed for user: {request.Username}");
                    return Results.Unauthorized();
                }

                _log.Info($"User logged in successfully: {request.Username}");
                return Results.Ok(response);
            }
            catch (Exception ex)
            {
                _log.Error("Error in login endpoint", ex);
                return Results.Problem("An error occurred during login");
            }
        })
        .WithName("Login")
        .WithSummary("Authenticate user")
        .WithDescription("Authenticates a user and returns a JWT token")
        .Produces<LoginResponse>(200)
        .Produces(401);
    }
}
