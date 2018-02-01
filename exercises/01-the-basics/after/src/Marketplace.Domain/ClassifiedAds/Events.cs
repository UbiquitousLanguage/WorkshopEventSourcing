using System;

namespace Marketplace.Domain.ClassifiedAds
{
    public static class Events
    {
        public static class V1
        {
            public class ClassifiedAdCreated
            {
                public Guid Id { get; set; }
                public Guid Owner { get; set; }
                public string Title { get; set; }
                public DateTimeOffset CreatedAt { get; set; }
                public Guid CreatedBy { get; set; }

                public override string ToString()
                    => $"Classified Ad {Id} was created.";
            }

            public class ClassifiedAdRenamed
            {
                public Guid Id { get; set; }
                public string Title { get; set; }
                public DateTimeOffset RenamedAt { get; set; }
                public Guid RenamedBy { get; set; }

                public override string ToString()
                    => $"Classified Ad {Id} was renamed to '{(Title?.Length > 25 ? $"{Title?.Substring(0, 22)}..." : Title )}'";}
            
            public class ClassifiedAdTextUpdated
            {
                public Guid Id { get; set; }
                public string AdText { get; set; }
                public DateTimeOffset TextUpdatedAt { get; set; }
                public Guid TextUpdatedBy { get; set; }
            }

            public class ClassifiedAdPriceChanged
            {
                public Guid Id { get; set; }
                public double Price { get; set; }
                public DateTimeOffset PriceChangedAt { get; set; }
                public Guid PriceChangedBy { get; set; }
            }
        }
    }
}
