using System;
using System.Threading.Tasks;
using PublicClassifiedAdScreen.Tools;
using Raven.Client.Documents.Session;

namespace PublicClassifiedAdScreen
{
    public class PublicAdsWithSellerRating : Projection
    {
        private readonly Func<IAsyncDocumentSession> _openSession;

        public PublicAdsWithSellerRating(Func<IAsyncDocumentSession> getSession) => _openSession = getSession;

        public override async Task Handle(object e)
        {
            PublicClassifiedAdWithSellerRating doc;
            using (var session = _openSession())
            {
                switch (e)
                {
                    case ClassifiedAds.Events.V1.ClassifiedAdActivated x:
                        doc = new PublicClassifiedAdWithSellerRating
                        {
                            Id = DocumentId(x.Id),
                            Title = x.Title,
                            Price = x.Price
                        };
                        await session.StoreAsync(doc);
                        break;

                    case ClassifiedAds.Events.V1.ClassifiedAdDeactivated x:
                        session.Delete(DocumentId(x.Id));
                        break;

                    case Rating.Events.V1.DealRateAddedToUserRating x:
                        await session.UpdateIfFound<PublicClassifiedAdWithSellerRating>(
                            DocumentId(x.UserId), r => r.SellerRating = x.TotalRate);
                        break;
                }
                await session.SaveChangesAsync() ;
            }
        }

        private static string DocumentId(Guid id) => $"ActiveClassifiedAds/{id}";
    }

    public class PublicClassifiedAdWithSellerRating
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public double Price { get; set; }
        public int SellerRating { get; set; }
    }
}
