using System;
using System.Threading.Tasks;
using Marketplace.Domain.ClassifiedAds;
using Marketplace.Framework;
using Raven.Client.Documents.Session;

namespace Marketplace.Modules.ClassifiedAds.Projections
{
    public class ClassifiedAdsPendingActivation : Projection
    {
        private readonly Func<IAsyncDocumentSession> _getSession;

        public ClassifiedAdsPendingActivation(Func<IAsyncDocumentSession> getSession) => _getSession = getSession;

        public override async Task Handle(object e)
        {
            using (var session = _getSession())
            {
                switch (e)
                {
                    case Events.V1.ClassifiedAdPublished x:
                        var doc = new AdAwaitingActivation
                        {
                            Id = DocumentId(x.Id),
                            Title = x.Title,
                            Text = x.Text
                        };
                        await session.StoreAsync(doc);
                        break;

                    case Events.V1.ClassifiedAdRenamed x:
                        await session.UpdateOrThrow<AdAwaitingActivation>(DocumentId(x.Id), r => r.Title = x.Title);
                        break;

                    case Events.V1.ClassifiedAdTextUpdated x:
                        await session.UpdateOrThrow<AdAwaitingActivation>(DocumentId(x.Id), r => r.Text = x.AdText);
                        break;

                    case Events.V1.ClassifiedAdActivated x:
                        session.Delete(DocumentId(x.Id));
                        break;

                    case Events.V1.ClassifiedAdRemoved x:
                        session.Delete(DocumentId(x.Id));
                        break;
                }
                await session.SaveChangesAsync();
            }
        }

        private static string DocumentId(Guid id) => $"AdAwaitingActivation/{id}";
    }

    public class AdAwaitingActivation
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public string Text { get; set; }
    }
}
