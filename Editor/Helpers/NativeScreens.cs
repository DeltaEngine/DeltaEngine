using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace DeltaEngine.Editor.Helpers
{
	internal class NativeScreens
	{
		/// <summary>
		/// Another way to use this is System.Windows.Forms.Screen.AllScreens;
		/// </summary>
		public IEnumerable<NativeRect> GetDisplayWorkAreas()
		{
			var screens = new List<NativeRect>();
			EnumDisplayMonitors(IntPtr.Zero, IntPtr.Zero,
				delegate(IntPtr hMonitor, IntPtr hdcMonitor, ref NativeRect lprcMonitor, IntPtr dwData)
				{
					var monitor = new MonitorInfo();
					monitor.size = (uint)Marshal.SizeOf(monitor);
					bool success = GetMonitorInfo(hMonitor, ref monitor);
					if (success)
						screens.Add(monitor.workArea);
					return true;
				}, IntPtr.Zero);
			return screens;
		}

		[DllImport("user32.dll")]
		private static extern bool EnumDisplayMonitors(IntPtr hdc, IntPtr lprcClip,
			MonitorEnumDelegate lpfnEnum, IntPtr dwData);

		private delegate bool MonitorEnumDelegate(
			IntPtr hMonitor, IntPtr hdcMonitor, ref NativeRect lprcMonitor, IntPtr dwData);

		[DllImport("user32.dll")]
		private static extern bool GetMonitorInfo(IntPtr hmon, ref MonitorInfo mi);

		[StructLayout(LayoutKind.Sequential)]
		private struct MonitorInfo
		{
			public uint size;
			private readonly NativeRect monitor;
			public readonly NativeRect workArea;
			private readonly uint flags;
		}

		[StructLayout(LayoutKind.Sequential)]
		internal struct NativeRect
		{
			public readonly int left;
			public readonly int top;
			public readonly int right;
			public readonly int bottom;
		}
	}
}