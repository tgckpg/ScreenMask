using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace ScreenMask.ClippingMode
{
	class EditModeSetup
	{
		public static void CreateSelectionRect( Canvas Stage, Rect B, Action<Rect> ChangeCallback )
		{
			SolidColorBrush HoverShadeBrush = new SolidColorBrush() { Color = Color.FromArgb( 1, 0, 0, 0 ) };

			TextBlock Dim = new TextBlock
			{
				Foreground = new SolidColorBrush() { Color = Colors.White },
				HorizontalAlignment = HorizontalAlignment.Center,
				VerticalAlignment = VerticalAlignment.Center
			};

			Grid Corners = new Grid();
			Corners.Width = B.Width + 10;
			Corners.Height = B.Height + 10;

			Border CornerTL = new Border();
			CornerTL.Width = CornerTL.Height = 15;
			CornerTL.VerticalAlignment = VerticalAlignment.Top;
			CornerTL.HorizontalAlignment = HorizontalAlignment.Left;
			CornerTL.BorderThickness = new Thickness( 5, 5, 0, 0 );
			CornerTL.BorderBrush = new SolidColorBrush() { Color = Color.FromArgb( 120, 255, 255, 255 ) };

			Border CornerTR = new Border();
			CornerTR.Width = CornerTR.Height = 15;
			CornerTR.VerticalAlignment = VerticalAlignment.Top;
			CornerTR.HorizontalAlignment = HorizontalAlignment.Right;
			CornerTR.BorderThickness = new Thickness( 0, 5, 5, 0 );
			CornerTR.BorderBrush = new SolidColorBrush() { Color = Color.FromArgb( 120, 255, 255, 255 ) };

			Border CornerBR = new Border();
			CornerBR.Width = CornerBR.Height = 15;
			CornerBR.VerticalAlignment = VerticalAlignment.Bottom;
			CornerBR.HorizontalAlignment = HorizontalAlignment.Right;
			CornerBR.BorderThickness = new Thickness( 0, 0, 5, 5 );
			CornerBR.BorderBrush = new SolidColorBrush() { Color = Color.FromArgb( 120, 255, 255, 255 ) };

			Border CornerBL = new Border();
			CornerBL.Width = CornerBL.Height = 15;
			CornerBL.VerticalAlignment = VerticalAlignment.Bottom;
			CornerBL.HorizontalAlignment = HorizontalAlignment.Left;
			CornerBL.BorderThickness = new Thickness( 5, 0, 0, 5 );
			CornerBL.BorderBrush = new SolidColorBrush() { Color = Color.FromArgb( 120, 255, 255, 255 ) };

			Border HoverShade = new Border
			{
				Background = HoverShadeBrush,
				Width = B.Width,
				Height = B.Height
			};

			HoverShade.Child = Dim;
			Corners.Children.Add( CornerTL );
			Corners.Children.Add( CornerTR );
			Corners.Children.Add( CornerBR );
			Corners.Children.Add( CornerBL );

			Stage.Children.Add( Corners );
			Stage.Children.Add( HoverShade );

			void UpdateDisplay()
			{
				double Y1 = B.Top - AppVars.W_OFFSET_X;
				double X1 = B.Left - AppVars.W_OFFSET_Y;

				Canvas.SetTop( HoverShade, Y1 );
				Canvas.SetLeft( HoverShade, X1 );
				Canvas.SetTop( Corners, Y1 - 5 );
				Canvas.SetLeft( Corners, X1 - 5 );

				Dim.Text = $"X: {B.X.AsDecimalPlaces( 2 )} Y: {B.Y.AsDecimalPlaces( 2 )} W: {B.Width.AsDecimalPlaces( 2 )} H: {B.Height.AsDecimalPlaces( 2 )}";
			}

			UpdateDisplay();

			HoverShade.MouseEnter += ( s, e ) =>
				HoverShadeBrush.Color = Color.FromArgb( 120, 0, 0, 0 );

			HoverShade.MouseLeave += ( s, e ) =>
				HoverShadeBrush.Color = Color.FromArgb( 1, 0, 0, 0 );

			BindMouseDragHandlers( Stage, HoverShade, d =>
			{
				B.X += d.X; B.Y += d.Y;
				ChangeCallback( B );
				UpdateDisplay();
			} );

			void CornerTailOps()
			{
				ChangeCallback( B );

				Corners.Width = B.Width + 10;
				Corners.Height = B.Height + 10;
				HoverShade.Width = B.Width;
				HoverShade.Height = B.Height;
				UpdateDisplay();
			}

			BindMouseDragHandlers( Stage, CornerTL, d =>
			{
				B.X += d.X; B.Y += d.Y;
				B.Width -= d.X; B.Height -= d.Y;
				CornerTailOps();
			} );

			BindMouseDragHandlers( Stage, CornerTR, d =>
			{
				B.Y += d.Y;
				B.Width += d.X; B.Height -= d.Y;
				CornerTailOps();
			} );

			BindMouseDragHandlers( Stage, CornerBL, d =>
			{
				B.X += d.X;
				B.Width -= d.X; B.Height += d.Y;
				CornerTailOps();
			} );

			BindMouseDragHandlers( Stage, CornerBR, d =>
			{
				B.Width += d.X; B.Height += d.Y;
				CornerTailOps();
			} );
		}

		private static void BindMouseDragHandlers( Canvas Stage, FrameworkElement Elem, Action<System.Windows.Vector> HandleStack )
		{
			Point DragStart;
			void _OnMove( object sender, MouseEventArgs e )
			{
				if ( e.LeftButton == MouseButtonState.Pressed )
				{
					Point DragEnd = e.GetPosition( Stage );
					var d = DragEnd - DragStart;
					DragStart = DragEnd;

					HandleStack( d );
				}
				else
				{
					Elem.ReleaseMouseCapture();
					Elem.MouseMove -= _OnMove;
				}
			}

			Elem.MouseDown += ( s, e ) =>
			{
				Elem.CaptureMouse();
				DragStart = e.GetPosition( Stage );
				Elem.MouseMove += _OnMove;
			};

			Elem.MouseUp += ( s, e ) =>
			{
				Elem.ReleaseMouseCapture();
				Elem.MouseMove -= _OnMove;
			};
		}

	}
}
