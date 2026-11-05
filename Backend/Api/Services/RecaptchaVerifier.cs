using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ApiPrueba.Services;

public sealed class RecaptchaVerifier
{
    private const string VerificationUrl = "https://www.google.com/recaptcha/api/siteverify";
    private readonly HttpClient _httpClient;
    private readonly string? _secretKey;
    private readonly ILogger<RecaptchaVerifier> _logger;

    public RecaptchaVerifier(HttpClient httpClient, IConfiguration configuration, ILogger<RecaptchaVerifier> logger)
    {
        _httpClient = httpClient;
        _secretKey = configuration["Recaptcha:SecretKey"];
        _logger = logger;
    }

    public async Task<bool> VerifyAsync(string? token, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(token) || string.IsNullOrWhiteSpace(_secretKey))
        {
            return false;
        }

        try
        {
            using var content = new FormUrlEncodedContent(new Dictionary<string, string>
            {
                ["secret"] = _secretKey,
                ["response"] = token
            });
            using var response = await _httpClient.PostAsync(VerificationUrl, content, cancellationToken);
            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning("Google reCAPTCHA verification returned HTTP {StatusCode}.", response.StatusCode);
                return false;
            }

            var result = await response.Content.ReadFromJsonAsync<VerificationResponse>(cancellationToken);
            return result?.Success == true;
        }
        catch (HttpRequestException exception)
        {
            _logger.LogWarning(exception, "Google reCAPTCHA verification could not be reached.");
            return false;
        }
        catch (TaskCanceledException exception) when (!cancellationToken.IsCancellationRequested)
        {
            _logger.LogWarning(exception, "Google reCAPTCHA verification timed out.");
            return false;
        }
        catch (JsonException exception)
        {
            _logger.LogWarning(exception, "Google reCAPTCHA returned an invalid verification response.");
            return false;
        }
    }

    private sealed record VerificationResponse(
        [property: JsonPropertyName("success")] bool Success);
}