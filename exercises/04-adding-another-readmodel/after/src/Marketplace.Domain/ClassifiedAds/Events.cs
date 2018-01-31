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

                public override string ToString() =>
                    $"Classified Ad {Id} was renamed to '{(Title?.Length > 25 ? $"{Title?.Substring(0, 22)}..." : Title)}'";
            }

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

            public class PictureAddedToClassifiedAd
            {
                public Guid Id { get; set; }
                public Picture Picture { get; set; }
                public DateTimeOffset PictureAddedAt { get; set; }
                public Guid PictureAddedBy { get; set; }
            }

            public class PictureRemovedFromClassifiedAd
            {
                public Guid Id { get; set; }
                public Picture Picture { get; set; }
                public DateTimeOffset PictureRemovedAt { get; set; }
                public Guid PictureRemovedBy { get; set; }
            }

            public class ClassifiedAdPublished
            {
                public Guid Id { get; set; }
                public string Title { get; set; }
                public string Text { get; set; }
                public DateTimeOffset PublishedAt { get; set; }
                public Guid PublishedBy { get; set; }
            }

            public class ClassifiedAdRejected
            {
                public Guid Id { get; set; }
                public string Reason { get; set; }
                public DateTimeOffset RejectedAt { get; set; }
                public Guid RejectedBy { get; set; }
            }

            public class ClassifiedAdActivated
            {
                public Guid Id { get; set; }
                public string Title { get; set; }
                public double Price { get; set; }
                public Guid ActivatedBy { get; set; }
                public DateTimeOffset ActivatedAt { get; set; }
            }

            public class ClassifiedAdReportedByUser
            {
                public Guid Id { get; set; }
                public string Reason { get; set; }
                public DateTimeOffset ReportedAt { get; set; }
                public Guid ReportedBy { get; set; }
            }

            public class ClassifiedAdDeactivated
            {
                public Guid Id { get; set; }
                public DateTimeOffset DeactivatedAt { get; set; }
                public Guid DeactivatedBy { get; set; }
            }

            public class ClassifiedAdMarkedAsSold
            {
                public Guid Id { get; set; }
                public DateTimeOffset MarkedAsSoldAt { get; set; }
                public Guid MarkedAsSoldBy { get; set; }
            }

            public class ClassifiedAdRemoved
            {
                public Guid Id { get; set; }
                public DateTimeOffset RemovedAt { get; set; }
                public Guid RemovedBy { get; set; }
            }
        }
    }
}
