using System;

namespace PublicClassifiedAdScreen.ClassifiedAds
{
    public static class Events
    {
        public static class V1
        {
            public class ClassifiedAdActivated
            {
                public Guid Id { get; set; }
                public string Title { get; set; }
                public double Price { get; set; }
                public Guid ActivatedBy { get; set; }
                public DateTimeOffset ActivatedAt { get; set; }
            }

            public class ClassifiedAdDeactivated
            {
                public Guid Id { get; set; }
                public DateTimeOffset DeactivatedAt { get; set; }
                public Guid DeactivatedBy { get; set; }
            }
        }
    }
}
