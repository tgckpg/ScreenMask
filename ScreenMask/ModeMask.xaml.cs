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
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;

namespace ScreenMask
{
	public partial class ModeMask : MainWindowEx
	{
		public ModeMask()
		{
			InitializeComponent();
			AppConfig.Current.Mode = "Mask";
		}

		protected override void CreateMask( MaskDef Def )
		{
			Mask SMask = new Mask( Def );
			SMask.Show();
		}

		protected override void SaveSettings()
		{
			base.SaveSettings();
			AppConfig.Current.Masks = Application.Current.Windows.CastOnly<Mask>().Select( x => x.AsDef() );
			AppConfig.Current.GadgetPos = new Point( Top, Left );
			AppConfig.Current.Save();
		}

		private void Window_Loaded( object sender, RoutedEventArgs e )
		{
			Point P = AppConfig.Current.GadgetPos;
			Top = P.X;
			Left = P.Y;

			AppConfig.Current.Masks.Do( x => CreateMask( x ) );

			VisualStateManager.GoToElementState( OuterRect, "Idle", false );
		}

		private void Grid_MouseDown( object sender, MouseButtonEventArgs e )
		{
			if ( e.ChangedButton == MouseButton.Left )
			{
				this.DragMove();
			}
		}

		private void Grid_MouseRightButtonUp( object sender, MouseButtonEventArgs e )
		{
			ContextMenu MainMenu = Resources[ "MainMenu" ] as ContextMenu;
			MainMenu.IsOpen = true;
		}

		private void BringAlltoTop_Click( object sender, RoutedEventArgs e )
			=> Application.Current.Windows.CastOnly<Mask>().Do( x => x.Activate() );

		private void RemoveAll_Click( object sender, RoutedEventArgs e )
			=> Application.Current.Windows.CastOnly<Mask>().Do( x => x.Close() );

		private void ClippingMode_Click( object sender, RoutedEventArgs e )
		{
			Application.Current.Windows.CastOnly<Mask>().Do( x => x.Close() );

			ModeClipping CMask = new ModeClipping();
			Application.Current.MainWindow = CMask;

			Close();

			CMask.Show();
		}
	}
}
