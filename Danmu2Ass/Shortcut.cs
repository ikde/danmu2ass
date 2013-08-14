using System;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Interop;

namespace Kaedei.Danmu2Ass
{
	/*
		Shortcut sc = new Shortcut(); 
		sc.Path = "目标文件地址"; 
		sc.Arguments = "启动参数"; 
		sc.WorkingDirectory = "启动文件的文件夹"; 
		sc.Description = "描述"; 
		sc.Save("这个快捷方式保存在哪"); 
	*/

	public class Shortcut
	{
		private const int MAX_DESCRIPTION_LENGTH = 512;
		private const int MAX_PATH = 512;

		private readonly NativeClasses.IShellLinkW m_link;

		public Shortcut()
		{
			m_link = NativeClasses.CreateShellLink();
		}

		public Shortcut(string path)
			: this()
		{
			Marshal.ThrowExceptionForHR(m_link.SetPath(path));
		}

		public string Path
		{
			get
			{
				var fdata = new NativeClasses._WIN32_FIND_DATAW();
				var path = new StringBuilder(MAX_PATH, MAX_PATH);
				Marshal.ThrowExceptionForHR(m_link.GetPath(path, path.MaxCapacity, ref fdata, NativeClasses.SLGP_UNCPRIORITY));
				return path.ToString();
			}
			set { Marshal.ThrowExceptionForHR(m_link.SetPath(value)); }
		}

		public string Description
		{
			get
			{
				var desc = new StringBuilder(MAX_DESCRIPTION_LENGTH, MAX_DESCRIPTION_LENGTH);
				Marshal.ThrowExceptionForHR(m_link.GetDescription(desc, desc.MaxCapacity));
				return desc.ToString();
			}
			set { Marshal.ThrowExceptionForHR(m_link.SetDescription(value)); }
		}

		public string RelativePath
		{
			set { Marshal.ThrowExceptionForHR(m_link.SetRelativePath(value, 0)); }
		}

		public string WorkingDirectory
		{
			get
			{
				var dir = new StringBuilder(MAX_PATH, MAX_PATH);
				Marshal.ThrowExceptionForHR(m_link.GetWorkingDirectory(dir, dir.MaxCapacity));
				return dir.ToString();
			}
			set { Marshal.ThrowExceptionForHR(m_link.SetWorkingDirectory(value)); }
		}

		public string Arguments
		{
			get
			{
				var args = new StringBuilder(MAX_PATH, MAX_PATH);
				Marshal.ThrowExceptionForHR(m_link.GetArguments(args, args.MaxCapacity));
				return args.ToString();
			}
			set { Marshal.ThrowExceptionForHR(m_link.SetArguments(value)); }
		}

		public ushort HotKey
		{
			get
			{
				ushort key;
				Marshal.ThrowExceptionForHR(m_link.GetHotkey(out key));
				return key;
			}
			set { Marshal.ThrowExceptionForHR(m_link.SetHotkey(value)); }
		}

		private NativeClasses.IPersistFile AsPersist
		{
			get { return ((NativeClasses.IPersistFile) m_link); }
		}

		public void Resolve(IntPtr hwnd, uint flags)
		{
			Marshal.ThrowExceptionForHR(m_link.Resolve(hwnd, flags));
		}

		public void Resolve(IWin32Window window)
		{
			Resolve(window.Handle, 0);
		}

		public void Resolve()
		{
			Resolve(IntPtr.Zero, (uint) NativeClasses.SLR_MODE.SLR_NO_UI);
		}

		public void Save(string fileName)
		{
			int hres = AsPersist.Save(fileName, true);
			Marshal.ThrowExceptionForHR(hres);
		}

		public void Load(string fileName)
		{
			int hres = AsPersist.Load(fileName, (uint) NativeClasses.STGM_ACCESS.STGM_READ);
			Marshal.ThrowExceptionForHR(hres);
		}
	}
}