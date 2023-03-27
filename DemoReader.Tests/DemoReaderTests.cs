using System;
using System.IO;

namespace DemoReader.Tests
{
	[TestClass]
	public class ParsingTests
	{
		[TestMethod]
		public void Parse_BasicDemo()
		{
			string fileName = "basic_demo.dem";
			string path = Path.Combine(Environment.CurrentDirectory, @"Demos/", fileName);

			var demoReader = new DemoReader();
			demoReader.Analyze(path);
		}
	}
}