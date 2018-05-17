using System;
using System.Threading.Tasks;
using Marketplace.Framework.Logging;
using Raven.Client.Documents.Session;

namespace Marketplace.Framework
{
    public class RavenCheckpointStore : ICheckpointStore
    {
        private static readonly ILog Log = LogProvider.For<RavenCheckpointStore>();

        private readonly Func<IAsyncDocumentSession> _getSession;

        public RavenCheckpointStore(Func<IAsyncDocumentSession> getSession)
        {
            _getSession = getSession;
        }

        public async Task<T> GetLastCheckpoint<T>(string projection)
        {
            Log.Trace("Getting last checkpoint for {projection}...", projection);

            using (var session = _getSession())
            {
                var doc = await session.LoadAsync<CheckpointDocument>(GetCheckpointDocumentId(projection));

                if (doc != null)
                {
                    var checkpoint = (T) doc.Checkpoint;
                    Log.Debug("{projection} last checkpoint is {@checkpoint}", projection, checkpoint);
                    return checkpoint;
                }

                Log.Trace("Checkpoint for {projection} not found", projection);
                return default;
            }
        }

        public async Task SetCheckpoint<T>(T checkpoint, string projection)
        {
            Log.Trace("Setting checkpoint {@checkpoint} for {projection}...", checkpoint, projection);

            using (var session = _getSession())
            {
                var id = GetCheckpointDocumentId(projection);
                var doc = await session.LoadAsync<CheckpointDocument>(id);

                if (doc != null)
                    doc.Checkpoint = checkpoint;
                else
                    await session.StoreAsync(new CheckpointDocument {Checkpoint = checkpoint}, id);

                await session.SaveChangesAsync();

                Log.Debug("{projection} checkpoint set to {@checkpoint}", projection, checkpoint);
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
