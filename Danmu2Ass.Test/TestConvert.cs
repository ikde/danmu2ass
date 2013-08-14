using System;
using System.IO;
using System.Linq;
using Kaedei.Danmu2Ass.Parser;
using NUnit.Framework;

namespace Kaedei.Danmu2Ass.Test
{
	[TestFixture]
	public class TestConvert : Program
	{
		[Test]
		public void TestParserFactory()
		{
			var acfun = ParserFactory.CreateParser("acfun");
			Assert.IsNotNull(acfun);
			var bilibili = ParserFactory.CreateParser("bilibili");
			Assert.IsNotNull(bilibili);
			try
			{
				ParserFactory.CreateParser("asdfasdf");
				Assert.IsTrue(false);
			}
			catch
			{
				Assert.IsTrue(true);
			}
		}

		[Test]
		public void TestAcfunParser()
		{
			var parser = ParserFactory.CreateParser("acfun");
			Assert.IsNotNull(parser);

			var cItems = parser.Format(File.ReadAllText("Sample.json"));
			Assert.IsNotEmpty(cItems);
			Assert.IsTrue(cItems.Any());
			var firstItem = cItems.First();
			Assert.IsTrue(Math.Abs(firstItem.Time - 976.489F) < 0.001);
			Assert.IsTrue(firstItem.Message == "哈哈哈哈");
		}

		/// <summary>
		/// 没找到b站数据
		/// </summary>
		[Test]
		public void TestBilibiliParser()
		{
			var parser = ParserFactory.CreateParser("acfun");
			Assert.IsNotNull(parser);

			var cItems = parser.Format(File.ReadAllText("Sample.json"));
			Assert.IsNotEmpty(cItems);
			Assert.IsTrue(cItems.Any());
			var firstItem = cItems.First();
			Assert.IsTrue(Math.Abs(firstItem.Time - 976.489F) < 0.001);
			Assert.IsTrue(firstItem.Message == "哈哈哈哈");
		}

		[Test]
		[ExpectedException(ExpectedException = typeof(ArgumentException))]
		public void TestConvertToAss_NullString()
		{
			ConvertToAss(" ");
		}

		[Test]
		[ExpectedException(ExpectedException = typeof(FileNotFoundException))]
		public void TestConvertToAss_NotExistFile()
		{
			ConvertToAss(@"C:\ASDFASDF.abc");
		}

		[Test]
		[ExpectedException(ExpectedException = typeof(InvalidDataException))]
		public void TestConvertToAss_FileType()
		{
			ConvertToAss(@"C:\Windows\notepad.exe");
		}

		[Test]
		[ExpectedException(ExpectedException = typeof(Exception))]
		public void TestConvertToAss_NotValidFile()
		{
			File.Copy(@"C:\Windows\notepad.exe", "notepad.xml", true);
			ConvertToAss(@"notepad.xml");
		}

		[Test]
		public void TestConvertToAss()
		{
			TestConverter_Init();
			ConvertToAss(@"Sample.json");
		}

		[Test]
		public void TestConverter_Init()
		{
			m_converter = new Converter();
			m_converter.Init();
		}

	}
}