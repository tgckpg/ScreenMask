using GR.Effects;
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
using System.Windows.Media.Animation;
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
				ClippingMode.EditModeSetup.CreateSelectionRect( EditMode, RectG, R => RectG.Rect = R );
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
			AppConfig.Current.Masks.Do( x => CreateMask( x ) );
			if ( ( Topmost = AppConfig.Current.AlwaysOnTop ) )
			{
				( ( ContextMenu ) Resources[ "MainMenu" ] ).Items
					.CastOnly<MenuItem>().First( x => x.Name == "AlwaysOnTop" )
					.IsChecked = true;
			}
		}

		private void ToggleAlwaysTop( object sender, RoutedEventArgs e )
			=> Topmost = ( sender as MenuItem ).IsChecked;

		private void Grid_MouseRightButtonUp( object sender, MouseButtonEventArgs e )
		{
			ContextMenu MainMenu = ( ContextMenu ) Resources[ "MainMenu" ];

			if ( FocusManager.GetFocusedElement( EditMode ) is FrameworkElement Elem && Elem.Tag is RectangleGeometry RectG )
			{
				MainMenu.Items.CastOnly<MenuItem>().First( x => x.Name == "DeleteThisMask" ).Visibility = Visibility.Visible;
			}
			else
			{
				MainMenu.Items.CastOnly<MenuItem>().First( x => x.Name == "DeleteThisMask" ).Visibility = Visibility.Collapsed;
			}

			MainMenu.IsOpen = true;
		}

		private void MaskMode_Click( object sender, RoutedEventArgs e )
		{
			Close();
			ModeMask MMode = new ModeMask();
			MMode.Show();
		}

		private void DeleteThisMask_Click( object sender, RoutedEventArgs e )
		{
			if( FocusManager.GetFocusedElement( EditMode ) is FrameworkElement Elem && Elem.Tag is RectangleGeometry RectG )
			{
				Masks.Children.Remove( RectG );
				EditMode.Children.CastOnly<FrameworkElement>()
					.Where( x => RectG.Equals( x.Tag ) )
					.ToArray()
					.Do( x => EditMode.Children.Remove( x ) );
				SaveSettings();
			}
		}

		private void EditMasks_Click( object sender, RoutedEventArgs e )
		{
			if ( EditMode.Visibility == Visibility.Visible )
			{
				EditMode.Visibility = Visibility.Collapsed;
				EditMode.Children.Clear();

				KeyDown -= ModeClipping_KeyDown;
				MouseDown -= ModeClipping_MouseDown;
				MouseUp -= ModeClipping_MouseUp;

				( ( ContextMenu ) Resources[ "MainMenu" ] ).Items.CastOnly<Control>()
					.Where( x => "ShowOnEditMode".Equals( x.Tag ) )
					.Do( x => x.Visibility = Visibility.Collapsed );
			}
			else
			{
				EditMode.Visibility = Visibility.Visible;
				Masks.Children.CastOnly<RectangleGeometry>().Skip( 1 )
					.Do( x => ClippingMode.EditModeSetup.CreateSelectionRect( EditMode, x, R => x.Rect = R ) );

				KeyDown += ModeClipping_KeyDown;
				MouseDown += ModeClipping_MouseDown;
				MouseUp += ModeClipping_MouseUp;

				( ( ContextMenu ) Resources[ "MainMenu" ] ).Items.CastOnly<Control>()
					.Where( x => "ShowOnEditMode".Equals( x.Tag ) )
					.Do( x => x.Visibility = Visibility.Visible );
			}
		}

		private void ModeClipping_MouseUp( object sender, MouseButtonEventArgs e ) => SaveSettings();
		private void ModeClipping_MouseDown( object sender, MouseButtonEventArgs e )
		{
			if( FocusManager.GetFocusedElement( EditMode ) is FrameworkElement Elem && Elem.DataContext is Storyboard BlurStory )
			{
				SimpleStory.DoubleAnimation( BlurStory, Elem, "Opacity", Elem.Opacity, ClippingMode.EditModeSetup.OPACITY_IDLE );
				BlurStory.Begin();
			}
			FocusManager.SetFocusedElement( EditMode, null );
		}

		private void ModeClipping_KeyDown( object sender, KeyEventArgs e )
			=> FocusManager.GetFocusedElement( EditMode )?.RaiseEvent( e );

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
