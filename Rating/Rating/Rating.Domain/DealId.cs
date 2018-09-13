using System;
using Rating.Framework;

namespace Rating.Domain
{
    public class DealId : Value<DealId>
    {
        private readonly Guid _value;

        public DealId(Guid value)
        {
            _value = value;
        }

        public static implicit operator Guid(DealId self) => self._value;

        public static implicit operator DealId(Guid value) => new DealId(value);
    }
}
