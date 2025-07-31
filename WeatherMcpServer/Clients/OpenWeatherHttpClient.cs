using Microsoft.Extensions.Logging;
using System.Net.Http.Json;
namespace WeatherMcpServer.Clients;

public class OpenWeatherHttpClient(
    HttpClient httpClient,
    ILogger<OpenWeatherHttpClient> logger)
{
    public async Task<T> GetAsync<T>(string url)
    {
        logger.LogDebug("Making GET request to {Url}", url);

        var response = await httpClient.GetAsync(url);
        if (!response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStringAsync();
            throw new InvalidOperationException($"Request to {url} failed with status code {(int)response.StatusCode} ({response.ReasonPhrase}). Response content: {content}");
        }
        
        return await response.Content.ReadFromJsonAsync<T>() 
               ?? throw new InvalidOperationException($"Failed to deserialize response from {url}");
    }
}
