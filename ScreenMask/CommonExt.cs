using System;
using System.Collections.Generic;
using System.Text;

namespace ScreenMask
{
	public static class CommonExt
	{
		public static decimal AsDecimalPlaces( this double v, int p ) => decimal.Round( ( decimal ) v, p );
	}
}
