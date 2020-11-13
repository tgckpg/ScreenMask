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
			List<MaskDef> MaskDefs = new List<MaskDef>();
			foreach ( Window W in Application.Current.Windows )
			{
				if ( W is Mask M )
				{
					MaskDefs.Add( M.AsDef() );
				}
			}

			AppConfig.Current.Masks = MaskDefs;
			AppConfig.Current.GadgetPos = new Point( Top, Left );
			AppConfig.Current.Save();
		}

		private void Window_Loaded( object sender, RoutedEventArgs e )
		{
			Point P = AppConfig.Current.GadgetPos;
			Top = P.X;
			Left = P.Y;

			foreach ( MaskDef Def in AppConfig.Current.Masks )
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

		private void ClippingMode_Click( object sender, RoutedEventArgs e )
		{
			Close();

			foreach ( Window W in Application.Current.Windows )
				if ( W is Mask M )
					M.Close();

			ModeClipping CMask = new ModeClipping();
			CMask.Show();

			Application.Current.MainWindow = CMask;
		}
	}
}
