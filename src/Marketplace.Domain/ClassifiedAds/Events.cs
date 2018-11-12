using System;

namespace Marketplace.Domain.ClassifiedAds
{
    public static class Events
    {
        public static class V1
        {
            public class ClassifiedAdRegistered
            {
                public Guid ClassifiedAdId { get; set; }
                public string Title { get; set; }
                public DateTimeOffset RegisteredAt { get; set; }
                public Guid Owner { get; set; }
                
                public override string ToString() 
                    => $"Classified Ad {ClassifiedAdId} registered.";      
            }

            public class ClassifiedAdTitleChanged
            {
                public Guid ClassifiedAdId { get; set; }
                public Guid Owner { get; set; }
                public string Title { get; set; }
                public DateTimeOffset ChangedAt { get; set; }
                
                public override string ToString() 
                    => $"Classified Ad title {ClassifiedAdId} changed to '{(Title?.Length > 25 ? $"{Title?.Substring(0, 22)}..." : Title )}'";
            }
            
            public class ClassifiedAdTextChanged
            {
                public Guid ClassifiedAdId { get; set; }
                public string Text { get; set; }
                public DateTimeOffset ChangedAt { get; set; }
                public Guid Owner { get; set; }
            }

            public class ClassifiedAdPriceChanged
            {
                public Guid ClassifiedAdId { get; set; }
                public double Price { get; set; }
                public DateTimeOffset ChangedAt { get; set; }
                public Guid Owner { get; set; }
            }
            
            public class ClassifiedAdPublished
            {
                public Guid ClassifiedAdId { get; set; }
                public string Title { get; set; }
                public string Text { get; set; }
                public DateTimeOffset PublishedAt { get; set; }
                public Guid Owner { get; set; }
            }
            
           
            public class ClassifiedAdSold
            {
                public Guid ClassifiedAdId { get; set; }
                public DateTimeOffset SoldAt { get; set; }
                public Guid Owner { get; set; }
            }

            public class ClassifiedAdRemoved
            {
                public Guid ClassifiedAdId { get; set; }
                public DateTimeOffset RemovedAt { get; set; }
                public Guid Owner { get; set; }
            }
        }
    }
}
