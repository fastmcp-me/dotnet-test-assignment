using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeatherMcpServer.Application.Interfaces;

public interface ILinkBuilder
{
    Uri BuildUri(string baseUri, IEnumerable<string>? pathSegments = null, IDictionary<string, string?>? query = null);
}
