using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace ScreenMask
{
	public class ProcessWindowInfo
	{
		public Process Process { get; private set; }
		public BitmapSource ProcessIcon
		{
			get
			{
				Icon ico = Process.GetIcon();
				if ( ico == null )
					return null;
				return Imaging.CreateBitmapSourceFromHIcon( ico.Handle, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions() );
			}
		}

		public string Title { get; private set; }

		public bool HasBound { get; private set; } = false;
		public bool CanFit { get; private set; } = false;

		public string Remarks { get; set; }

		public Rect Bound { get; private set; }

		public ProcessWindowInfo( Process P, string Title, Rect R )
		{
			this.Process = P;
			this.Title = Title;

			HasBound = ( 0 < R.Width && 0 < R.Height );

			R.X -= AppVars.W_OFFSET_X + 1;

			double d = R.Width + 2 * ( AppVars.W_OFFSET_X + 1 );
			R.Width = 0 < d ? d : 0;

			d = R.Height + AppVars.W_OFFSET_Y + 1;
			R.Height = 0 < d ? d : 0;

			Bound = R;

			CanFit = ( HasBound
				&& 0 < ( Bound.Left + Bound.Width ) && 0 < ( Bound.Top + Bound.Height )
			);

			if ( !CanFit )
			{
				Remarks = "Window is currently minimized or inactive";
			}
		}
	}
}