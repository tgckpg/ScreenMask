using ScreenMask.Config;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;

namespace ScreenMask
{
	public partial class MainWindow : Window
	{
		public MainWindow()
		{
			InitializeComponent();
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

		private void Window_Loaded( object sender, RoutedEventArgs e )
		{
			AppConfig.Read();

			Point P = AppConfig.GetMainWindowPos();
			Top = P.X;
			Left = P.Y;

			foreach ( MaskDef Def in AppConfig.GetMasks() )
				CreateMask( Def );
		}

		private void Grid_MouseDown( object sender, MouseButtonEventArgs e )
		{
			if ( e.ChangedButton == MouseButton.Left )
			{
				this.DragMove();
			}
		}

		private void Grid_MouseUp( object sender, MouseButtonEventArgs e ) => SaveSettings();

		private void Grid_MouseRightButtonUp( object sender, MouseButtonEventArgs e )
		{
			ContextMenu MainMenu = Resources[ "MainMenu" ] as ContextMenu;
			MainMenu.IsOpen = true;
		}

		private void SaveSettings()
		{
			AppConfig.SetMainWindowPos( Top, Left );
			SaveMasks();
			AppConfig.Save();
		}

		private void SaveMasks()
		{
			List<MaskDef> MaskDefs = new List<MaskDef>();
			foreach ( Window W in Application.Current.Windows )
			{
				if ( W is Mask M )
				{
					MaskDefs.Add( M.AsDef() );
				}
			}

			AppConfig.SetMasks( MaskDefs );
		}

		private void NewMask_Click( object sender, RoutedEventArgs e )
		{
			FullScreenDrawSpace DrawSpace = new FullScreenDrawSpace();
			DrawSpace.Show();
			DrawSpace.Closed += ( sender, e ) =>
			{
				CreateMask( new MaskDef() { Rect = DrawSpace.DefinedArea } );
				SaveSettings();
			};
		}

		private void BringAlltoTop_Click( object sender, RoutedEventArgs e )
		{
			foreach ( Window W in Application.Current.Windows )
				if ( W is Mask M )
					M.Activate();
		}

		private void RemoveAll_Click( object sender, RoutedEventArgs e )
		{
			foreach ( Window W in Application.Current.Windows )
				if ( W is Mask M )
					M.Close();
		}

		private void Exit_Click( object sender, RoutedEventArgs e )
		{
			SaveSettings();
			Application.Current.Shutdown();
		}

		private void CreateMask( MaskDef Def )
		{
			Mask SMask = new Mask( Def );
			SMask.Show();
		}
	}
}
