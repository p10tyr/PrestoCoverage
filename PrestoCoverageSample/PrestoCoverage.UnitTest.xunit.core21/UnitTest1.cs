using Xunit;

namespace PrestoCoverage.UnitTest.xunit
{
    public class UnitTest1
    {
        //To make all this jazz work you can run the powershell "runtest.ps1" just make sure you have .NET Core SDK 2.1+ and Coverlet global tool installed
        //dotnet tool install --global coverlet.console 

        [Fact]
        public void Test1()
        {
            var sample = new Sample.SampleClass();

            sample.Counter = 10;

            var result1 = sample.MissingBranchtested(0);

            var countResult = sample.Count(1, 1);

            Assert.Equal(1, countResult);
        }

        [Fact]
        public void Test2()
        {
            var sample = new Sample.SampleClass();

            var result1 = sample.FlatBranching(true, true, false);
        }
    }
}
