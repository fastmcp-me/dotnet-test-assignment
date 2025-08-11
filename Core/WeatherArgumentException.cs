using System;
using System.Runtime.Serialization;

namespace Core;

[Serializable]
public class WeatherArgumentException : ArgumentException
{
    public WeatherArgumentException() { }

    public WeatherArgumentException(string message)
        : base(message) { }

    public WeatherArgumentException(string message, Exception innerException)
        : base(message, innerException) { }

    public WeatherArgumentException(string message, string paramName)
        : base(message, paramName) { }
}