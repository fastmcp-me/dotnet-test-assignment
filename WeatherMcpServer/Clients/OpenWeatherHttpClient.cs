using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Text.Json;
using WeatherMcpServer.Options;
namespace WeatherMcpServer.Clients;

public class OpenWeatherHttpClient(
    HttpClient httpClient,
    IOptions<OpenWeatherOptions> options,
    ILogger<OpenWeatherHttpClient> logger)
{
    private readonly string _openWeatherBaseUrl = options.Value.BaseUrl;

    public async Task<T> GetAsync<T>(string url)
    {
        logger.LogDebug("Making GET request to {Url}", url);

        var response = await httpClient.GetAsync(_openWeatherBaseUrl + url);
        var content = await response.Content.ReadAsStringAsync();

        logger.LogDebug("Response from {Url}: {Content}", url, content);

        if (!response.IsSuccessStatusCode)
        {
            logger.LogWarning("Request to {Url} failed with status code {StatusCode} ({ReasonPhrase}). Response content: {Content}",
                url, (int)response.StatusCode, response.ReasonPhrase, content);
            throw new InvalidOperationException($"Request to {url} failed with status code {(int)response.StatusCode} ({response.ReasonPhrase}). Response content: {content}");
        }

        try
        {
            var result = JsonSerializer.Deserialize<T>(content) 
                ?? throw new InvalidOperationException($"Failed to deserialize response from {url}");
            return result;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Deserialization failed for response from {Url}. Content: {Content}", url, content);
            throw;
        }
    }
}
