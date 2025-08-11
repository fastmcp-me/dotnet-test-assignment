﻿namespace WeatherMcpServer.Domain;

public class Result<T>
{
    public bool Success { get; }
    public T? Value { get; }
    public string? Error { get; }
    public Exception? Exception { get; }

    private Result(bool success, T? value, string? error, Exception? exception)
    {
        Success = success;
        Value = value;
        Error = error;
        Exception = exception;
    }

    public static Result<T> Ok(T value) => new(true, value, null, null);
    public static Result<T> Fail(string error, Exception? ex = null) => new(false, default, error, ex);
}
