using System.Net;
using System.Text.Json;
using WeatherMcpServer.Infrastructure.Exceptions;
using Microsoft.Extensions.Logging;

namespace WeatherMcpServer.Infrastructure;

public abstract class HttpProvider
{
    private readonly HttpClient _httpClient;
    private readonly ILogger _logger;

    protected HttpProvider(
        HttpClient httpClient,
        ILogger logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    protected async Task<TResponse?> GetAsync<TResponse>(string url)
        where TResponse : class, new()
    {
        try
        {
            var response = await _httpClient.GetAsync(url).ConfigureAwait(false);

            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                _logger.LogWarning("URL '{url}' returned not found result.", url);
                return null;
            }

            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

            var result = JsonSerializer.Deserialize<TResponse>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            return result;
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "Network error while fetching result via URL {url}'.", url);
            throw new ExternalServiceException($"Network error while fetching result via URL {url}'.");
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex, "Invalid JSON format while fetching result via URL {url}'.", url);
            throw new ExternalServiceException($"Invalid JSON format while fetching result via URL {url}'.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Unexpected error while fetching while fetching result via URL {url}");
            throw;
        }
    }
}
