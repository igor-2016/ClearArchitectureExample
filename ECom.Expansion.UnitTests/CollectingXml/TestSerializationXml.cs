using Collecting.Interfaces.Clients.Responses;
using DomainServices.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace ECom.Expansion.UnitTests.CollectingXml
{
    public class TestSerializationXml
    {

        [Fact]
        public void Can_DeserializeOrderDataFromCollectingFozzyClientXml()
        {
            var path = $"_Data{Path.DirectorySeparatorChar}GetOrderXml.xml";
            var data =  ReadXml(path);

            Assert.NotNull(data);
            var order = data.FromXml<FozzyOrderData>();
            Assert.NotNull(order);
            Assert.NotNull(order.order);
            Assert.NotNull(order.order[0].orderId);
            Assert.True(order.order.Length == 1);
            Assert.True(order.orderLines.Length == 2);
            Assert.True(order.orderLines[0].lagerId > 0);
        }

        public static string ReadXml(string filePath)
        {
            return File.ReadAllText(filePath);
        }

    }
}
