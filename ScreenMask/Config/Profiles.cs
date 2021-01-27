using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ScreenMask.Config
{
	class ProfileData
	{
		private static List<ProcessProfile> _Process;
		public static List<ProcessProfile> ProcessProfiles => ( _Process ??= new List<ProcessProfile>( AppConfig.Current.ProcessProfiles ) );
	}

	static class ProfileExt
	{
		public static ProcessProfile GetOrCreate( this List<ProcessProfile> Profiles, string BinId )
		{
			ProcessProfile Profile = Profiles.Where( x => x.BinId == BinId ).FirstOrDefault();
			if( Profile == null)
			{
				Profile = new ProcessProfile() { BinId = BinId, WindowProfiles = new List<WindowProfile>() };
				Profiles.Add( Profile );
			}

			return Profile;
		}
		public static WindowProfile GetOrCreate( this List<WindowProfile> Profiles, string Title )
		{
			WindowProfile Profile = Profiles.Where( x => x.Title == Title ).FirstOrDefault();
			if( Profile == null)
			{
				Profile = new WindowProfile() { Title = Title };
				Profiles.Add( Profile );
			}

			return Profile;
		}
	}

	class ProcessProfile
	{
		public string BinId { get; set; }
		public List<WindowProfile> WindowProfiles { get; set; }
	}

	class WindowProfile
	{
		public string Title { get; set; }
		public Vector4 Offsets { get; set; } = Vector4.Zero;
		public Rect Bounds { get; set; }
	}
}
