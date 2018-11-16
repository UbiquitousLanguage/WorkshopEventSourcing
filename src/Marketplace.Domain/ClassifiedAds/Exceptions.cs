using Marketplace.Framework;

namespace Marketplace.Domain.ClassifiedAds
{
    public class ClassifiedAdNotFound : EntityNotFoundException { }

    public class ClassifiedAdAlreadyRegistered : DomainException
    {
        public ClassifiedAdAlreadyRegistered()
            : base("Classified ad already registered.") { }
    }

    public class PriceNotAllowed : DomainException
    {
        public PriceNotAllowed()
            : base("Price not allowed.") { }
    }

    public class TitleTooLong : DomainException
    {
        public TitleTooLong()
            : base("Title too long.") { }
    }

    public class TitleRequired : DomainException
    {
        public TitleRequired()
            : base("Title required.") { }
    }

    public class InvalidClassifiedAdId : DomainException
    {
        public InvalidClassifiedAdId()
            : base("Id cannot be default.") { }
    }

    public class InvalidOwnerId : DomainException
    {
        public InvalidOwnerId()
            : base("Id cannot be default.") { }
    }

    public class ProfanityFound : DomainException
    {
        public ProfanityFound()
            : base("Profanity found.") { }
    }

    public class ClassifiedAdUnpublished : DomainException
    {
        public ClassifiedAdUnpublished()
            : base("Classified ad unpublished.") { }
    }

    public class ClassifiedAdRemoved : DomainException
    {
        public ClassifiedAdRemoved()
            : base("Classified ad removed.") { }
    }
}
