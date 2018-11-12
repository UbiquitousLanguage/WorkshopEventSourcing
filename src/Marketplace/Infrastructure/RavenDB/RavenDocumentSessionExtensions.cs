using System;
using System.Threading.Tasks;
using Marketplace.Projections;
using Raven.Client.Documents.Session;

namespace Marketplace.Infrastructure.RavenDB
{
    public static class RavenDocumentSessionExtensions
    {
        public static async Task Save<T>(this IAsyncDocumentSession session, string documentId, Action<T> update, bool throwOnNotFound = false) where T : new()
        {
            var doc = await session.LoadAsync<T>(documentId);
            
            if (doc == null)
            {
                if (throwOnNotFound)
                    throw new ReadModelNotFoundException(typeof(T).Name, documentId);
                
                doc = new T();
                await session.StoreAsync(doc);
            }
            
            update(doc);

            await session.SaveChangesAsync() ;
        }

        public static Task ThenSave<T>(this Func<IAsyncDocumentSession> getSession, string documentId, Action<T> action, bool throwOnNotFound = false) where T : new() 
            => getSession().Save(documentId, action, throwOnNotFound);

        public static async Task ThenDelete(this Func<IAsyncDocumentSession> getSession, string documentId)
        {
            using (var session = getSession())
            {
                session.Delete(documentId);
                await session.SaveChangesAsync();
            }
        }
    }
}
