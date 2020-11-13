using GR.Effects;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace ScreenMask.ClippingMode
{
	class EditModeSetup
	{
		public const double OPACITY_IDLE = 0.1;
		public const double OPACITY_IDLE_FOCUS = 0.5;
		public const double OPACITY_MOUSE_DOWN = 0.6;
		public const double OPACITY_MOUSE_OVER = 0.8;

		private static double Step = 0.1;

		public static void CreateSelectionRect( Canvas Stage, RectangleGeometry RectG, Action<Rect> ChangeCallback )
		{
			Rect B = RectG.Rect;

			SolidColorBrush HoverShadeBrush = new SolidColorBrush() { Color = Color.FromArgb( 10, 0, 0, 0 ) };

			TextBlock Dim = new TextBlock
			{
				Foreground = new SolidColorBrush() { Color = Colors.White },
				HorizontalAlignment = HorizontalAlignment.Center,
				VerticalAlignment = VerticalAlignment.Center
			};

			Grid Corners = new Grid() { Tag = RectG };

			Border CornerTL = new Border() { Name = "CornerTL", BorderThickness = new Thickness( 5, 5, 0, 0 ) };
			Border CornerTR = new Border() { Name = "CornerTR", BorderThickness = new Thickness( 0, 5, 5, 0 ) };
			Border CornerBR = new Border() { Name = "CornerBR", BorderThickness = new Thickness( 0, 0, 5, 5 ) };
			Border CornerBL = new Border() { Name = "CornerBL", BorderThickness = new Thickness( 5, 0, 0, 5 ) };

			CornerTL.VerticalAlignment = CornerTR.VerticalAlignment = VerticalAlignment.Top;
			CornerBR.VerticalAlignment = CornerBL.VerticalAlignment = VerticalAlignment.Bottom;
			CornerTR.HorizontalAlignment = CornerBR.HorizontalAlignment = HorizontalAlignment.Right;
			CornerTL.HorizontalAlignment = CornerBL.HorizontalAlignment = HorizontalAlignment.Left;

			new Border[] { CornerTL, CornerTR, CornerBL, CornerBR }
			.Do( x =>
			{
				x.Opacity = OPACITY_IDLE;
				x.Width = x.Height = 15;
				x.BorderBrush = new SolidColorBrush() { Color = Colors.White };
				Corners.Children.Add( x );
			} );

			Border HoverShade = new Border
			{
				Name = "HoverShade",
				Background = HoverShadeBrush,
				BorderThickness = new Thickness( 0.2 ),
				BorderBrush = new SolidColorBrush() { Color = Colors.OrangeRed },
				Opacity = OPACITY_IDLE,
				Child = Dim,
				Tag = RectG
			};

			Stage.Children.Add( Corners );
			Stage.Children.Add( HoverShade );

			void UpdateDisplay()
			{
				double Y1 = B.Top - AppVars.W_OFFSET_X;
				double X1 = B.Left - AppVars.W_OFFSET_Y;

				Canvas.SetTop( HoverShade, Y1 );
				Canvas.SetLeft( HoverShade, X1 );
				Canvas.SetTop( Corners, Y1 - 8 );
				Canvas.SetLeft( Corners, X1 - 8 );

				Dim.Text = $"X: {B.X.AsDecimalPlaces( 2 )} Y: {B.Y.AsDecimalPlaces( 2 )} W: {B.Width.AsDecimalPlaces( 2 )} H: {B.Height.AsDecimalPlaces( 2 )}";
			}

			BindMouseDragHandlers( Stage, HoverShade, d =>
			{
				B.X += d.X; B.Y += d.Y;
				ChangeCallback( B );
				UpdateDisplay();
			} );

			void CornerTailOps()
			{
				ChangeCallback( B );

				Corners.Width = B.Width + 16;
				Corners.Height = B.Height + 16;
				HoverShade.Width = B.Width;
				HoverShade.Height = B.Height;
				UpdateDisplay();
			}

			CornerTailOps();

			void BindCorner( Border Corner, Action<System.Windows.Vector> HandleStack )
			{
				BindMouseDragHandlers( Stage, Corner, d =>
				{
					HandleStack( d );
					CornerTailOps();
				} );
			}

			BindCorner( CornerTL, d => { B.X += d.X; B.Y += d.Y; B.Width -= d.X; B.Height -= d.Y; } );
			BindCorner( CornerTR, d => { B.Y += d.Y; B.Width += d.X; B.Height -= d.Y; } );
			BindCorner( CornerBL, d => { B.X += d.X; B.Width -= d.X; B.Height += d.Y; } );
			BindCorner( CornerBR, d => { B.Width += d.X; B.Height += d.Y; } );
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

			Elem.KeyDown += ( s, e ) =>
			{
				e.Handled = true;
				double d = 0;
				if ( e.IsRepeat )
				{
					double t = Step;
					d = ( t *= 0.5 ) * t * t;
					Step += 0.075;
				}
				else
				{
					Step = 0.1;
					d = 0.1;
				}

				switch ( e.Key )
				{
					case Key.Up:
						HandleStack( new System.Windows.Vector( 0, -d ) );
						break;
					case Key.Down:
						HandleStack( new System.Windows.Vector( 0, d ) );
						break;
					case Key.Left:
						HandleStack( new System.Windows.Vector( -d, 0 ) );
						break;
					case Key.Right:
						HandleStack( new System.Windows.Vector( d, 0 ) );
						break;
				}
			};

			Storyboard ElemStory = new Storyboard();
			Elem.DataContext = ElemStory;

			Elem.MouseEnter += ( s, e ) =>
			{
				SimpleStory.DoubleAnimation( ElemStory, Elem, "Opacity", Elem.Opacity, OPACITY_MOUSE_OVER );
				ElemStory.Begin();
			};

			Elem.MouseLeave += ( s, e ) =>
			{
				SimpleStory.DoubleAnimation( ElemStory, Elem, "Opacity", Elem.Opacity
					, Elem.Equals( FocusManager.GetFocusedElement( Stage ) ) ? OPACITY_MOUSE_DOWN : OPACITY_IDLE );
				ElemStory.Begin();
			};

			Elem.MouseDown += ( s, e ) =>
			{
				e.Handled = true;
				if ( FocusManager.GetFocusedElement( Stage ) is FrameworkElement OElem && !Elem.Equals( OElem ) && OElem.DataContext is Storyboard BlurStory )
				{
					SimpleStory.DoubleAnimation( BlurStory, OElem, "Opacity", OElem.Opacity, OPACITY_IDLE );
					BlurStory.Begin();
				}

				FocusManager.SetFocusedElement( Stage, Elem );
				Elem.CaptureMouse();
				DragStart = e.GetPosition( Stage );
				Elem.MouseMove += _OnMove;

				SimpleStory.DoubleAnimation( ElemStory, Elem, "Opacity", Elem.Opacity, OPACITY_IDLE_FOCUS );
				ElemStory.Begin();
			};

			Elem.MouseUp += ( s, e ) =>
			{
				Elem.ReleaseMouseCapture();
				Elem.MouseMove -= _OnMove;

				SimpleStory.DoubleAnimation( ElemStory, Elem, "Opacity", Elem.Opacity, OPACITY_MOUSE_OVER );
				ElemStory.Begin();
			};
		}

	}
}
