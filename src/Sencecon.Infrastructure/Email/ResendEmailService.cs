using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Microsoft.Extensions.Options;
using Sencecon.Application.Common.Interfaces;

namespace Sencecon.Infrastructure.Email;

public class ResendEmailService : IEmailService
{
    private readonly HttpClient _httpClient;
    private readonly ResendSettings _resendSettings;
    private readonly FrontendSettings _frontendSettings;

    public ResendEmailService(HttpClient httpClient, IOptions<ResendSettings> resendSettings, IOptions<FrontendSettings> frontendSettings)
    {
        _httpClient = httpClient;
        _resendSettings = resendSettings.Value;
        _frontendSettings = frontendSettings.Value;

        _httpClient.BaseAddress ??= new Uri("https://api.resend.com/");
    }

    public Task SendPasswordResetEmailAsync(string toEmail, string displayName, string rawToken, CancellationToken cancellationToken)
    {
        var link = $"{_frontendSettings.BaseUrl.TrimEnd('/')}/?resetToken={Uri.EscapeDataString(rawToken)}";
        var html = $"""
            <p>Hi {WebUtility.HtmlEncode(displayName)},</p>
            <p>A password reset was requested for your Sencecon account. Click the link below to set a new password — it expires in 24 hours.</p>
            <p><a href="{link}">Reset your password</a></p>
            <p>If you didn't request this, you can safely ignore this email — your password won't change.</p>
            """;

        return SendAsync(toEmail, "Reset your Sencecon password", html, cancellationToken);
    }

    public Task SendLoginAlertEmailAsync(string toEmail, string displayName, DateTimeOffset whenUtc, string? ipAddress, string? userAgent, CancellationToken cancellationToken)
    {
        var when = whenUtc.ToUniversalTime().ToString("dddd, d MMMM yyyy 'at' HH:mm 'UTC'");
        var link = $"{_frontendSettings.BaseUrl.TrimEnd('/')}/";
        var html = $"""
            <p>Hi {WebUtility.HtmlEncode(displayName)},</p>
            <p>Your Sencecon account was just signed in to.</p>
            <ul>
              <li><strong>When:</strong> {WebUtility.HtmlEncode(when)}</li>
              <li><strong>IP address:</strong> {WebUtility.HtmlEncode(ipAddress ?? "unknown")}</li>
              <li><strong>Device:</strong> {WebUtility.HtmlEncode(userAgent ?? "unknown")}</li>
            </ul>
            <p>If this was you, no action is needed. If you don't recognise this sign-in, <a href="{link}">reset your password</a> right away.</p>
            """;

        return SendAsync(toEmail, "New sign-in to your Sencecon account", html, cancellationToken);
    }

    private async Task SendAsync(string toEmail, string subject, string htmlBody, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(_resendSettings.ApiKey))
        {
            // Fail loudly rather than silently dropping the email — a missing key
            // means Resend:ApiKey hasn't been configured yet (see Railway env vars).
            throw new InvalidOperationException("Resend:ApiKey is not configured — cannot send email.");
        }

        using var request = new HttpRequestMessage(HttpMethod.Post, "emails")
        {
            Content = JsonContent.Create(new
            {
                from = _resendSettings.FromAddress,
                to = new[] { toEmail },
                subject,
                html = htmlBody
            })
        };
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _resendSettings.ApiKey);

        var response = await _httpClient.SendAsync(request, cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            var body = await response.Content.ReadAsStringAsync(cancellationToken);
            throw new InvalidOperationException($"Resend API request failed ({(int)response.StatusCode}): {body}");
        }
    }
}
