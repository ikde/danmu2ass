using System;

namespace Kaedei.Danmu2Ass.Parser
{
	public static class ParserFactory
	{
		public static IParser CreateParser(string name)
		{
			switch(name.ToUpper())
			{
				case "ACFUN":
					return new AcfunParser();
				case "BILIBILI":
					return new BilibiliParser();
			}
			throw new Exception("Parser name not exist.");
		}
	}
}