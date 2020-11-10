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
		public Process SelectedProcess { get; set; }
		public SelectProcess()
		{
			InitializeComponent();
		}

		private void Window_Loaded( object sender, RoutedEventArgs e )
		{
			ProcList.ItemsSource = Process.GetProcesses().Where( x =>
			{
				Rect R = x.GetWindowRect();
				return 0 < R.Width && 0 < R.Height;
			} ).OrderBy( x => x.ProcessName );
		}

		private void ProcList_SelectionChanged( object sender, SelectionChangedEventArgs e )
		{
			SelectedProcess = ( Process ) e.AddedItems[ 0 ];
			Close();
		}
	}
}
