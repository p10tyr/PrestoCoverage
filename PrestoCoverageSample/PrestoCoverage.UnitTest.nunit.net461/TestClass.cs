using NUnit.Framework;

namespace PrestoCoverage.UnitTest.nunit.net461
{
    [TestFixture]
    public class TestClass
    {
        [Test]
        public void TestMethod()
        {
            var sample = new Sample.Standard20.SampleClass();

            sample.Counter = 10;

            var result1 = sample.MissingBranchtested(0);

            var countResult = sample.Count(1, 1);

            Assert.Pass("Your first passing test");
        }
    }
}
