using System;
using System.Threading.Tasks;
using Marketplace.Projections;
using Raven.Client.Documents.Session;

namespace Marketplace.Framework
{
    public static class RavenDocumentSessionExtensions
    {
        public static async Task UpdateOrThrow<T>(this IAsyncDocumentSession session, string id, Action<T> update)
        {
            var doc = await session.LoadAsync<T>(id);
            if (doc == null)
                throw new ReadModelNotFoundException(typeof(T).Name, id);

            update(doc);
        }
    }
}
