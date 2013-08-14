using System;
using System.Collections.Generic;
using NUnit.Framework;

namespace Kaedei.Danmu2Ass.Test
{
	[TestFixture]
	public class TestExtractParameter : Program
	{
		[Test]
		public void GenParameter()
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
					"C:\app.xml",
					"asdfadsf"
				};
			var list = ExtractParameter(paras);
			Assert.IsTrue(!list.Contains("-help"));
			Assert.IsTrue(!list.Contains("-width"));
			Assert.IsTrue(!list.Contains("1024"));
			Assert.IsTrue(!list.Contains("-height"));
			Assert.IsTrue(!list.Contains("768"));
			Assert.IsTrue(!list.Contains("-bottom"));
			Assert.IsTrue(!list.Contains("5"));
			Assert.IsTrue(!list.Contains("-line"));
			Assert.IsTrue(!list.Contains("10"));
			Assert.IsTrue(!list.Contains("-shift"));
			Assert.IsTrue(!list.Contains("1.23"));
			Assert.IsTrue(!list.Contains("-s"));
			Assert.IsTrue(list.Contains("C:\app.xml"));

			Assert.IsTrue(Config.Width == 1024);
			Assert.IsTrue(Config.Height == 768);
			Assert.IsTrue(Config.Bottom == 5);
			Assert.IsTrue(Config.Line == 10);
			Assert.IsTrue(Math.Abs(Config.Shift - 1.23F) < 0.01);
			Assert.IsTrue(Config.Silence);
		}
	}
}