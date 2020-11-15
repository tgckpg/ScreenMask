using System;
using System.Collections.Generic;
using System.Diagnostics;
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
	public partial class SelectProcess : Window
	{
		public Action<Rect> SelectedCallback { get; set; }

		private MaskableProcess Selected;

		public SelectProcess()
		{
			InitializeComponent();
		}

		public new void Show() => throw new Exception( "Please use ShowDialog" );

		private void Window_Loaded( object sender, RoutedEventArgs e )
		{
			Win32Calls.HideFromAltTab( this );
			RefreshProcessList();
		}

		private void Button_Click( object sender, RoutedEventArgs e ) => RefreshProcessList();

		private void RefreshProcessList()
			=> ProcList.ItemsSource = Process.GetProcesses()
				.Select( x => new MaskableProcess( x ) )
				.Where( x => x.HasMainWindow )
				.OrderBy( x => x.ProcessName );

		private void ProcList_SelectionChanged( object sender, SelectionChangedEventArgs e )
		{
			Selected = e.AddedItems.CastOnly<MaskableProcess>().FirstOrDefault();
			UpdateBounds();
		}

		private void UpdateBounds()
		{
			if ( Selected?.CanFit != true )
				return;
			Rect B = Selected.Bounds;

			_ = int.TryParse( OffsetIX.Text, out int _X );
			_ = int.TryParse( OffsetIY.Text, out int _Y );
			_ = int.TryParse( OffsetIW.Text, out int _W );
			_ = int.TryParse( OffsetIH.Text, out int _H );

			Point P = this.GetDpiScale();
			B.X += _X;
			B.Y += _Y;
			B.Width += _W;
			B.Height += _H;
			B.X /= P.X;
			B.Y /= P.Y;
			B.Width /= P.X;
			B.Height /= P.Y;
			SelectedCallback?.Invoke( B );
		}

		private void ProcList_MouseDoubleClick( object sender, MouseButtonEventArgs e )
		{
			if ( Selected?.CanFit == true )
			{
				DialogResult = true;
			}
		}

		private void Offsets_TextChanged( object sender, TextChangedEventArgs e )
			=> UpdateBounds();

	}
}
