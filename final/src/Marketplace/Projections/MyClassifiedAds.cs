using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Marketplace.Domain.ClassifiedAds;
using Marketplace.Framework;
using Raven.Client.Documents.Session;

namespace Marketplace.Projections
{
    public class MyClassifiedAdsProjection : Projection
    {
        private readonly Func<IAsyncDocumentSession> _openSession;
        
        public MyClassifiedAdsProjection(Func<IAsyncDocumentSession> openSession)
        {
            _openSession = openSession;
        }
        
        public override async Task Handle(object e)
        {
            using (var session = _openSession())
            {
                switch (e)
                {
                    case Events.V1.ClassifiedAdCreated x:
                        var id = DocumentId(x.Id);

                        var doc = await session.LoadAsync<MyClassifiedAds>(id);
                        
                        if (doc == null)
                        {
                            doc = new MyClassifiedAds
                            {
                                Id = id,
                                ListOfAds = new List<MyClassifiedAds.MyClassifiedAd>()
                            };
                            await session.StoreAsync(doc);
                        }
                        doc.ListOfAds.Add(new MyClassifiedAds.MyClassifiedAd
                        {
                            Id = x.Id,
                            Status = "New",
                            Title = x.Title
                        });
                        break;
                }

                await session.SaveChangesAsync();
            }
        }

        private static string DocumentId(Guid id) => $"MyClassifiedAds/{id}";
    }

    public class MyClassifiedAds
    {
        public string Id { get; set; }
        public List<MyClassifiedAd> ListOfAds { get; set; }

        public class MyClassifiedAd
        {
            public Guid Id { get; set; }
            public string Title { get; set; }
            public string Status { get; set; }
        }
        
        public enum ClassifiedAdStatus
        {
            Active,
            Inactive,
            Sold
        }
    }
}
