using System.Text;
using WeatherMcpServer.Application.Interfaces;

namespace WeatherMcpServer.Infrastructure;

public class LinkBuilder : ILinkBuilder
{
    public Uri BuildUri(string baseUri, IEnumerable<string>? pathSegments = null, IDictionary<string, string?>? query = null)
    {
        if (string.IsNullOrWhiteSpace(baseUri))
            throw new ArgumentNullException(nameof(baseUri));
        var sb = new StringBuilder();
        sb.Append(baseUri.TrimEnd('/'));
        if (pathSegments != null)
        {
            foreach (var seg in pathSegments)
            {
                if (string.IsNullOrEmpty(seg)) continue;
                sb.Append('/');
                sb.Append(Uri.EscapeDataString(seg.Trim('/')));
            }
        }

        if (query != null && query.Any(kv => !string.IsNullOrEmpty(kv.Value)))
        {
            var q = string.Join("&", query
                .Where(kv => kv.Value != null)
                .Select(kv => $"{Uri.EscapeDataString(kv.Key)}={Uri.EscapeDataString(kv.Value!)}"));
            sb.Append('?');
            sb.Append(q);
        }

        return new Uri(sb.ToString(), UriKind.Absolute);
    }
}
