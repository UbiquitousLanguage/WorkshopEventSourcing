using System;
using System.Threading.Tasks;
using Marketplace.Framework.Logging;
using Raven.Client.Documents.Session;

namespace Marketplace.Framework
{
    public class RavenCheckpointStore : ICheckpointStore
    {
        private readonly Func<IAsyncDocumentSession> _getSession;

        public RavenCheckpointStore(Func<IAsyncDocumentSession> getSession)
        {
            _getSession = getSession;
        }

        public async Task<T> GetLastCheckpoint<T>(string projection)
        {
            using (var session = _getSession())
            {
                var doc = await session.LoadAsync<CheckpointDocument>(GetCheckpointDocumentId(projection));

                if (doc == null) return default;

                var checkpoint = (T) doc.Checkpoint;
                return checkpoint;
            }
        }

        public async Task SetCheckpoint<T>(T checkpoint, string projection)
        {
            using (var session = _getSession())
            {
                var id = GetCheckpointDocumentId(projection);
                var doc = await session.LoadAsync<CheckpointDocument>(id);

                if (doc != null)
                    doc.Checkpoint = checkpoint;
                else
                    await session.StoreAsync(new CheckpointDocument {Checkpoint = checkpoint}, id);

                await session.SaveChangesAsync();
            }
        }

        private static string GetCheckpointDocumentId(string projection) => 
            $"checkpoints/{projection.ToLowerInvariant()}";

        private class CheckpointDocument
        {
            public object Checkpoint { get; set; }
        }
    }
}
