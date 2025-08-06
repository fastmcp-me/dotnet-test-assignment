using Humanizer;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Text.Json;
using WeatherMcpServer.Options;
using WeatherMcpServer.Presenters;
namespace WeatherMcpServer.Clients;

public class OpenWeatherHttpClient(
    HttpClient httpClient,
    IOptions<OpenWeatherOptions> options,
    ILogger<OpenWeatherHttpClient> logger)
{
    private readonly string _openWeatherBaseUrl = options.Value.BaseUrl;

    public async Task<JsonDocument> GetJsonAsync(string url)
    {
        var fullUrl = _openWeatherBaseUrl + url;
        var maskedUrl = MaskApiKey(fullUrl);

        logger.LogDebug("Making GET request to {MaskedUrl}", maskedUrl);

        var response = await httpClient.GetAsync(fullUrl);
        var content = await response.Content.ReadAsStringAsync();

        logger.LogDebug("Response from {MaskedUrl}: {Content}", maskedUrl, content);

        JsonDocument result;
        try
        {
            result = JsonDocument.Parse(content);
        }
        catch (Exception ex)
        {
            var preview = content.Truncate(100);
            logger.LogError(ex, "Failed to parse JSON response from {MaskedUrl}. The content might not be valid JSON or is in an unexpected format. Content preview: {Preview}", maskedUrl, preview);

            throw new InvalidOperationException($"Failed to parse JSON response from {maskedUrl}. The content might not be valid JSON or is in an unexpected format. Content preview: {preview}");
        }

        if (!response.IsSuccessStatusCode)
        {
            var errorMessage = result.RootElement.GetErrorMessage();
            logger.LogWarning("Request to {MaskedUrl} failed with status code {StatusCode} ({ReasonPhrase}). Message: {ErrorMessage}",
                maskedUrl, (int)response.StatusCode, response.ReasonPhrase, errorMessage);

            throw new InvalidOperationException($"{errorMessage}");
        }

        return result;
    }

    private static string MaskApiKey(string url)
    {
        var uri = new Uri(url);
        var query = System.Web.HttpUtility.ParseQueryString(uri.Query);

        if (query.AllKeys.Contains("appid"))
        {
            query["appid"] = "***";
        }

        var maskedQuery = query.ToString();
        var maskedUrl = $"{uri.Scheme}://{uri.Host}{uri.AbsolutePath}?{maskedQuery}";

        return maskedUrl;
    }

}
