using System.Collections.Concurrent;
using System.Xml.Linq;
using Microsoft.AspNetCore.DataProtection.Repositories;

namespace TeamFlow.IntegrationTests;

internal sealed class InMemoryKeyRepository : IXmlRepository
{
    private readonly ConcurrentQueue<XElement> elements = new();

    public IReadOnlyCollection<XElement> GetAllElements() => elements.ToArray();

    public void StoreElement(XElement element, string friendlyName) => elements.Enqueue(new XElement(element));
}
