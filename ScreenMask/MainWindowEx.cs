using ScreenMask.Config;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Interop;

namespace ScreenMask
{
	public abstract class MainWindowEx : Window
	{
		protected abstract void SaveSettings();
		protected abstract void CreateMask( MaskDef Def );

		protected override void OnSourceInitialized( EventArgs e )
		{
			base.OnSourceInitialized( e );
			HwndSource source = HwndSource.FromHwnd( new WindowInteropHelper( this ).Handle );
			source.AddHook( new HwndSourceHook( WndProc ) );
		}

		private static IntPtr WndProc( IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled )
		{
			if( msg == Win32Calls.MSG_SHOW_MYSELF )
			{
				Application.Current.MainWindow.Activate();
			}

			return IntPtr.Zero;
		}

		protected void NewMask_Click( object sender, RoutedEventArgs e )
		{
			FullScreenDrawSpace DrawSpace = new FullScreenDrawSpace() { Owner = this };
			if ( DrawSpace.ShowDialog() == true )
			{
				CreateMask( new MaskDef() { Rect = DrawSpace.DefinedArea } );
				SaveSettings();
			}
		}

		protected void Exit_Click( object sender, RoutedEventArgs e )
		{
			SaveSettings();
			Application.Current.Shutdown();
		}
	}
}
