using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text.Json;
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

		public IEnumerable<MaskDef> Masks
		{
			get => JsonSerializer.Deserialize<MaskDef[]>( Conf.AppSettings.Settings[ "Masks" ]?.Value ?? "[]" );
			set
			{
				Conf.AppSettings.Settings.Remove( "Masks" );
				Conf.AppSettings.Settings.Add( new KeyValueConfigurationElement( "Masks", JsonSerializer.Serialize( value ) ) );
			}
		}

		public Point GadgetPos
		{
			get => JsonSerializer.Deserialize<Point>( Conf.AppSettings.Settings[ "MWPos" ]?.Value ?? "{\"X\":0,\"Y\":0}" );
			set
			{
				Conf.AppSettings.Settings.Remove( "MWPos" );
				Conf.AppSettings.Settings.Add( new KeyValueConfigurationElement( "MWPos", JsonSerializer.Serialize( value ) ) );
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
			get => JsonSerializer.Deserialize<bool>( Conf.AppSettings.Settings[ "AlwaysOnTop" ]?.Value ?? "false" );
			set
			{
				Conf.AppSettings.Settings.Remove( "AlwaysOnTop" );
				Conf.AppSettings.Settings.Add( new KeyValueConfigurationElement( "AlwaysOnTop", JsonSerializer.Serialize( value ) ) );
			}
		}

		public IEnumerable<ProcessProfile> ProcessProfiles
		{
			get
			{
				JsonSerializerOptions Options = new JsonSerializerOptions();
				Options.Converters.Add( new Converters.JsonVector4Converter() );
				return JsonSerializer.Deserialize<ProcessProfile[]>( Conf.AppSettings.Settings[ "ProcessProfiles" ]?.Value ?? "[]", Options );
			}
			set
			{
				Conf.AppSettings.Settings.Remove( "ProcessProfiles" );
				JsonSerializerOptions Options = new JsonSerializerOptions();
				Options.Converters.Add( new Converters.JsonVector4Converter() );
				Conf.AppSettings.Settings.Add( new KeyValueConfigurationElement( "ProcessProfiles", JsonSerializer.Serialize( value, Options ) ) );
			}
		}

	}
}
