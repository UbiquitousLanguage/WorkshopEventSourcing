namespace Marketplace.Domain.ClassifiedAds
{
    using System;
    using System.Linq;
    using Marketplace.Domain.Messages;
    using Marketplace.Framework;

    public class ClassifiedAd : Aggregate
    {
        private Guid            CategoryId;
        private Email           ContactEmail;
        private PhoneNumber     ContactPhoneNumber;
        private DateTimeOffset  CreatedAt;
        private Guid            CreatedBy;
        private Description     Description;
        private DateTimeOffset? MarkedAsSoldAt;
        private Guid?           MarkedAsSoldBy;
        private Guid            OwnerId;
        private Picture[]       Pictures = new Picture[0];
        private DateTimeOffset? PublishedAt;
        private Guid?           PublishedBy;
        private Title           Title;

        protected override void When(object e)
        {
            switch (e)
            {
                case Events.V1.ClassifiedAdCreated x:
                    Id                 = x.Id.ToString();
                    Title              = x.Title;
                    Description        = x.Description;
                    ContactEmail       = x.ContactEmail;
                    ContactPhoneNumber = x.ContactPhoneNumber;
                    CategoryId         = x.CategoryId;
                    Pictures           = x.Pictures.Select(p => new Picture(p.Url, p.Description)).ToArray();
                    OwnerId            = x.OwnerId;
                    CreatedAt          = x.CreatedAt;
                    CreatedBy          = x.CreatedBy;
                    break;

                case Events.V1.ClassifiedAdPublished x:
                    PublishedAt = x.PublishedAt;
                    PublishedBy = x.PublishedBy;
                    break;

                case Events.V1.ClassifiedAdMarkedAsSold x:
                    MarkedAsSoldAt = x.MarkedAsSoldAt;
                    MarkedAsSoldBy = x.MarkedAsSoldBy;
                    break;
            }
        }

        public void Create(Guid id, Title title, Description description, Email contactEmail, PhoneNumber contactPhoneNumber)
            => Apply(new Events.V1.ClassifiedAdCreated {
                Id                 = id,
                Title              = title,
                Description        = description,
                ContactEmail       = contactEmail,
                ContactPhoneNumber = contactPhoneNumber
            });
    }
}
