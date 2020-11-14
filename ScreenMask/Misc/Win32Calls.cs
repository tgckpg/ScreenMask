using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;
using System.Windows.Interop;

namespace ScreenMask
{
	class Win32Calls
	{
		/**
		 * See: https://stackoverflow.com/a/522874/1510539
		 */
		public const int HWND_BROADCAST = 0xffff;
		public static readonly int MSG_SHOW_MYSELF = RegisterWindowMessage( "MSG_SHOW_MYSELF" );
		[DllImport( "user32" )]
		public static extern bool PostMessage( IntPtr hwnd, int msg, IntPtr wparam, IntPtr lparam );
		[DllImport( "user32" )]
		public static extern int RegisterWindowMessage( string message );

		/**
		 * See: https://stackoverflow.com/a/3261489/1510539
		 */
		[DllImport( "user32.dll", SetLastError = true )]
		static extern int GetWindowLong( IntPtr hWnd, int nIndex );
		[DllImport( "user32.dll" )]
		static extern int SetWindowLong( IntPtr hWnd, int nIndex, int dwNewLong );
		private const int GWL_EX_STYLE = -20;
		private const int WS_EX_APPWINDOW = 0x00040000, WS_EX_TOOLWINDOW = 0x00000080;
		public static void HideFromAltTab( Window w )
		{
			var helper = new WindowInteropHelper( w ).Handle;
			SetWindowLong( helper, GWL_EX_STYLE, ( GetWindowLong( helper, GWL_EX_STYLE ) | WS_EX_TOOLWINDOW ) & ~WS_EX_APPWINDOW );
		}

		/**
		 * See: https://stackoverflow.com/questions/9668872/how-to-get-windows-position
		 */
		[DllImport( "user32.dll", CharSet = CharSet.Auto )]
		public static extern IntPtr FindWindow( string strClassName, string strWindowName );

		[DllImport( "user32.dll" )]
		public static extern bool GetWindowRect( IntPtr hwnd, ref U32Rect rectangle );
	}

	public static class Win32CallsExt
	{
		public static Rect GetWindowRect( this Process P )
		{
			U32Rect R = new U32Rect();
			Win32Calls.GetWindowRect( P.MainWindowHandle, ref R );

			return new Rect( new System.Windows.Point( R.Left, R.Top ), new System.Windows.Point( R.Right, R.Bottom ) );
		}
	}

	public struct U32Rect
	{
		public int Left { get; set; }
		public int Top { get; set; }
		public int Right { get; set; }
		public int Bottom { get; set; }
	}
}
