using System;
// ReSharper disable CheckNamespace

namespace Rating.Contracts
{
    public static class UserRatingContracts
    {
        public static class V1
        {
            public class AddDealRate
            {
                public Guid UserId { get; set; }
                public Guid DealId { get; set; }
                public int Rate { get; set; }

                public override string ToString() => $"Adding the rate {Rate} for the deal {DealId} to user rating {UserId}";
            }
        }
    }
}
