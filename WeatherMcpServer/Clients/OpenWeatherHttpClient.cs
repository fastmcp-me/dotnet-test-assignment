using Microsoft.Extensions.Logging;
using System.Text.Json;
namespace WeatherMcpServer.Clients;

public class OpenWeatherHttpClient(
    HttpClient httpClient,
    ILogger<OpenWeatherHttpClient> logger)
{
    public async Task<T> GetAsync<T>(string url)
    {
        logger.LogDebug("Making GET request to {Url}", url);

        var response = await httpClient.GetAsync(url);
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
