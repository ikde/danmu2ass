using System.Collections.Generic;
using System.IO;
using NUnit.Framework;

namespace Kaedei.Danmu2Ass.Test
{
	[TestFixture]
	public class TestMain : Program
	{
		[Test]
		public void Test()
		{
			var paras = new List<string>
				{
					"-help",
					"-width",
					"1024",
					"-height",
					"768",
					"-bottom",
					"5",
					"-line",
					"10",
					"-shift",
					"1.23",
					"-s",
					"Sample.json",
					"Sample.json"
				};
			Main(paras.ToArray());
			Assert.IsTrue(File.Exists("Sample.ass"));
		}

		[Test]
		public void Test1()
		{
			var paras = new List<string>
				{
					"-help",
					"-width",
					"-height",
					"-bottom",
					"-line",
					"-shift",
					"1.23",
					"-s",
					"Sample.json",
					"Sample.json"
				};
			Main(paras.ToArray());
			Assert.IsTrue(File.Exists("Sample.ass"));
		}
	}
}