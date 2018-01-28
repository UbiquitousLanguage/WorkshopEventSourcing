using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Marketplace.Domain.ClassifiedAds;
using Marketplace.Framework;
using Microsoft.AspNetCore.Diagnostics;
using Raven.Client.Documents.Session;

namespace Marketplace.Projections
{
    public class PublishedAdsProjection : Projection
    {
        private readonly Func<IAsyncDocumentSession> _openSession;

        public PublishedAdsProjection(Func<IAsyncDocumentSession> openSession)
        {
            _openSession = openSession;
        }

        public override async Task Handle(object e)
        {
            using (var session = _openSession())
            {
//                switch (e)
//                {
//                    case Events.V1.ClassifiedAdCreated x:
//                        var id = DocumentId(x.Id);
//
//                        var doc = await session.LoadAsync<MyClassifiedAds>(id);
//
//                        if (doc == null)
//                        {
//                            doc = new MyClassifiedAds
//                            {
//                                Id = id,
//                                ListOfAds = new List<MyClassifiedAds.MyClassifiedAd>()
//                            };
//                            await session.StoreAsync(doc);
//                        }
//
//                        doc.ListOfAds.Add(new MyClassifiedAds.MyClassifiedAd
//                        {
//                            Id = x.Id,
//                            Status = "New"
//                        });
//                        break;
//                }

                await session.SaveChangesAsync();
            }
        }

        private static string DocumentId(Guid id) => $"MyClassifiedAds/{id}";
    }

    public class PublishedAd
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public double Price { get; set; }
    }
}
