﻿using ScreenMask.Config;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace ScreenMask
{
	public partial class SelectProcess : Window
	{
		public Action<Rect> SelectedCallback { get; set; }

		private ProcessWindowInfo Selected;

		private WindowProfile Profile => ProfileData
				.ProcessProfiles.GetOrCreate( Selected.Process.GetBinId() )
				.WindowProfiles.GetOrCreate( Selected.Title );

		public SelectProcess()
		{
			InitializeComponent();
		}

		public new void Show() => throw new Exception( "Please use ShowDialog" );

		private void Window_Loaded( object sender, RoutedEventArgs e )
		{
			Win32Calls.HideFromAltTab( this );
			RefreshProcessList();
		}

		private void Button_Click( object sender, RoutedEventArgs e ) => RefreshProcessList();

		private void RefreshProcessList()
			=> ProcList.ItemsSource = Process.GetProcesses()
				.GetWindowRects()
				.SelectMany( x => x.Item2, ( Ps, WRs ) => (Ps.Item1, WRs.Item1, WRs.Item2) )
				.Select( x => new ProcessWindowInfo( x.Item1, x.Item2, x.Item3 ) )
				.Where( x => x.HasBound && !string.IsNullOrEmpty( x.Title ) )
				.OrderBy( x => x.Process.ProcessName.ToUpper() )
				.ThenByDescending( x => x.Title );

		private void ProcList_SelectionChanged( object sender, SelectionChangedEventArgs e )
		{
			Selected = e.AddedItems.CastOnly<ProcessWindowInfo>().FirstOrDefault();
			if ( Selected == null )
			{
				OffsetIX.Text = OffsetIY.Text = OffsetIW.Text = OffsetIH.Text = "0";
			}
			else
			{
				OffsetIX.Text = Profile.Offsets.X.ToString();
				OffsetIY.Text = Profile.Offsets.Y.ToString();
				OffsetIW.Text = Profile.Offsets.W.ToString();
				OffsetIH.Text = Profile.Offsets.Z.ToString();
				UpdateBounds();
			}
		}

		private void UpdateBounds()
		{
			if ( Selected?.CanFit != true )
				return;

			Rect B = Selected.Bound;

			_ = float.TryParse( OffsetIX.Text, out float _X );
			_ = float.TryParse( OffsetIY.Text, out float _Y );
			_ = float.TryParse( OffsetIW.Text, out float _W );
			_ = float.TryParse( OffsetIH.Text, out float _H );

			Profile.Offsets = new Vector4( _X, _Y, _H, _W );

			Point P = this.GetDpiScale();
			B.X += _X;
			B.Y += _Y;
			B.Width += _W;
			B.Height += _H;
			B.X /= P.X;
			B.Y /= P.Y;
			B.Width /= P.X;
			B.Height /= P.Y;
			SelectedCallback?.Invoke( B );
		}

		private void ProcList_MouseDoubleClick( object sender, MouseButtonEventArgs e )
		{
			if ( Selected?.CanFit == true )
			{
				DialogResult = true;
			}
		}

		private void Offsets_TextChanged( object sender, TextChangedEventArgs e )
			=> UpdateBounds();

	}
}
