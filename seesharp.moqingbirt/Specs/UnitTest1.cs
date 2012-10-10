using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;

namespace Specs
{
    using seesharp.moqingbirt;
    using seesharp.moqingbirt.TestBench;

    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            var mock = new Mock<IMyInjectedService>();
            mock.Setup(o => o.IsAvailable).Returns(true);
            Assert.IsTrue(mock.Object.IsAvailable);

            mock.Setup(o => o.IsAvailable).Returns(false);

            mock.Setup(o => o.ReturnAnInteger()).Returns(1);
            mock.Setup(o => o.SetAnInteger(1.0));

            mock.Verify(o => o.SetAnInteger(1.0), 1);
            Assert.IsFalse(mock.Object.IsAvailable);

            //mock.VerifyGet(o => o.IsAvailable, 2);
            //mock.VerifySet(o => o.IsAvailable, 0);
        }
    }
}
