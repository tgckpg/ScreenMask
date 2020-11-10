using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Windows;

namespace ScreenMask.Config
{
	class AppConfig
	{
		private static Configuration Conf;

		public static void Read() => Conf = ConfigurationManager.OpenExeConfiguration( ConfigurationUserLevel.None );
		public static void Save() => Conf.Save( ConfigurationSaveMode.Modified );

		public static MaskDef[] GetMasks()
			=> JsonConvert.DeserializeObject<MaskDef[]>( Conf.AppSettings.Settings[ "Masks" ]?.Value ?? "[]" );

		public static Point GetMainWindowPos()
			=> JsonConvert.DeserializeObject<Point>( Conf.AppSettings.Settings[ "MWPos" ]?.Value ?? "\"0,0\"" );

		public static void SetMasks( IList<MaskDef> Masks )
		{
			Conf.AppSettings.Settings.Remove( "Masks" );
			Conf.AppSettings.Settings.Add( new KeyValueConfigurationElement( "Masks", JsonConvert.SerializeObject( Masks ) ) );
		}

		internal static void SetMainWindowPos( double top, double left )
		{
			Conf.AppSettings.Settings.Remove( "MWPos" );
			Conf.AppSettings.Settings.Add( new KeyValueConfigurationElement( "MWPos", JsonConvert.SerializeObject( new Point( top, left ) ) ) );
		}
	}
}
