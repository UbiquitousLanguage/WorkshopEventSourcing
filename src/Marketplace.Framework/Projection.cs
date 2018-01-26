using System;
using System.Threading.Tasks;

namespace Marketplace.Framework
{
    public abstract class Projection
    {
        private readonly Type _type;

        protected Projection()
        {
            _type = GetType();
        }

        public abstract Task Handle(object e);

        public override string ToString()
        {
            return _type.Name;
        }

        public static implicit operator string(Projection self)
        {
            return self.ToString();
        }
    }
}
