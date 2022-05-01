using Entities.Models.Expansion;
using System;

namespace ECom.Expansion.IntegrationTests
{
    public class Order : VersionedEntity<Guid>
    {
        public string Number { get; set; }
    }
}