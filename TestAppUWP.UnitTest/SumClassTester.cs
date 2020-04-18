using Microsoft.VisualStudio.TestTools.UnitTesting;
using TestAppUWP.Data;

namespace TestAppUWP.AppShell
{
    [TestClass]
    public class SumClassTester
    {
        [TestMethod]
        public void SumTest()
        {
            var sumClass = new SumClass
            {
                Add1 = 6,
                Add2 = 3
            };
            Assert.AreEqual(9, sumClass.Result);
        }
    }
}
