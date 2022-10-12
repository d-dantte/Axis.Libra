using Axis.Libra.Utils;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Axis.Libra.Tests.Unit.Utils
{
    [TestClass]
    public class PropertySerializerTests
    {
        [TestMethod]
        public void Serialize_WithNullObject_ShouldThrowException()
        {
            Assert.ThrowsException<ArgumentNullException>(() => PropertySerializer.Serialize(null));
        }
    }
}
