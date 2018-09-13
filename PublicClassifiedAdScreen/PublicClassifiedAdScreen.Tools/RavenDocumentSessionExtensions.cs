using System;
using System.Threading.Tasks;
using Raven.Client.Documents.Session;

namespace PublicClassifiedAdScreen.Tools
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
        
        public static async Task UpdateIfFound<T>(this IAsyncDocumentSession session, string id, Action<T> update)
        {
            var doc = await session.LoadAsync<T>(id);
            if (doc != null) update(doc);
        }
       
    }
}
