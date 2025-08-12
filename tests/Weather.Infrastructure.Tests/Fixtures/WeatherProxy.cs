using System;
using System.Diagnostics;
using System.Net;

namespace Weather.Infrastructure.Tests.Fixtures;

public class WeatherProxy : IDisposable
{
    private Process? _devProxyProcess;

    public HttpClient HttpClient { get; init; }

    public WeatherProxy()
    {
        StartDevProxy();
        HttpClient = CreateHttpClient();
    }

    private HttpClient CreateHttpClient()
    {
        var httpClientHandler = new HttpClientHandler
        {
            Proxy = new WebProxy("http://127.0.0.1:8000"),
            UseProxy = true
        };
    
        return new HttpClient(httpClientHandler)
        {
            BaseAddress = new Uri("https://api.openweathermap.org/data/2.5/")
        };
    }

    private void StartDevProxy()
    {
        var startInfo = new ProcessStartInfo
        {
            FileName = GetDevProxyPath(),
            ArgumentList = {
                "--config-file",
                "devproxy/devproxyrc.json",
                "--port",
                "8000"
            },
            RedirectStandardOutput = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        _devProxyProcess = Process.Start(startInfo);

        if (_devProxyProcess == null)
        {
            throw new InvalidOperationException("Failed to start devproxy.");
        }
        var waiter = new ManualResetEvent(false);

        _devProxyProcess.OutputDataReceived += (sender, args) =>
        {
            if (args.Data != null && args.Data.Contains("Dev Proxy listening"))
            {
                waiter.Set();
            }
        };
        _devProxyProcess.BeginOutputReadLine();
        waiter.WaitOne();
    }

    public void Dispose()
    {
        StopDevProxy();
        HttpClient.Dispose();
    }

    private void StopDevProxy()
    {
        if (_devProxyProcess is not null && !_devProxyProcess.HasExited)
        {
            _devProxyProcess.Kill();
        }
    }

    private string GetDevProxyPath()
	{
		var paths = Environment.GetEnvironmentVariable("PATH")?.Split(Path.PathSeparator) ?? Array.Empty<string>();
		var appPaths = paths.SelectMany(path => new[] { "devproxy", "devproxy.exe" }.Select(program => Path.Combine(path, program)));

		var fullPath = appPaths.FirstOrDefault(File.Exists);

		if (string.IsNullOrEmpty(fullPath))
		{
			throw new Exception("devproxy is not found in PATH");
		}

		return fullPath;
	}
}