using System.Diagnostics;
using System.Windows;

namespace ScreenMask
{
	public class MaskableProcess
	{
		private Process P;
		public string ProcessName => P.ProcessName;

		public bool HasMainWindow { get; private set; } = false;
		public bool CanFit { get; private set; } = false;

		public string Remarks { get; set; }

		private Rect _RawBounds;
		public Rect Bounds { get; private set; }

		public MaskableProcess( Process P )
		{
			this.P = P;

			if ( P.Id != AppVars.CurrentPID )
			{
				CheckMaskability();
			}
		}

		private void CheckMaskability()
		{
			_RawBounds = P.GetWindowRect();

			Rect Adj = _RawBounds;
			Adj.X -= AppVars.W_OFFSET_X + 1;

			double d = Adj.Width + 2 * ( AppVars.W_OFFSET_X + 1 );
			Adj.Width = 0 < d ? d : 0;

			d = Adj.Height + AppVars.W_OFFSET_Y + 1;
			Adj.Height = 0 < d ? d : 0;

			Bounds = Adj;

			HasMainWindow = ( 0 < Bounds.Width && 0 < Bounds.Height );
			CanFit = ( HasMainWindow
				&& 0 < ( Bounds.Left + Bounds.Width ) && 0 < ( Bounds.Top + Bounds.Height )
			);

			if ( !CanFit )
			{
				Remarks = "Process is currently minimized or has no active window";
			}
		}
	}
}