using System.Collections.Generic;

namespace Kaedei.Danmu2Ass.Parser
{
	public interface IParser
	{
		List<CItem> Format(string input);
	}
}