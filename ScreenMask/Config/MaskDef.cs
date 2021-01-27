using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;

namespace ScreenMask.Config
{
	public class MaskDef
	{
		public Rect Rect { get; set; }
		public string ProfileId { get; set; }
		public bool AlwaysOnTop { get; set; } = true;
		public byte[] BgColor { get; set; } = new byte[] { 255, 0, 0, 0 };
	}
}
