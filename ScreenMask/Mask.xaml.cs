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

namespace ScreenMask
{
	public partial class Mask : Window
	{
		enum AdjustmentType { None, Move, Resize }

		private AdjustmentType Adjustment = AdjustmentType.None;
		private double Step = 0.1;

		public Mask( MaskDef Def )
		{
			InitializeComponent();

			Top = Def.Rect.Top;
			Left = Def.Rect.Left;
			Width = Def.Rect.Width;
			Height = Def.Rect.Height;
			Topmost = Def.AlwaysOnTop;

			Background = new SolidColorBrush( Color.FromArgb( Def.BgColor[ 0 ], Def.BgColor[ 1 ], Def.BgColor[ 2 ], Def.BgColor[ 3 ] ) );

			foreach ( MenuItem Item in ( ( ContextMenu ) Resources[ "MainMenu" ] ).Items )
			{
				if ( Item.Name == "AlwaysOnTop" )
				{
					Item.IsChecked = Topmost;
				}
			}
		}

		private void WindowLoaded( object sender, RoutedEventArgs args )
		{
			Win32Calls.HideFromAltTab( this );
			EditMask.DataContext = this;
		}

		private void Grid_MouseDown( object sender, MouseButtonEventArgs e )
		{
			if ( e.ChangedButton == MouseButton.Left )
				this.DragMove();
		}

		private void Grid_MouseRightButtonUp( object sender, MouseButtonEventArgs e )
		{
			ContextMenu MainMenu = Resources[ "MainMenu" ] as ContextMenu;
			MainMenu.IsOpen = true;
		}

		private void ToggleAlwaysTop( object sender, RoutedEventArgs e )
			=> Topmost = ( sender as MenuItem ).IsChecked;

		public MaskDef AsDef()
		{
			Color BgColor = ( ( SolidColorBrush ) Background ).Color;
			return new MaskDef
			{
				Rect = new Rect( Left, Top, Width, Height ),
				AlwaysOnTop = Topmost,
				BgColor = new byte[] { BgColor.A, BgColor.R, BgColor.G, BgColor.B }
			};
		}

		private void Move_Click( object sender, RoutedEventArgs e )
		{
			Adjustment = AdjustmentType.Move;
			EditMask.Visibility = Visibility.Visible;
		}

		private void Resize_Click( object sender, RoutedEventArgs e )
		{
			Adjustment = AdjustmentType.Resize;
			EditMask.Visibility = Visibility.Visible;
		}

		private void DeleteThisMask_Click( object sender, RoutedEventArgs e ) => Close();

		private void Window_KeyDown( object sender, KeyEventArgs e )
		{
			if ( Adjustment != AdjustmentType.None && ( e.Key == Key.Escape || e.Key == Key.Enter ) )
			{
				Adjustment = AdjustmentType.None;
				EditMask.Visibility = Visibility.Collapsed;
				return;
			}

			switch ( Adjustment )
			{
				case AdjustmentType.Move:
					Adj_Move( e.Key );
					break;
				case AdjustmentType.Resize:
					Adj_Resize( e.Key );
					break;
			}
		}

		private void Adj_Resize( Key key )
		{
			switch ( key )
			{
				case Key.Up:
				case Key.K:
					if ( 0 < Height )
						Height -= Step;
					break;
				case Key.Down:
				case Key.J:
					Height += Step;
					break;
				case Key.Left:
				case Key.H:
					if ( 0 < Width )
						Width -= Step;
					break;
				case Key.Right:
				case Key.L:
					Width += Step;
					break;
			}
		}

		private void Adj_Move( Key key )
		{
			switch ( key )
			{
				case Key.Up:
				case Key.K:
					Top -= Step;
					break;
				case Key.Down:
				case Key.J:
					Top += Step;
					break;
				case Key.Left:
				case Key.H:
					Left -= Step;
					break;
				case Key.Right:
				case Key.L:
					Left += Step;
					break;
			}
		}

		private void Color_Click( object sender, RoutedEventArgs e )
		{
			ColorPicker Picker = new ColorPicker { Owner = this };
			if ( Picker.ShowDialog() == true )
			{
				Background = new SolidColorBrush( Picker.SelectedColor );
			}
		}
	}
}
