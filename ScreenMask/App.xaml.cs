using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace ScreenMask
{
	public partial class App : Application
	{
		static readonly Mutex mutex = new Mutex( true, "{A15D7E21-CB77-4210-BD9B-F30E4E8B533A}" );
		public App()
		{
			if ( mutex.WaitOne( TimeSpan.Zero, true ) )
			{
				mutex.ReleaseMutex();
			}
			else
			{
				Win32Calls.PostMessage( ( IntPtr ) Win32Calls.HWND_BROADCAST, Win32Calls.MSG_SHOW_MYSELF, IntPtr.Zero, IntPtr.Zero );
				Current.MainWindow.Close();
			}
		}
	}
}
