using GR.Effects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
	public partial class FitWindow : Window
	{
		public Rect TargetBounds { get; private set; }
		public string ProfileId { get; private set; }
		private Storyboard PreviewStory = new Storyboard();

		public FitWindow()
		{
			InitializeComponent();
		}

		private void Window_Loaded( object sender, RoutedEventArgs e )
		{
			Storyboard BStory = new Storyboard() { AutoReverse = true, RepeatBehavior = RepeatBehavior.Forever };
			SimpleStory.DoubleAnimation( BStory, SelBounds, "Opacity", 1, 0.5, 1000 );
			BStory.Begin();

			var j = ShowProcessList();
		}

		private async Task ShowProcessList()
		{
			await Task.Delay( 0 );
			SelectProcess SelProc = new SelectProcess
			{
				Owner = this,
				SelectedCallback = MoveBounds
			};

			if ( SelProc.ShowDialog() == true )
			{
				DialogResult = true;
			}

			Close();
		}

		private void MoveBounds( string ProcessId, string WindowId, Rect R )
		{
			ProfileId = $"{ProcessId}\n{WindowId}";

			TargetBounds = R;
			SimpleStory.DoubleAnimation( PreviewStory, this, "Top", this.Top, R.Top, 250, 250 );
			SimpleStory.DoubleAnimation( PreviewStory, this, "Left", this.Left, R.Left, 250 );
			SimpleStory.DoubleAnimation( PreviewStory, this, "Width", this.Width, R.Width, 250 );
			SimpleStory.DoubleAnimation( PreviewStory, this, "Height", this.Height, R.Height, 250, 250 );
			PreviewStory.Begin();
		}
	}
}
