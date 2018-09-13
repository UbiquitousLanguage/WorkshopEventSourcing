using System;

namespace Rating.Domain
{
    public static class Events
    {
        public static class V1
        {
            public class DealRateAddedToUserRating
            {
                public Guid UserId { get; set; }
                public Guid DealId { get; set; }
                public int Rate { get; set; }
                public int TotalRate { get; set; }
            }
        }
    }
}
