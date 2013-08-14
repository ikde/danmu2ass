using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Kaedei.Danmu2Ass.Parser;

namespace Kaedei.Danmu2Ass
{
	public class Program
	{
		protected static Converter m_converter;

		protected static void Main(string[] args)
		{
			Console.WriteLine("XML弹幕->ASS字幕转换工具 v" + Assembly.GetExecutingAssembly().GetName().Version);
			Console.WriteLine("(c) Kaedei 2013-2014");
			Console.WriteLine("http://blog.sina.com.cn/kaedei");
			Console.WriteLine();
			var argumentList = args.ToList();
			if (argumentList.Any())
			{
				Console.WriteLine("当前命令行:");
				argumentList.ForEach(Console.WriteLine);
				Console.WriteLine();

				//提取参数
				argumentList = ExtractParameter(argumentList);

				//初始化转换器
				m_converter = new Converter();
				m_converter.Init();

				//转换
				Parallel.ForEach(argumentList, f =>
					{
						try
						{
							ConvertToAss(f);
							Console.WriteLine("完成: " + f);
						}
						catch (Exception ex)
						{
							Console.WriteLine(ex.Message);
						}
					});
			}
			else
			{
				//注册到SendTo
				RegisterSendTo();
				Console.WriteLine("使用方法任选其一:");
				Console.WriteLine("  第一种-> 右键单击需要转换的xml/json弹幕文件，选择【发送到-转换为ASS字幕文件】");
				Console.WriteLine("  第二种-> 将需要转换的xml/json弹幕文件拖放到此程序的图标上");
				Console.WriteLine("转换后的ASS文件将被放到原弹幕文件的旁边");
			}

			Console.WriteLine();
			if (!Config.Silence)
			{
				Console.WriteLine("按任意键退出...");
				Console.ReadKey();
			}
		}

		protected static List<string> ExtractParameter(List<string> argumentList)
		{
			var fileList = new List<string>();
			for (int i = 0; i < argumentList.Count; i++)
			{
				try
				{
					var para = argumentList[i];
					switch (para.ToUpper())
					{
						case "-HELP":
						case "/HELP":
						case "/?":
							Console.WriteLine("可用参数列表:");
							Console.WriteLine(
								"Danmu2Ass.exe [-help|/?] [-width XXX] [-height XXX] [-line XXX] [-bottom XXX]\r\n [-shift XXX.XXX] [-S] File1 [File2,File3...]");
							Console.WriteLine();
							Console.WriteLine("-help或/?\t显示此提示");
							Console.WriteLine("-width XXX\t设置视频宽度为XXX，默认为1920");
							Console.WriteLine("-height XXX\t设置视频高度为XXX，默认为1080");
							Console.WriteLine("-bottom X\t设置为底部保留X分之一的区域，默认为6");
							Console.WriteLine("-shift XXX.XXX\t设置弹幕晚出现X秒，默认为0");
							Console.WriteLine("-S\t静默模式，程序完成时自动退出");
							Console.WriteLine("File\t文件完整路径，多个文件用空格隔开，包含空格的路径使用双引号括起来");
							break;
						case "-WIDTH":
						case "/WIDTH":
							Config.Width = int.Parse(argumentList[i + 1]);
							i++;
							break;
						case "-HEIGHT":
						case "/HEIGHT":
							Config.Height = int.Parse(argumentList[i + 1]);
							i++;
							break;
						case "-LINE":
						case "/LINE":
							Config.Line = int.Parse(argumentList[i + 1]);
							i++;
							break;
						case "-BOTTOM":
						case "/BOTTOM":
							Config.Bottom = int.Parse(argumentList[i + 1]);
							i++;
							break;
						case "-SHIFT":
						case "/SHIFT":
							Config.Shift = float.Parse(argumentList[i + 1]);
							i++;
							break;
						case "-S":
						case "/S":
							Config.Silence = true;
							break;
						default:
							fileList.Add(para);
							break;
					}
				}
				catch (Exception)
				{
					Console.WriteLine("解析命令行参数时出现错误，使用[Danmu2Ass.exe -help]查看提示信息");
				}
			}

			return fileList;
		}


		protected static void ConvertToAss(string file)
		{
			if (string.IsNullOrWhiteSpace(file))
			{
				throw new ArgumentException("文件名为空");
			}
			if (!File.Exists(file))
			{
				throw new FileNotFoundException("文件不存在: " + file);
			}
			var ext = Path.GetExtension(file);
			if (string.IsNullOrWhiteSpace(ext) ||
				(ext.ToUpper() != ".XML" && ext.ToUpper() != ".JSON"))
			{
				throw new InvalidDataException("文件扩展名不是.xml或.json: " + file);
			}

			try
			{
				var content = File.ReadAllText(file);

				IParser parser;
				List<CItem> cItems = null;
				switch (ext.ToUpper())
				{
					case ".XML":
						parser = ParserFactory.CreateParser("bilibili");
						cItems = parser.Format(content);
						break;
					case ".JSON":
						parser = ParserFactory.CreateParser("acfun");
						cItems = parser.Format(content);
						break;
				}
				var output = m_converter.GetCommentAss(cItems, Config.Width, Config.Height, Config.Line, Config.Bottom, Config.Shift);
				WriteOutput(Path.ChangeExtension(file, ".ass"), output);
			}
			catch
			{
				throw new Exception("处理文件时出错: " + file);
			}
		}

		private static void WriteOutput(string newfile, string output)
		{
			File.WriteAllText(newfile, output, Encoding.UTF8);
		}

		protected static string RegisterSendTo()
		{
			try
			{
				var sendToFolderLink =
					Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
								 "Microsoft" + Path.DirectorySeparatorChar + "Windows" + Path.DirectorySeparatorChar + "SendTo" +
								 Path.DirectorySeparatorChar + "转换为ASS字幕文件.lnk");
				if (File.Exists(sendToFolderLink)) File.Delete(sendToFolderLink);

				var sc = new Shortcut();
				sc.Path = Assembly.GetExecutingAssembly().Location;
				sc.WorkingDirectory = Path.GetDirectoryName(sc.Path);
				sc.Description = "XML弹幕->ASS字幕转换工具";

				sc.Save(sendToFolderLink);
				return sendToFolderLink;
			}
			catch
			{
				return null;
			}
		}
	}
}