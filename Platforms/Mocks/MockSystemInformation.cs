using System;
using DeltaEngine.Datatypes;

namespace DeltaEngine.Platforms.Mocks
{
	/// <summary>
	/// Mock system information used in unit testing when real information would be too slow
	/// to obtain.
	/// </summary>
	public class MockSystemInformation : SystemInformation
	{
		public override float AvailableRam
		{
			get { return 1024; }
		}
		public override int CoreCount
		{
			get { return 1; }
		}
		public override string CpuName
		{
			get { return "MockCpu"; }
		}
		public override float CpuSpeed
		{
			get { return 1000; }
		}
		public override float[] CpuUsage
		{
			get { return new float[] { 0 }; }
		}
		public override string GpuName
		{
			get { return "MockGpu"; }
		}
		public override bool IsConsole
		{
			get { return false; }
		}
		public override bool IsMobileDevice
		{
			get { return false; }
		}
		public override bool IsTablet
		{
			get { return false; }
		}
		public override string MachineName
		{
			get { return "MockMachineName"; }
		}
		public override float MaxRam
		{
			get { return 1024; }
		}
		public override Size MaxResolution
		{
			get { return new Size(1920, 1080); }
		}
		public override NetworkState NetworkState
		{
			get { return NetworkState.Disconnected; }
		}
		public override string PlatformName
		{
			get { return "Windows"; }
		}
		public override Version PlatformVersion
		{
			get { return new Version(6, 2); }
		}
		public override bool SoundCardAvailable
		{
			get { return true; }
		}
		public override float UsedRam
		{
			get { return 148; }
		}
		public override string Username
		{
			get { return "MockUsername"; }
		}
		public override Version Version
		{
			get { return new Version(0, 9, 8, 3); }
		}
	}
}