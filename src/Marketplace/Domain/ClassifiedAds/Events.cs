namespace Marketplace.Domain.ClassifiedAds
{
    using System;
    using Marketplace.Domain.Messages;

    public static class Events
    {
        public static class V1
        {
            public class ClassifiedAdCreated
            {
                public ClassifiedAdCreated()
                {
                    Pictures = new Shared.V1.Picture[0];
                }
                
                public Guid                Id                 { get; set; }
                public string              Title              { get; set; }
                public string              Description        { get; set; }
                public string              ContactEmail       { get; set; }
                public string              ContactPhoneNumber { get; set; }
                public Guid                CategoryId         { get; set; }
                public Shared.V1.Picture[] Pictures           { get; set; }
                public Guid                OwnerId            { get; set; }
                public DateTimeOffset      CreatedAt          { get; set; }
                public Guid                CreatedBy          { get; set; }
            }

            public class ClassifiedAdPublished
            {
                public Guid           Id          { get; set; }
                public string         Title       { get; set; }
                public Guid           CategoryId  { get; set; }
                public Guid           OwnerId     { get; set; }
                public DateTimeOffset PublishedAt { get; set; }
                public Guid           PublishedBy { get; set; }
            }

            public class ClassifiedAdMarkedAsSold
            {
                public Guid           Id             { get; set; }
                public DateTimeOffset MarkedAsSoldAt { get; set; }
                public Guid           MarkedAsSoldBy { get; set; }
            }
        }
    }
}
