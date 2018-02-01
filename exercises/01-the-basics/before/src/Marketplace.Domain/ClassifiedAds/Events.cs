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
        }
    }
}
