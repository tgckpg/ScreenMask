using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
	public partial class ColorPicker : Window
	{
		public Color SelectedColor { get; private set; }

		public ColorPicker()
		{
			InitializeComponent();
		}

		public new void Show() => throw new Exception( "Please use ShowDialog" );

		private void WindowLoaded( object sender, RoutedEventArgs args ) => Win32Calls.HideFromAltTab( this );

		private void OK_Click( object sender, RoutedEventArgs e )
		{
			DialogResult = true;
			Close();
		}

		private void Cancel_Click( object sender, RoutedEventArgs e )
		{
			DialogResult = false;
			Close();
		}

		private void TextBox_TextChanged( object sender, TextChangedEventArgs e )
		{
			TextBox ColorTxBox = ( TextBox ) sender;

			string HexColor = ColorTxBox.Text;
			Regex Pat = new Regex( "[^\\dABCDEFabcdef]+", RegexOptions.Compiled );
			HexColor = Pat.Replace( HexColor, "" );

			ColorTxBox.Text = HexColor;
			ColorTxBox.CaretIndex = HexColor.Length;

			if ( OK_Button != null )
				OK_Button.IsEnabled = false;

			if ( !string.IsNullOrEmpty( HexColor ) )
			{
				(byte A, byte R, byte G, byte B) = (0xFF, 0xFF, 0xFF, 0xFF);
				switch ( HexColor.Length )
				{
					case 1:
						R = G = B = Convert.ToByte( new string( new char[] { HexColor[ 0 ], HexColor[ 0 ] } ), 16 );
						break;
					case 3:
						R = Convert.ToByte( new string( new char[] { HexColor[ 0 ], HexColor[ 0 ] } ), 16 );
						G = Convert.ToByte( new string( new char[] { HexColor[ 1 ], HexColor[ 1 ] } ), 16 );
						B = Convert.ToByte( new string( new char[] { HexColor[ 2 ], HexColor[ 2 ] } ), 16 );
						break;
					case 6:
						R = Convert.ToByte( new string( new char[] { HexColor[ 0 ], HexColor[ 1 ] } ), 16 );
						G = Convert.ToByte( new string( new char[] { HexColor[ 2 ], HexColor[ 3 ] } ), 16 );
						B = Convert.ToByte( new string( new char[] { HexColor[ 4 ], HexColor[ 5 ] } ), 16 );
						break;
					case 8:
						A = Convert.ToByte( new string( new char[] { HexColor[ 0 ], HexColor[ 1 ] } ), 16 );
						R = Convert.ToByte( new string( new char[] { HexColor[ 2 ], HexColor[ 3 ] } ), 16 );
						G = Convert.ToByte( new string( new char[] { HexColor[ 4 ], HexColor[ 5 ] } ), 16 );
						B = Convert.ToByte( new string( new char[] { HexColor[ 5 ], HexColor[ 6 ] } ), 16 );
						break;
					default:
						return;
				}

				if ( OK_Button != null )
					OK_Button.IsEnabled = true;

				SelectedColor = Color.FromArgb( A, R, G, B );
				ColorPallet.Background = new SolidColorBrush( SelectedColor );
			}
		}

		private void Border_MouseDown( object sender, MouseButtonEventArgs e )
		{
			if ( e.ChangedButton == MouseButton.Left )
				this.DragMove();
		}
	}
}
