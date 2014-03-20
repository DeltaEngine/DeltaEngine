using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Management;
using System.Reflection;
using System.Runtime.InteropServices;
using DeltaEngine.Core;
using DeltaEngine.Datatypes;
using Microsoft.Win32;

namespace DeltaEngine.Platforms.Windows
{
	/// <summary>
	/// Provides static information that does not change over the course of a running application 
	/// or does not need updates.
	/// </summary>
	public class WindowsSystemInformation : SystemInformation
	{
		public override float AvailableRam
		{
			get
			{
				if (availableRamCounter == null)
					availableRamCounter = new PerformanceCounter("Memory", "Available Bytes");
				return availableRamCounter.NextValue() / (1024.0f * 1024.0f);
			}
		}

		private PerformanceCounter availableRamCounter;

		public override int CoreCount
		{
			get { return Environment.ProcessorCount; }
		}

		public override string CpuName
		{
			get
			{
				if (cpuName == null)
					FillCoreInfo();
				return cpuName;
			}
		}

		private string cpuName;

		private void FillCoreInfo()
		{
			RegistryKey regKey = Registry.LocalMachine;
			regKey = regKey.OpenSubKey("HARDWARE\\DESCRIPTION\\System\\CentralProcessor\\0");
			cpuSpeed = (int)regKey.GetValue("~MHz");
			cpuName = (string)regKey.GetValue("ProcessorNameString");
			var display = new WindowsDisplayDevice();
			display.cb = Marshal.SizeOf(display);
			uint deviceNum = 0;
			var gpuNames = new List<string>();
			while (EnumDisplayDevices(null, deviceNum++, display, 0))
				if (IsRelevantDevice(display, gpuNames))
					gpuNames.Add(display.deviceString);
			gpuName = string.Join(",", gpuNames);
		}

		private float cpuSpeed;

		private static bool IsRelevantDevice(WindowsDisplayDevice display, List<string> gpuNames)
		{
			return gpuNames.Contains(display.deviceString) == false &&
				display.deviceString.Trim().Length > 0 && display.deviceString.Contains("RDP") == false;
		}

		[DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
		[return: MarshalAs(UnmanagedType.Bool)]
		private static extern bool EnumDisplayDevices(
			[MarshalAs(UnmanagedType.LPTStr)] string lpDevice, uint iDevNum,
			[In, Out] WindowsDisplayDevice lpDisplayDevice, uint dwFlags);

		private string gpuName;

		public override float CpuSpeed
		{
			get
			{
				if (cpuSpeed == 0.0f)
					FillCoreInfo();
				return cpuSpeed;
			}
		}

		public override float[] CpuUsage
		{
			get
			{
				if (cpuUsageArray == null)
					FormCpuUsageArray();
				for (int index = 0; index < CoreCount; index++)
					cpuUsageArray[index] = cpuUsageCounters[index].NextValue();
				return cpuUsageArray;
			}
		}

		private void FormCpuUsageArray()
		{
			cpuUsageArray = new float[CoreCount];
			cpuUsageCounters = new PerformanceCounter[CoreCount];
			for (int index = 0; index < CoreCount; index++)
				cpuUsageCounters[index] = new PerformanceCounter("Processor", "% Processor Time",
					index.ToString(CultureInfo.InvariantCulture));
		}

		private float[] cpuUsageArray;
		private PerformanceCounter[] cpuUsageCounters;

		public override string GpuName
		{
			get
			{
				if (gpuName == null)
					FillCoreInfo();
				return gpuName;
			}
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
			get { return Environment.MachineName; }
		}

		public override float MaxRam
		{
			get
			{
				if (maxRam == 0.0f)
					SetMaxRam();
				return maxRam;
			}
		}

		private float maxRam;

		private void SetMaxRam()
		{
			var wmiOperatingSysm = new ManagementClass("Win32_OperatingSystem");
			ManagementObjectCollection operatingSystems = wmiOperatingSysm.GetInstances();
			foreach (ManagementObject operatingSystem in operatingSystems)
			{
				maxRam = Convert.ToInt32(operatingSystem.GetPropertyValue("TotalVisibleMemorySize")) /
					1024.0f;
				break;
			}
		}

		public override Size MaxResolution
		{
			get
			{
				return new Size(System.Windows.Forms.SystemInformation.PrimaryMonitorSize.Width,
					System.Windows.Forms.SystemInformation.PrimaryMonitorSize.Height);
			}
		}

		public override NetworkState NetworkState
		{
			get
			{
				try
				{
					int desc;
					return InternetGetConnectedState(out desc, 0)
						? NetworkState.ConnectedViaWifiNetwork : NetworkState.Disconnected;
				}
				catch
				{
					return NetworkState.Disconnected;
				}
			}
		}

		[DllImport("wininet.dll")]
		private static extern bool InternetGetConnectedState(out int description, int reservedValue);

		public override string PlatformName
		{
			get { return "Windows"; }
		}

		public override Version PlatformVersion
		{
			get { return Environment.OSVersion.Version; }
		}

		public override bool SoundCardAvailable
		{
			get
			{
				if (cachedNumAudioDevices == -1)
					cachedNumAudioDevices = (int)waveOutGetNumDevs();
				return cachedNumAudioDevices > 0;
			}
		}

		private int cachedNumAudioDevices = -1;

		[DllImport("winmm.dll", SetLastError = true)]
		private static extern uint waveOutGetNumDevs();

		public override float UsedRam
		{
			get
			{
				if (usedRamCounter == null)
					GetUsedRamCounter();
				return usedRamCounter == null ? 0.0f : usedRamCounter.NextValue() / (1024.0f * 1024.0f);
			}
		}

		private PerformanceCounter usedRamCounter;

		private void GetUsedRamCounter()
		{
			try
			{
				TryGetUsedRamCounter();
			}
			catch (Exception ex)
			{
				Logger.Warning("Unable to get used ram: " + ex);
			}
		}

		private void TryGetUsedRamCounter()
		{
			usedRamCounter = new PerformanceCounter("Memory", "Committed Bytes");
		}

		public override string Username
		{
			get { return Environment.UserName; }
		}

		public override Version Version
		{
			get { return version ?? (version = Assembly.GetExecutingAssembly().GetName().Version); }
		}

		private Version version;
	}
}