using ScreenMask.Config;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace ScreenMask
{
	public partial class ModeClipping : MainWindowEx
	{
		public ModeClipping()
		{
			InitializeComponent();
			AppConfig.Current.Mode = "Clipping";
		}

		protected override void CreateMask( MaskDef Def )
		{
			RectangleGeometry RectG = new RectangleGeometry( Def.Rect );
			Masks.Children.Add( RectG );
		}

		protected override void SaveSettings()
		{
			List<MaskDef> MaskDefs = new List<MaskDef>();

			foreach ( RectangleGeometry RectG in Masks.Children )
			{
				if ( RectG == BackgroundRect )
					continue;

				MaskDefs.Add( new MaskDef() { Rect = RectG.Rect } );
			}

			AppConfig.Current.Masks = MaskDefs;
			AppConfig.Current.AlwaysOnTop = Topmost;
			AppConfig.Current.Save();
		}

		private void WindowLoaded( object sender, RoutedEventArgs args )
		{
			BackgroundRect.Rect = new Rect( Left, Top, Width, Height );

			foreach ( MaskDef Def in AppConfig.Current.Masks )
				CreateMask( Def );

			Topmost = AppConfig.Current.AlwaysOnTop;
		}

		private void ToggleAlwaysTop( object sender, RoutedEventArgs e )
			=> Topmost = ( sender as MenuItem ).IsChecked;

		private void Grid_MouseRightButtonUp( object sender, MouseButtonEventArgs e )
		{
			ContextMenu MainMenu = Resources[ "MainMenu" ] as ContextMenu;
			MainMenu.IsOpen = true;
		}

		private void MaskMode_Click( object sender, RoutedEventArgs e )
		{
			Close();
			ModeMask MMode = new ModeMask();
			MMode.Show();
		}

		private void EditMasks_Click( object sender, RoutedEventArgs e )
		{
			if ( EditMode.Visibility == Visibility.Visible )
			{
				EditMode.Visibility = Visibility.Collapsed;
				EditMode.Children.Clear();
			}
			else
			{
				EditMode.Visibility = Visibility.Visible;
				foreach ( RectangleGeometry RectG in Masks.Children )
				{
					if ( RectG == BackgroundRect )
						continue;

					ClippingMode.EditModeSetup.CreateSelectionRect( EditMode, RectG.Rect, R => RectG.Rect = R );
				}
			}
		}

		private void Color_Click( object sender, RoutedEventArgs e )
		{
			ColorPicker Picker = new ColorPicker() { Owner = this };
			if ( Picker.ShowDialog() == true )
			{
				BackgroundBrush.Brush = new SolidColorBrush( Picker.SelectedColor );
			}
		}
	}
}
