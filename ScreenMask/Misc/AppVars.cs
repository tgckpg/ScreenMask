using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace ScreenMask
{
	public class AppVars
	{
		public static double W_OFFSET_X = -8;
		public static double W_OFFSET_Y = -8;

		private static int? _PID;
		public static int CurrentPID => _PID ??= Process.GetCurrentProcess().Id;
	}
}
