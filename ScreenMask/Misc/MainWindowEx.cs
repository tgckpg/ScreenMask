using ScreenMask.Config;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Interop;

namespace ScreenMask
{
	public abstract class MainWindowEx : Window
	{
		protected abstract void CreateMask( MaskDef Def );

		protected virtual void SaveSettings()
		{
			AppConfig.Current.ProcessProfiles = ProfileData.ProcessProfiles;
		}

		protected void StoreProfileOffset( string ProfileId, Rect CurrentBounds, Point DpiScale )
		{
			if ( string.IsNullOrEmpty( ProfileId ) || !ProfileId.Contains( "\n" ) )
				return;

			string[] s = ProfileId.Split( "\n", 2 );

			var WinProfile = ProfileData.ProcessProfiles
				.FirstOrDefault( x => x.BinId == s[ 0 ] )?
				.WindowProfiles.Where( x => x.Title == s[ 1 ] )
				.FirstOrDefault();

			if ( WinProfile == null )
				return;

			CurrentBounds.X *= DpiScale.X;
			CurrentBounds.Y *= DpiScale.Y;
			CurrentBounds.Width *= DpiScale.X;
			CurrentBounds.Height *= DpiScale.Y;
			WinProfile.Offsets = new System.Numerics.Vector4(
				( float ) ( CurrentBounds.X - WinProfile.Bounds.X )
				, ( float ) ( CurrentBounds.Y - WinProfile.Bounds.Y )
				, ( float ) ( CurrentBounds.Height - WinProfile.Bounds.Height )
				, ( float ) ( CurrentBounds.Width - WinProfile.Bounds.Width )
			);
		}

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

		protected void TogglePreventSleep( object sender, RoutedEventArgs e )
			=> SetPreventSleep( AppConfig.Current.PreventSleep = !AppConfig.Current.PreventSleep );

		protected void SetPreventSleep( bool PreventSleep )
		{
			if ( PreventSleep )
			{
				Win32Calls.SetThreadExecutionState( EXECUTION_STATE.ES_CONTINUOUS | EXECUTION_STATE.ES_DISPLAY_REQUIRED );
			}
			else
			{
				Win32Calls.SetThreadExecutionState( EXECUTION_STATE.ES_CONTINUOUS );
			}
		}

		protected void Exit_Click( object sender, RoutedEventArgs e )
		{
			SaveSettings();
			Application.Current.Shutdown();
		}
	}
}
