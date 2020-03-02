using DiiagramrAPI.Application;
using Xunit;

namespace DiiagramrAPIUnitTests
{
    public class ShellUnitTests : UnitTest
    {
        [Fact]
        public void Test1()
        {
            var shell = CreateUnitTestInstance<Shell>();
        }
    }
}