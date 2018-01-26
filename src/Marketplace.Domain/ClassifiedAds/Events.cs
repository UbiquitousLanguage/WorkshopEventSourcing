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
                public DateTimeOffset CreatedAt { get; set; }
                public Guid CreatedBy { get; set; }
            }

            public class ClassifiedAdRenamed
            {
                public Guid Id { get; set; }
                public Title Title { get; set; }
            }

            public class ClassifiedAdPublished
            {
                public Guid Id { get; set; }
                public DateTimeOffset PublishedAt { get; set; }
                public Guid PublishedBy { get; set; }
            }

            public class ClassifiedAdMarkedAsSold
            {
                public Guid Id { get; set; }
                public DateTimeOffset MarkedAsSoldAt { get; set; }
                public Guid MarkedAsSoldBy { get; set; }
            }
        }
    }
}
