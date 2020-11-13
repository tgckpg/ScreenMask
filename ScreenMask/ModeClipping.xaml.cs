using ScreenMask.Config;
using System;
using System.Collections.Generic;
using System.Linq;
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

			if ( EditMode.Visibility == Visibility.Visible )
			{
				ClippingMode.EditModeSetup.CreateSelectionRect( EditMode, RectG.Rect, R => RectG.Rect = R );
			}
		}

		protected override void SaveSettings()
		{
			AppConfig.Current.Masks = Masks.Children
				.CastOnly<RectangleGeometry>().Skip( 1 )
				.Select( x => new MaskDef() { Rect = x.Rect } );
;
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

				KeyDown -= ModeClipping_KeyDown;
			}
			else
			{
				EditMode.Visibility = Visibility.Visible;
				Masks.Children.CastOnly<RectangleGeometry>().Skip( 1 )
					.Do( x => ClippingMode.EditModeSetup.CreateSelectionRect( EditMode, x.Rect, R => x.Rect = R ) );

				KeyDown += ModeClipping_KeyDown;
			}
		}

		private void ModeClipping_KeyDown( object sender, KeyEventArgs e )
		{
			IInputElement Elem = FocusManager.GetFocusedElement( EditMode );
			Elem.RaiseEvent( e );
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
