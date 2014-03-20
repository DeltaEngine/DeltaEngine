using System.Runtime.InteropServices;

namespace DeltaEngine.Platforms.Windows
{
	[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
	internal class WindowsDisplayDevice
	{
		internal int cb = 0;
		[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
		internal string deviceName;
		[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
		internal string deviceString;
		internal int stateFlags;
		[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
		internal string deviceID;
		[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
		internal string deviceKey;
	}
}