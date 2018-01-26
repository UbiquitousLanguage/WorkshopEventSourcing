using System;

namespace Marketplace.Contracts
{
    public static class ClassifiedAds
    {
        public static class V1
        {
            public class CreateClassifiedAd
            {
                public Guid Id { get; set; }
                public Guid OwnerId { get; set; }
                public DateTime CreatedAt { get; set; }
                public Guid CreatedBy { get; set; }

                public override string ToString() => $"Creating Classified Ad {Id}";
            }
        }
    }
}
