using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Marketplace.Domain.ClassifiedAds;
using Marketplace.Framework;
using Microsoft.AspNetCore.Diagnostics;
using Raven.Client.Documents.Session;

namespace Marketplace.Projections
{
    public class ActiveAdsProjection : Projection
    {
        private readonly Func<IAsyncDocumentSession> _openSession;

        public ActiveAdsProjection(Func<IAsyncDocumentSession> openSession)
        {
            _openSession = openSession;
        }

        public override async Task Handle(object e)
        {
            ActiveAd doc;
            using (var session = _openSession())
            {
                switch (e)
                {
                    case Events.V1.ClassifiedAdActivated x:
                        doc = new ActiveAd { Id = DocumentId(x.Id) };
                        await session.StoreAsync(doc);
                        break;

                    case Events.V1.ClassifiedAdDeactivated x:
                        session.Delete(DocumentId(x.Id));
                        break;
                }
                await session.SaveChangesAsync();
            }
        }

        private static string DocumentId(Guid id) => $"PublishedAd/{id}";
    }

    public class ActiveAd
    {
        public string Id { get; set; }
    }
}
