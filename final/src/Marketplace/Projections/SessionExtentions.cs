using System;
using System.Threading.Tasks;
using Raven.Client.Documents.Session;

namespace Marketplace.Projections
{
    public static class SessionExtentions
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
