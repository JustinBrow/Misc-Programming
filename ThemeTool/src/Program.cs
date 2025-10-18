using System;
using System.Globalization;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security.Permissions;

namespace ThemeApi
{
	public static class ThemeManagerHelpClass
	{
		[ComImport]
		[Guid("D23CC733-5522-406D-8DFB-B3CF5EF52A71")]
		[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
		public interface ITheme
		{
			[DispId(1610678272)]
			string DisplayName
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				[return: MarshalAs(UnmanagedType.BStr)]
				get;
			}

			[DispId(1610678273)]
			string VisualStyle
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				[return: MarshalAs(UnmanagedType.BStr)]
				get;
			}
		}

		[ComImport]
		[Guid("0646EBBE-C1B7-4045-8FD0-FFD65D3FC792")]
		[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
		public interface IThemeManager
		{
			[DispId(1610678272)]
			ITheme CurrentTheme
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				[return: MarshalAs(UnmanagedType.Interface)]
				get;
			}

			[MethodImpl(MethodImplOptions.InternalCall)]
			void ApplyTheme([In][MarshalAs(UnmanagedType.BStr)] string bstrThemePath);
		}

		[ComImport]
		[Guid("A2C56C2A-E63A-433E-9953-92E94F0122EA")]
		[CoClass(typeof(ThemeManagerClass))]
		public interface ThemeManager : IThemeManager
		{
		}

		[ComImport]
		[TypeLibType(TypeLibTypeFlags.FCanCreate)]
		[Guid("C04B329E-5823-4415-9C93-BA44688947B0")]
		[ClassInterface(ClassInterfaceType.None)]
		public class ThemeManagerClass : ThemeManager, IThemeManager
		{
			[DispId(1610678272)]
			public virtual extern ITheme CurrentTheme
			{
				[MethodImpl(MethodImplOptions.InternalCall)]
				[return: MarshalAs(UnmanagedType.Interface)]
				get;
			}

			[MethodImpl(MethodImplOptions.InternalCall)]
			public virtual extern void ApplyTheme([In][MarshalAs(UnmanagedType.BStr)] string bstrThemePath);

			// Causes compile errors so I commeted it out.
			//[MethodImpl(MethodImplOptions.InternalCall)]
			//public extern ThemeManagerClass();
		}

		private static class NativeMethods
		{
			[DllImport("UxTheme.dll")]
			[return: MarshalAs(UnmanagedType.Bool)]
			public static extern bool IsThemeActive();
		}

		private static IThemeManager themeManager = new ThemeManagerClass();

		[PermissionSet(SecurityAction.LinkDemand)]
		public static string GetCurrentThemeName()
		{
			return themeManager.CurrentTheme.DisplayName;
		}

		[PermissionSet(SecurityAction.LinkDemand)]
		public static void ChangeTheme(string themeFilePath)
		{
			themeManager.ApplyTheme(themeFilePath);
		}

		[PermissionSet(SecurityAction.LinkDemand)]
		public static string GetCurrentVisualStyleName()
		{
			return Path.GetFileName(themeManager.CurrentTheme.VisualStyle);
		}

		public static string GetThemeStatus()
		{
			if (!NativeMethods.IsThemeActive())
			{
				return "stopped";
			}
			return "running";
		}

		[STAThread]
		[PermissionSet(SecurityAction.LinkDemand)]
		public static void Main(string[] args)
		{
			if (args.Length < 1)
			{
				return;
			}
			string format = "";
			string strA = args[0].ToLower(CultureInfo.InvariantCulture);
			try
			{
				if (string.Compare(strA, "getcurrentthemename") == 0)
				{
					format = GetCurrentThemeName();
				}
				else if (string.Compare(strA, "changetheme") == 0)
				{
					if (args.Length < 2)
					{
						return;
					}
					ChangeTheme(args[1]);
				}
				else if (string.Compare(strA, "getcurrentvisualstylename") == 0)
				{
					format = GetCurrentVisualStyleName();
				}
				else
				{
					if (string.Compare(strA, "getthemestatus") != 0)
					{
						return;
					}
					format = GetThemeStatus();
				}
			}
			catch
			{
				format = "";
			}
		}
	}
}
