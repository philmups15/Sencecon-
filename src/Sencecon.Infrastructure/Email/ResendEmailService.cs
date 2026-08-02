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
            <p>An administrator requested a password reset for your Sencecon account. Click the link below to set a new password — it expires in 24 hours.</p>
            <p><a href="{link}">Reset your password</a></p>
            <p>If you didn't expect this, you can ignore this email.</p>
            """;

        return SendAsync(toEmail, "Reset your Sencecon password", html, cancellationToken);
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
