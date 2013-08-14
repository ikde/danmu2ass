using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;

namespace Kaedei.Danmu2Ass.Parser
{
	class BilibiliParser : IParser
	{
		public List<CItem> Format(string input)
		{
			var doc = new XmlDocument();
			doc.LoadXml(input);

			var children = doc.SelectSingleNode("i").ChildNodes;
			return (from XmlNode node in children
			        where node.Name.Equals("d", StringComparison.CurrentCultureIgnoreCase)
			        let attributes = (node as XmlElement).GetAttribute("p").Split(',')
			        select new CItem
				        {
					        Time = float.Parse(attributes[0]),
					        Mode = int.Parse(attributes[1]),
					        Size = int.Parse(attributes[2]),
					        Color = int.Parse(attributes[3]),
					        Timestamp = int.Parse(attributes[4]),
					        Pool = 0,
					        UID = 0,
					        Message = (node as XmlElement).InnerText
				        }).ToList();
		}
	}
}