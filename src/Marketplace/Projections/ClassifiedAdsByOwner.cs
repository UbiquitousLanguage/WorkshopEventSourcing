using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Marketplace.Domain.ClassifiedAds;
using Marketplace.Framework;
using Raven.Client.Documents.Session;

namespace Marketplace.Projections
{
    public class ClassifiedAdsByOwner : Projection
    {
        private readonly Func<IAsyncDocumentSession> _getSession;
        
        public ClassifiedAdsByOwner(Func<IAsyncDocumentSession> getSession) => _getSession = getSession;

        public override async Task Handle(object e)
        {
            using (var session = _getSession())
            {
                switch (e)
                {
                    case Events.V1.ClassifiedAdCreated x:
                        var id = DocumentId(x.Owner);

                        var doc = await session.LoadAsync<ClassifiedAdsByOwnerDocument>(id);                    
                        if (doc == null)
                        {
                            doc = new ClassifiedAdsByOwnerDocument
                            {
                                Id = id,
                                ListOfAds = new List<ClassifiedAdsByOwnerDocument.MyClassifiedAd>()
                            };
                            await session.StoreAsync(doc);
                        }
                        
                        doc.ListOfAds.Add(new ClassifiedAdsByOwnerDocument.MyClassifiedAd
                        {
                            Id = x.Id,
                            Status = "New",
                            Title = x.Title
                        });
                        break;
                    
                    case Events.V1.ClassifiedAdPriceChanged x:
                        await session.UpdateOrThrow<ClassifiedAdsByOwnerDocument>(DocumentId(x.Owner),
                            r =>
                            {
                                var ad = r.ListOfAds.First(a => a.Id == x.Id);
                                ad.Price = x.Price;
                            });
                        break;
                    
                    case Events.V1.ClassifiedAdRenamed x:
                        await session.UpdateOrThrow<ClassifiedAdsByOwnerDocument>(DocumentId(x.Owner),
                            r =>
                            {
                                var ad = r.ListOfAds.First(a => a.Id == x.Id);
                                ad.Title = x.Title;
                            });
                        break;
                }

                await session.SaveChangesAsync();
            }
        }

        private static string DocumentId(Guid id) => $"ClassifiedAdsByOwner/{id}";
    }

    public class ClassifiedAdsByOwnerDocument
    {
        public string Id { get; set; }
        public List<MyClassifiedAd> ListOfAds { get; set; }

        public class MyClassifiedAd
        {
            public Guid Id { get; set; }
            public string Title { get; set; }
            public string Status { get; set; }
            public double Price { get; set; }
        }
        
        public enum ClassifiedAdStatus
        {
            Active,
            Inactive,
            Sold
        }
    }
}
