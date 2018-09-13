using System;

namespace Rating.Domain
{
    public class DealRate : IEquatable<DealRate>
    {
        internal DealId DealId { get; set; }
        internal Rate Rate { get; set; }

        public DealRate(DealId dealId, Rate rate)
        {
            DealId = dealId;
            Rate = rate;
        }

        internal DealRate() { }

        public bool Equals(DealRate other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(DealId, other.DealId);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((DealRate) obj);
        }

        public override int GetHashCode()
        {
            return DealId != null ? DealId.GetHashCode() : 0;
        }
    }
}
