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

		private ProcessWindowInfo Selected;

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
				.GetWindowRects()
				.SelectMany( x => x.Item2, ( Ps, WRs ) => (Ps.Item1, WRs.Item1, WRs.Item2) )
				.Select( x => new ProcessWindowInfo( x.Item1, x.Item2, x.Item3 ) )
				.Where( x => x.HasBound && !string.IsNullOrEmpty( x.Title ) )
				.OrderBy( x => x.Process.ProcessName.ToUpper() )
				.ThenByDescending( x => x.Title );

		private void ProcList_SelectionChanged( object sender, SelectionChangedEventArgs e )
		{
			Selected = e.AddedItems.CastOnly<ProcessWindowInfo>().FirstOrDefault();
			UpdateBounds();
		}

		private void UpdateBounds()
		{
			if ( Selected?.CanFit != true )
				return;

			Rect B = Selected.Bound;

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
