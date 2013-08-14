using System;
using System.IO;
using NUnit.Framework;

namespace Kaedei.Danmu2Ass.Test
{
	[TestFixture]
	public class TestShortcut : Program
	{
		[Test]
	 	public void RegisterSendTo()
		{
			var sendToFolderLink = Program.RegisterSendTo();
			Assert.IsTrue(File.Exists(sendToFolderLink));
		}
	}
}