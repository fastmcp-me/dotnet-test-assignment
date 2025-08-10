namespace WeatherMcpServer.Application.Interfaces;

public interface IProviderSelector
{
    IEnumerable<IWeatherProvider> Select(IEnumerable<IWeatherProvider> providers);
}
