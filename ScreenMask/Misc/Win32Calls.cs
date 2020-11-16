using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
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


		/**
		 * See: https://stackoverflow.com/a/22440420/1510539
		 */
		[DllImport( "user32.dll" )]
		public static extern uint GetWindowThreadProcessId( IntPtr hWnd, out uint lpdwProcessId );

		[DllImport( "user32.Dll" )]
		[return: MarshalAs( UnmanagedType.Bool )]
		public static extern bool EnumChildWindows( IntPtr parentHandle, Win32Callback callback, IntPtr lParam );

		public delegate bool Win32Callback( IntPtr hwnd, IntPtr lParam );

		/**
		 * See: https://stackoverflow.com/a/17890354/1510539
		 */
		[DllImport( "user32.dll", CharSet = CharSet.Auto, SetLastError = true )]
		public static extern IntPtr SendMessageTimeout( IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam, uint fuFlags, uint uTimeout, out IntPtr lpdwResult );
	}

	public static class Win32CallsExt
	{
		public static Icon GetIcon( this Process P )
		{
			try
			{
				return Icon.ExtractAssociatedIcon( P.MainModule.FileName );
			}
			catch
			{
				return null;
			}
		}

		public static IEnumerable<(Process, IEnumerable<(string, Rect)>)> GetWindowRects( this IEnumerable<Process> Ps )
		{
			List<IntPtr> rootWindows = GetChildWindows( IntPtr.Zero );

			Dictionary<uint, List<(string, Rect)>> PIDRects = new Dictionary<uint, List<(string, Rect)>>();

			foreach ( IntPtr hWnd in rootWindows )
			{
				_ = Win32Calls.GetWindowThreadProcessId( hWnd, out uint lpdwProcessId );
				if ( !PIDRects.TryGetValue( lpdwProcessId, out List<(string, Rect)> Rects ) )
					PIDRects[ lpdwProcessId ] = Rects = new List<(string, Rect)>();

				Rects.Add( (GetWindowTitle( hWnd ), GetWindowRect( hWnd )) );
			}

			return Ps.Select( x =>
			{
				if ( PIDRects.TryGetValue( ( uint ) x.Id, out List<(string, Rect)> Rects ) )
					return (x, Rects.AsEnumerable());
				return (x, Array.Empty<(string, Rect)>());
			} );
		}

		private static Rect GetWindowRect(IntPtr Ptr )
		{
			U32Rect R = new U32Rect();
			_ = Win32Calls.GetWindowRect( Ptr, ref R );

			return new Rect( new System.Windows.Point( R.Left, R.Top ), new System.Windows.Point( R.Right, R.Bottom ) );
		}

		private static string GetWindowTitle( IntPtr hWnd )
		{
			const uint SMTO_ABORTIFHUNG = 0x0002;
			const uint WM_GETTEXT = 0xD;
			const int MAX_STRING_SIZE = 32768;
			IntPtr memoryHandle = Marshal.AllocCoTaskMem( MAX_STRING_SIZE );
			Marshal.Copy( new char[] { '\0' }, 0, memoryHandle, 1 );
			_ = Win32Calls.SendMessageTimeout( hWnd, WM_GETTEXT, ( IntPtr ) MAX_STRING_SIZE, memoryHandle, SMTO_ABORTIFHUNG, 1000, out _ );
			string Title = Marshal.PtrToStringAuto( memoryHandle );
			Marshal.FreeCoTaskMem( memoryHandle );
			return Title;
		}

		public static List<IntPtr> GetChildWindows( IntPtr parent )
		{
			List<IntPtr> result = new List<IntPtr>();
			GCHandle listHandle = GCHandle.Alloc( result );
			try
			{
				Win32Calls.Win32Callback childProc = new Win32Calls.Win32Callback( EnumWindow );
				Win32Calls.EnumChildWindows( parent, childProc, GCHandle.ToIntPtr( listHandle ) );
			}
			finally
			{
				if ( listHandle.IsAllocated )
					listHandle.Free();
			}
			return result;
		}

		private static bool EnumWindow( IntPtr handle, IntPtr pointer )
		{
			GCHandle gch = GCHandle.FromIntPtr( pointer );
			List<IntPtr> list = gch.Target as List<IntPtr>;
			if ( list == null )
			{
				throw new InvalidCastException( "GCHandle Target could not be cast as List<IntPtr>" );
			}
			list.Add( handle );
			return true;
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
