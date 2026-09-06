using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;

namespace Sencecon.API.Middleware;

// Gates the Swagger UI and spec behind HTTP Basic auth. Credentials come from
// configuration (Swagger:User / Swagger:Password) — set as environment variables
// in deployed environments. When they are not configured the docs are exposed in
// Development (local convenience) and hidden (404) everywhere else.
public class SwaggerBasicAuthMiddleware
{
    private readonly RequestDelegate _next;
    private readonly string? _user;
    private readonly string? _password;
    private readonly bool _isDevelopment;

    public SwaggerBasicAuthMiddleware(RequestDelegate next, IConfiguration configuration, IHostEnvironment environment)
    {
        _next = next;
        _user = configuration["Swagger:User"];
        _password = configuration["Swagger:Password"];
        _isDevelopment = environment.IsDevelopment();
    }

    public async Task InvokeAsync(HttpContext context)
    {
        if (!context.Request.Path.StartsWithSegments("/swagger"))
        {
            await _next(context);
            return;
        }

        if (string.IsNullOrEmpty(_user) || string.IsNullOrEmpty(_password))
        {
            if (_isDevelopment)
            {
                await _next(context);
                return;
            }

            context.Response.StatusCode = StatusCodes.Status404NotFound;
            return;
        }

        if (TryGetCredentials(context.Request, out var user, out var password)
            && FixedTimeEquals(user, _user)
            && FixedTimeEquals(password, _password))
        {
            await _next(context);
            return;
        }

        context.Response.Headers.WWWAuthenticate = "Basic realm=\"Sencecon API docs\", charset=\"UTF-8\"";
        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
    }

    private static bool TryGetCredentials(HttpRequest request, out string user, out string password)
    {
        user = string.Empty;
        password = string.Empty;

        if (!AuthenticationHeaderValue.TryParse(request.Headers.Authorization, out var header)
            || !string.Equals(header.Scheme, "Basic", StringComparison.OrdinalIgnoreCase)
            || string.IsNullOrEmpty(header.Parameter))
        {
            return false;
        }

        string decoded;
        try
        {
            decoded = Encoding.UTF8.GetString(Convert.FromBase64String(header.Parameter));
        }
        catch (FormatException)
        {
            return false;
        }

        var separator = decoded.IndexOf(':');
        if (separator < 0)
        {
            return false;
        }

        user = decoded[..separator];
        password = decoded[(separator + 1)..];
        return true;
    }

    private static bool FixedTimeEquals(string a, string b) =>
        CryptographicOperations.FixedTimeEquals(Encoding.UTF8.GetBytes(a), Encoding.UTF8.GetBytes(b));
}
