using System;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using Marketplace.Contracts;
using Marketplace.Domain.ClassifiedAds;

namespace Marketplace.Tests
{
    public class Rename_classified_ad : Specification<ClassifiedAd, ClassifiedAds.V1.RenameAd>
    {
        public readonly Fixture AutoFixture = new Fixture();

        public override Func<ClassifiedAds.V1.RenameAd, Task> GetHandler(SpecificationAggregateStore store)
            => cmd => new ClassifiedAdsApplicationService(store).Handle(cmd);

        public override object[] Given() 
            => AutoFixture.CreateMany<Events.V1.ClassifiedAdCreated>(1)
                          .ToArray();

        public override ClassifiedAds.V1.RenameAd When()
            => AutoFixture.Build<ClassifiedAds.V1.RenameAd>()
                          .With(cmd => cmd.Id, ((Events.V1.ClassifiedAdCreated) History[0]).Id)
                          .Create();

        [Then]
        public void Classified_ad_is_renamed()
        {
            RaisedEvents.ShouldBeEquivalentTo(new [] { new Events.V1.ClassifiedAdRenamed {
                Id        = Command.Id,
                Title     = Command.Title,
                RenamedBy = Command.RenamedBy,
                RenamedAt = Command.RenamedAt
            }});
        }
    }
}