namespace Marketplace.Domain.ClassifiedAds
{
    public class ClassifiedAd
    {
        bool IsPublished { get; set; }
        bool WasSold { get; set; }
        bool WasRemoved { get; set; }

        string Title { get; set; }
        string Text { get; set; }
        double Price { get; set; }
        OwnerId Owner { get; set; } = OwnerId.Default;

        public void Register(ClassifiedAdId id, OwnerId owner)
        {
        }

        public void ChangeTitle(string title)
        {
        }

        public void ChangeText(string text)
        {
        }

        public void ChangePrice(double price)
        {
        }

        public void Publish()
        {
        }

        public void MarkAsSold()
        {
        }

        public void Remove()
        {
        }
    }
}
