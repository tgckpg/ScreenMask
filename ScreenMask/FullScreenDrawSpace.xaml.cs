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
	public partial class FullScreenDrawSpace : Window
	{
		public Rect DefinedArea;
		bool DrawBegun = false;
		Point StartPoint;

		public FullScreenDrawSpace()
		{
			InitializeComponent();
		}

		public new void Show() => throw new Exception( "Please use ShowDialog" );

		private void WindowLoaded( object sender, RoutedEventArgs args ) => Win32Calls.HideFromAltTab( this );

		private void Window_MouseUp( object sender, MouseButtonEventArgs e )
		{
			DefinedArea.X = Canvas.GetLeft( DefiningArea ) + ( AppVars.W_OFFSET_X + 1 );
			DefinedArea.Y = Canvas.GetTop( DefiningArea ) + ( AppVars.W_OFFSET_Y + 1 );
			DefinedArea.Width = DefiningArea.Width;
			DefinedArea.Height = DefiningArea.Height;

			DialogResult = true;
			Close();
		}

		private void BeginDraw( object sender, MouseButtonEventArgs e )
		{
			StartPoint = Mouse.GetPosition( this );
			DrawBegun = true;
		}

		private void Grid_MouseMove( object sender, MouseEventArgs e )
		{
			Point P = Mouse.GetPosition( this );
			MousePosLabel.Text = $"{P.X}, {P.Y}";
			if( DrawBegun )
			{
				if ( StartPoint.X > P.X )
				{
					DefiningArea.Width = StartPoint.X - P.X;
					Canvas.SetLeft( DefiningArea, P.X );
				}
				else
				{
					DefiningArea.Width = P.X - StartPoint.X;
					Canvas.SetLeft( DefiningArea, StartPoint.X );
				}

				if ( StartPoint.Y > P.Y )
				{
					DefiningArea.Height = StartPoint.Y - P.Y;
					Canvas.SetTop( DefiningArea, P.Y );
				}
				else
				{
					DefiningArea.Height = P.Y - StartPoint.Y;
					Canvas.SetTop( DefiningArea, StartPoint.Y );
				}
			}
		}

		private void Window_KeyDown( object sender, KeyEventArgs e )
		{
			if( e.Key == Key.Escape)
			{
				DialogResult = false;
				Close();
			}
		}
	}
}
