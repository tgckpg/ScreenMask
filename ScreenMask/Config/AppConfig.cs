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
		private Configuration Conf;

		private static AppConfig _Current;
		public static AppConfig Current => _Current ??= new AppConfig
		{
			Conf = ConfigurationManager.OpenExeConfiguration( ConfigurationUserLevel.None )
		};

		public void Save() => Conf.Save( ConfigurationSaveMode.Modified );

		public IList<MaskDef> Masks
		{
			get => JsonConvert.DeserializeObject<MaskDef[]>( Conf.AppSettings.Settings[ "Masks" ]?.Value ?? "[]" );
			set
			{
				Conf.AppSettings.Settings.Remove( "Masks" );
				Conf.AppSettings.Settings.Add( new KeyValueConfigurationElement( "Masks", JsonConvert.SerializeObject( value ) ) );
			}
		}

		public Point GadgetPos
		{
			get => JsonConvert.DeserializeObject<Point>( Conf.AppSettings.Settings[ "MWPos" ]?.Value ?? "\"0,0\"" );
			set
			{
				Conf.AppSettings.Settings.Remove( "MWPos" );
				Conf.AppSettings.Settings.Add( new KeyValueConfigurationElement( "MWPos", JsonConvert.SerializeObject( value ) ) );
			}
		}

		public string Mode
		{
			get => Conf.AppSettings.Settings[ "Mode" ]?.Value ?? "Default";
			set
			{
				Conf.AppSettings.Settings.Remove( "Mode" );
				Conf.AppSettings.Settings.Add( new KeyValueConfigurationElement( "Mode", value ) );
			}
		}

		public bool AlwaysOnTop
		{
			get => JsonConvert.DeserializeObject<bool>( Conf.AppSettings.Settings[ "AlwaysOnTop" ]?.Value ?? "false" );
			set
			{
				Conf.AppSettings.Settings.Remove( "AlwaysOnTop" );
				Conf.AppSettings.Settings.Add( new KeyValueConfigurationElement( "AlwaysOnTop", JsonConvert.SerializeObject( value ) ) );
			}
		}

	}
}
