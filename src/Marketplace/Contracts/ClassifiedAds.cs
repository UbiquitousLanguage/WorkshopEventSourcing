namespace Marketplace.Contracts
{
    using System;

    public static class ClassifiedAds
    {
        public static class V1
        {
            public class CreateClassifiedAd
            {
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
                
                public override string ToString() => $"Creating Classified Ad {Id} '{Title}' in {CategoryId}...";
            }
        }
    }
}
