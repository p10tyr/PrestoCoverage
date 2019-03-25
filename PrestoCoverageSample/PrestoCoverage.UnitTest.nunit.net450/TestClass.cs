using NUnit.Framework;

namespace PrestoCoverage.UnitTest.nunit.net450
{
    [TestFixture]
    public class TestClass
    {
        [Test]
        public void TestMethod()
        {
            var sample = new Sample.net450.SampleClass();

            sample.Counter = 10;

            int result = sample.DoSomething(1, 1);

            var result1 = sample.MissingBranchtested(0);

            var countResult = sample.Count(1, 1);

            //Assert.Pass("Your first passing test");
            Assert.AreEqual(1, result);
        }
    }
}
