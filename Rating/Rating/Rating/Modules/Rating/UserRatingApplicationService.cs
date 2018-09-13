using System.Threading.Tasks;
using Rating.Domain;
using Rating.Framework;

namespace Rating.Modules.Rating
{
    public class UserRatingApplicationService
    {
        private readonly IAggregateStore _store;

        public UserRatingApplicationService(IAggregateStore store)
        {
            _store = store;
        }

        public async Task Handle(Contracts.UserRatingContracts.V1.AddDealRate command)
        {
            var rating = await _store.Load<UserRating>(command.UserId.ToString());
            rating.AddDealRate(
                new UserId(command.UserId),
                new DealId(command.DealId),
                new Rate(command.Rate));
            await _store.Save(rating);
        }
    }
}
