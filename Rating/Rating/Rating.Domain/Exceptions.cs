using System;

namespace Rating.Domain
{
    public class Exceptions
    {
        public class DuplicateDealRate : Exception
        {
            public DuplicateDealRate(string message) : base(message) { }
        }
    }
}
