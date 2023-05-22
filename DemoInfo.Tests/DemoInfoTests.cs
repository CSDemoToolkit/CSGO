
namespace DemoInfo.Tests
{
    [TestClass]
    public class ParsingTests
    {
        [TestMethod]
        public void Parse_BasicDemo()
        {
            string fileName = "basic_demo.dem";
            string path = Path.Combine(Environment.CurrentDirectory, @"Demos/", fileName);
        }
    }
}