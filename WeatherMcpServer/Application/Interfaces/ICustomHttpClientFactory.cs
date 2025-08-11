namespace WeatherMcpServer.Application.Interfaces;

public interface ICustomHttpClientFactory
{
    HttpClient GetClient(string providerName);
    HttpClient GetPreferredClient();
}
