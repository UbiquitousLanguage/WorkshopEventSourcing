using System;
using System.Threading.Tasks;

namespace Marketplace.Framework
{
    public abstract class Projection
    {
        readonly Type _type;

        protected Projection() => _type = GetType();

        public abstract Task Handle(object e);

        public abstract bool CanHandle(object e);

        public override string ToString() => _type.Name;

        public static implicit operator string(Projection self) => self.ToString();
    }
}
