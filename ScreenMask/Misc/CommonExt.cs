using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
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

		public static string GetBinId(this Process P)
		{
			string B = P.MainModule.FileName;
			using ( SHA256 Hasher = SHA256.Create() )
			using ( Stream s = File.OpenRead( P.MainModule.FileName ) )
			{
				byte[] Buffer = new byte[ 4096 ];
				if( 0 < s.Read( Buffer, 0, 4096 ) )
				{
					byte[] Hash = Hasher.ComputeHash( Buffer );
					B = string.Concat( Hash.Select( x => $"{x:X2}" ) );
				}

			}
			return B;
		}
	}
}
