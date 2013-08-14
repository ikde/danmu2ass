using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Xml;
using IronPython.Hosting;
using Microsoft.Scripting.Hosting;

namespace Kaedei.Danmu2Ass
{
	public class Converter
	{
		ScriptEngine m_engine;
		CompiledCode m_code;
		volatile bool m_isInitialized;

		/// <summary>
		/// 初始化转换引擎
		/// </summary>
		public void Init()
		{
			m_engine = Python.CreateEngine();
			Assembly assembly = GetType().Assembly;
			var stream = assembly.GetManifestResourceStream("Kaedei.Danmu2Ass.PythonFile.Niconvert.py");
			string sourcepy = new StreamReader(stream).ReadToEnd();
			var source = m_engine.CreateScriptSourceFromString(sourcepy);
			m_code = source.Compile();
			m_isInitialized = true;
		}

		/// <summary>
		/// 获取ASS字符串
		/// </summary>
		/// <param name="commentItems">所有弹幕</param>
		/// <param name="resWidth">视频宽度</param>
		/// <param name="resHeight">视频高度</param>
		/// <param name="lines">同屏行数</param>
		/// <param name="bottomArea">底部字幕区域占高度的几分之一，设置为0时底部区域高度为0</param>
		/// <param name="shift">弹幕时间轴偏移量</param>
		/// <returns></returns>
		public string GetCommentAss(IEnumerable<CItem> commentItems, int resWidth, int resHeight, int lines = 14, int bottomArea = 6, float shift = 0.0F)
		{
			if (!m_isInitialized)
				throw new Exception("Engine not initialized.");

			var xmlstring = Combine(new List<CItem>(commentItems));

			//calculate fontsize/lines
			int bottommargin;
			if (bottomArea <= 0)
				bottommargin = 0;
			else
				bottommargin = resHeight / bottomArea;

			var fontsize = (resHeight - bottommargin) / lines;

			var scope = m_engine.CreateScope();
			m_code.Execute(scope);
			var convert = scope.GetVariable<Func<object, object, object, object, object, object, object, object>>("convert");
			var ass = convert(xmlstring, resWidth.ToString() + ":" + resHeight.ToString(),
				"Microsoft YaHei", fontsize, lines, bottommargin, shift).ToString();
			return ass;
		}

		static string Combine(IEnumerable<CItem> addItems)
		{
			const string xml = @"<?xml version=""1.0"" encoding=""UTF-8""?><i><chatserver>chat.bilibili.tv</chatserver><chatid>10000</chatid><source>k-v</source></i>";
			var doc = new XmlDocument();
			doc.LoadXml(xml);

			var node = doc.SelectSingleNode("i");


			foreach (var t in addItems)
			{
				var newnode = doc.CreateElement("d");
				var item = t;
				newnode.InnerText = item.Message;

				var sb = new StringBuilder();

				sb.Append(item.Time + ",");
				sb.Append(item.Mode + ",");
				sb.Append(item.Size + ",");
				sb.Append(item.Color + ",");
				sb.Append(item.Timestamp + ",");
				sb.Append(item.Pool + ",");
				sb.Append(item.UID + ",");
				sb.Append(item.CID);
				newnode.SetAttribute("p", sb.ToString());

				node.AppendChild(newnode);
			}

			var ms = new MemoryStream();
			var writer = XmlWriter.Create(ms);
			doc.Save(writer);
			ms.Position = 0;
			return Encoding.UTF8.GetString(ms.ToArray());
		}
	}
}