namespace WeatherMcpServer.Business.Infrastructure.Exceptions;

public class BusinessException : Exception
{
    public string ErrorCode { get; }
    
    public BusinessException(string message, string errorCode = "BUSINESS_ERROR") 
        : base(message)
    {
        ErrorCode = errorCode;
    }
    
    public BusinessException(string message, string errorCode, Exception innerException) 
        : base(message, innerException)
    {
        ErrorCode = errorCode;
    }
}

public class WeatherServiceException : BusinessException
{
    public WeatherServiceException(string message) 
        : base(message, "WEATHER_SERVICE_ERROR")
    {
    }
    
    public WeatherServiceException(string message, Exception innerException) 
        : base(message, "WEATHER_SERVICE_ERROR", innerException)
    {
    }
}