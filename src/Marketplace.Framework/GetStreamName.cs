namespace Marketplace.Framework
{
    using System;

    public delegate string GetStreamName(Type aggregateType, string aggregateId);
}