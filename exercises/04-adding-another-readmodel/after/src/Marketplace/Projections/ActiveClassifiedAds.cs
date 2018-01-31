using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Marketplace.Domain.ClassifiedAds;
using Marketplace.Framework;
using Microsoft.AspNetCore.Diagnostics;
using Raven.Client.Documents.Session;

namespace Marketplace.Projections
{
    public class ActiveClassifiedAds : Projection
    {
        private readonly Func<IAsyncDocumentSession> _openSession;

        public ActiveClassifiedAds(Func<IAsyncDocumentSession> getSession) => _openSession = getSession;

        public override async Task Handle(object e)
        {
            ActiveClassifiedAdDocument doc;
            using (var session = _openSession())
            {
                switch (e)
                {
                    case Events.V1.ClassifiedAdActivated x:
                        doc = new ActiveClassifiedAdDocument
                        {
                            Id = DocumentId(x.Id),
                            Title = x.Title,
                            Price = x.Price
                        };
                        await session.StoreAsync(doc);
                        break;

                    case Events.V1.ClassifiedAdRenamed x:
                        doc = await session.LoadAsync<ActiveClassifiedAdDocument>(DocumentId(x.Id));
                        doc.Title = x.Title;
                        break;

                    case Events.V1.ClassifiedAdPriceChanged x:
                        doc = await session.LoadAsync<ActiveClassifiedAdDocument>(DocumentId(x.Id));
                        doc.Price = x.Price;
                        break;

                    case Events.V1.ClassifiedAdDeactivated x:
                        session.Delete(DocumentId(x.Id));
                        break;
                }
                await session.SaveChangesAsync() ;
            }
        }

        private static string DocumentId(Guid id) => $"ActiveClassifiedAds/{id}";

    }

    public class ActiveClassifiedAdDocument
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public double Price { get; set; }
    }
}
