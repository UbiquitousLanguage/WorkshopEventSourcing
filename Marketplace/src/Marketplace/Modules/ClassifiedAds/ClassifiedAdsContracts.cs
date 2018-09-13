using System;

// ReSharper disable CheckNamespace

namespace Marketplace.Contracts
{
    public static class ClassifiedAds
    {
        public static class V1
        {
            /// <summary>
            ///     Create a new ad command
            /// </summary>
            public class CreateAd
            {
                /// <summary>
                ///     New ad id
                /// </summary>
                public Guid Id { get; set; }
                
                /// <summary>
                ///     Ad owner id
                /// </summary>
                public Guid OwnerId { get; set; }

                public override string ToString() => $"Creating Classified Ad {Id}";
            }

            public class RenameAd
            {
                /// <summary>
                ///     New ad id
                /// </summary>
                public Guid Id { get; set; }

                /// <summary>
                ///     The new title
                /// </summary>
                public string Title { get; set; }

                /// <summary>
                ///     Id of the user who renamed the ad
                /// </summary>
                public Guid RenamedBy { get; set; }
                
                public override string ToString() 
                    => $"Renaming Classified Ad {Id} to '{(Title?.Length > 25 ? $"{Title.Substring(0, 22)}..." : Title )}'";
            }

            public class UpdateText
            {
                public Guid Id { get; set; }
                public string Text { get; set; }
                public Guid TextChangedBy { get; set; }
            }

            public class ChangePrice
            {
                public Guid Id { get; set; }
                public double Price { get; set; }
                public Guid PriceChangedBy { get; set; }
            }

            public class Publish
            {
                public Guid Id { get; set; }
                public Guid PublishedBy { get; set; }
            }

            public class Activate
            {
                public Guid Id { get; set; }
                public Guid ActivatedBy { get; set; }
            }

            public class Reject
            {
                public Guid Id { get; set; }
                public string Reason { get; set; }
                public Guid RejectedBy { get; set; }
            }

            public class Report
            {
                public Guid Id { get; set; }
                public string Reason { get; set; }
                public Guid ReportedBy { get; set; }
            }

            public class Deactivate
            {
                public Guid Id { get; set; }
                public Guid DeactivatedBy { get; set; }
            }

            public class MarkAsSold
            {
                public Guid Id { get; set; }
                public Guid MarkedBy { get; set; }
            }

            public class Remove
            {
                public Guid Id { get; set; }
                public Guid RemovedBy { get; set; }
            }
        }
    }
    
    public static class Shared
    {
        public static class V1
        {
            public class Picture
            {
                public string Url { get; set; }
                public string Description { get; set; }
            }
        }
    }
}
