using System.Collections.Generic;
using System.Linq;
using Rating.Framework;

namespace Rating.Domain
{
    public class UserRating : Aggregate
    {
        private readonly List<DealRate> _dealRates;

        public UserRating(UserId userId)
        {
            Id = userId;
            _dealRates = new List<DealRate>();
        }

        public UserRating() { }

        public void AddDealRate(UserId userId, DealId dealId, Rate rate)
        {
            if (_dealRates.Any(x => x.DealId == dealId))
                throw new Exceptions.DuplicateDealRate("The deal has already been rated");

            Apply(new Events.V1.DealRateAddedToUserRating
            {
                UserId = userId,
                DealId = dealId,
                Rate = rate,
                TotalRate = _dealRates.Sum(x => x.Rate.Value)
            });
        }

        protected override void When(object @event)
        {
            switch (@event)
            {
                case Events.V1.DealRateAddedToUserRating e:
                    Id = e.UserId;
                    _dealRates.Add(new DealRate
                    {
                        DealId = e.DealId,
                        Rate = new Rate {Value = e.Rate}
                    });
                    break;
            }
        }
    }
}
