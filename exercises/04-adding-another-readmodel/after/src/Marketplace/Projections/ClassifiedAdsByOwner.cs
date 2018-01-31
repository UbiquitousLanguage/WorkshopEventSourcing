using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Marketplace.Domain.ClassifiedAds;
using Marketplace.Framework;
using Raven.Client.Documents.Session;

namespace Marketplace.Projections
{
    public class ClassifiedAdsByOwner : Projection
    {
        private readonly Func<IAsyncDocumentSession> _getSession;
        
        public ClassifiedAdsByOwner(Func<IAsyncDocumentSession> getSession)
        {
            _getSession = getSession;
        }
        
        public override async Task Handle(object e)
        {
            using (var session = _getSession())
            {
                switch (e)
                {
                    case Events.V1.ClassifiedAdCreated x:
                        var id = DocumentId(x.Id);

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
                            Status = ClassifiedAdsByOwnerDocument.ClassifiedAdStatus.New,
                            Title = x.Title
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
            public ClassifiedAdStatus Status { get; set; }
        }
        
        public enum ClassifiedAdStatus
        {
            New,
            Active,
            Inactive,
            Sold
        }
    }
}
