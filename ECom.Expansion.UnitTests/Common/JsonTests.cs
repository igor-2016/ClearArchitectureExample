using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace ECom.Expansion.UnitTests.Common
{
    public class JsonTests
    {

        [Fact]
        public void Can_SerializeNullProperty()
        {
            var entityToSerialize = new EntityWithNullProperty()
            {
                Id = 1,
                Name = "Namefewefew",
                Comment = null,
            };

            var entityAsString = JsonConvert.SerializeObject(entityToSerialize);

            var entity = JsonConvert.DeserializeObject<EntityLessProperties>(entityAsString);

            Assert.NotNull(entity);
            Assert.Equal(entityToSerialize.Id, entity.Id);
            //Assert.Equal(null, entity.Comment);

        }

        public class EntityLessProperties
        {
            [JsonProperty("id")]
            public int Id { get; set; }

            //[JsonProperty("name")]
            //public string Name { get; set; }

            [JsonProperty("value")]
            public int? Value { get; set; }

            //[JsonProperty("comment", NullValueHandling = NullValueHandling.Ignore)]
            //public string Comment { get; set; }
        }

        public class EntityWithNullProperty
        {
            [JsonProperty("id")]
            public int Id { get; set; }

            [JsonProperty("name")]
            public string Name { get; set; }

            [JsonProperty("value")]
            public int? Value { get; set; }

            [JsonProperty("comment", NullValueHandling = NullValueHandling.Ignore)]
            public string Comment { get; set; }
        }
    }
}
