using System;

namespace ECom.Expansion.UnitTests.Domain
{

    public class TestEntity
    {
        public int Id { get; set; }

        public int? ExternalId { get; set; }

        public string Name { get; set; }

        public DateTime CreatedOn { get; set; }

        public DateTime? ModifiedOn { get; set; }

        public TimeSpan Duration { get; set; }

        public decimal Price { get; set; }

    }

}
