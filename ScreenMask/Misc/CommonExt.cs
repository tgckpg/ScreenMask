using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Windows;
using System.Windows.Media;

namespace ScreenMask
{
	public static class CommonExt
	{
		public static decimal AsDecimalPlaces( this double v, int p ) => decimal.Round( ( decimal ) v, p );

		public static IEnumerable<T> CastOnly<T>( this IEnumerable Source )
		{
			foreach ( object O in Source )
				if ( O is T _R )
					yield return _R;
		}

		public static void Do<T>( this IEnumerable<T> Source, Action<T> Callback )
		{
			foreach ( T x in Source )
				Callback( x );
		}

		public static System.Windows.Point GetDpiScale( this Visual V )
		{
			PresentationSource source = PresentationSource.FromVisual( V );

			return new System.Windows.Point(
				source.CompositionTarget.TransformToDevice.M11,
				source.CompositionTarget.TransformToDevice.M22
			);
		}
	}
}
