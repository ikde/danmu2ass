using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Kaedei.Danmu2Ass.Parser
{
	class AcfunParser : IParser
	{
		public List<CItem> Format(string input)
		{
			var items = new List<CItem>();
			var regex = new Regex(@"{""c"":( |)""(?<c>.+?)"",( |)""m"":( |)""(?<m>.+?)""}", RegexOptions.Singleline);
			var matchCollection = regex.Matches(input);
			foreach (Match m in matchCollection)
			{
				var item = new CItem();

				string c = m.Groups["c"].Value;
				var cs = c.Split(',');
				item.Time = float.Parse(cs[0]);
				item.Color = int.Parse(cs[1]);
				item.Mode = int.Parse(cs[2]);
				item.Size = int.Parse(cs[3]);
				item.UID = 0;
				item.Timestamp = float.Parse(cs[5]);

				//转换unicode
				item.Message = Convert(m.Groups["m"].Value);
				//修复最后一个特殊弹幕出错
				if (item.Message.StartsWith("{") && !item.Message.EndsWith("}"))
					item.Message += "\"}";

				items.Add(item);
			}
			return items;
		}

		static string Convert(string input)
		{
			var chars = input.ToCharArray();
			var sb = new StringBuilder(chars.Length);
			for (var i = 0; i < chars.Length; i++)
			{
				if (i > chars.Length - 6)
				{
					sb.Append(chars[i]);
					continue;
				}
				if (chars[i] == '\\' && chars[i + 1] == 'u')
				{
					string temp = string.Concat(chars[i + 2], chars[i + 3], chars[i + 4], chars[i + 5]);
					char newchar = (char)int.Parse(temp, System.Globalization.NumberStyles.HexNumber);
					sb.Append(newchar);
					i += 5;
				}
				else
				{
					sb.Append(chars[i]);
				}
			}
			return sb.ToString();
		}
	}
}