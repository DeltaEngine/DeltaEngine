using System;
using System.IO;
using DeltaEngine.Core;
using DeltaEngine.Editor.AppBuilder.Android;
using DeltaEngine.Editor.Messages;

namespace DeltaEngine.Editor.AppBuilder
{
	public abstract class AppInfo
	{
		protected AppInfo(string fullAppDataFilePath, Guid appGuid, PlatformName platform,
			DateTime buildDate)
		{
			FilePath = fullAppDataFilePath;
			AppGuid = appGuid;
			Name = Path.GetFileNameWithoutExtension(fullAppDataFilePath);
			Platform = platform;
			BuildDate = buildDate;
		}

		public string FilePath { get; private set; }
		public Guid AppGuid { get; private set; }
		public string Name { get; protected set; }
		public PlatformName Platform { get; private set; }
		public DateTime BuildDate { get; private set; }

		public Device[] AvailableDevices
		{
			get
			{
				if (availableDevices == null)
					UpdateAvailableDevices();
				return availableDevices;
			}
		}

		private Device[] availableDevices;

		protected void UpdateAvailableDevices()
		{
			try
			{
				availableDevices = TryGetAvailableDevices();
			}
			catch (Exception)
			{
				availableDevices = null;
			}
		}

		private Device[] TryGetAvailableDevices()
		{
			return GetAvailableDevices();
		}

		public class NoDevicesAvailable : Exception { }

		protected abstract Device[] GetAvailableDevices();

		public virtual bool IsDeviceAvailable
		{
			get { return AvailableDevices != null && AvailableDevices.Length > 0; }
		}

		public string SolutionFilePath { get; set; }

		public bool IsSolutionPathAvailable
		{
			get { return SolutionFilePath != null && SolutionFilePath.EndsWith(".sln"); }
		}

		public void LaunchAppOnPrimaryDevice()
		{
			if (!IsDeviceAvailable)
				throw new NoDeviceAvailable(this);
			LaunchAppOnDevice(AvailableDevices[0]);
		}

		public class NoDeviceAvailable : Exception
		{
			public NoDeviceAvailable(AppInfo appInfo)
				: base(appInfo.ToString()) {}
		}

		public void LaunchAppOnDevice(Device device)
		{
			if (device == null)
				throw new NoDeviceSpecified();
			Logger.Info("Launching App " + Name + " on " + device.Name);
			device.Launch(this);
			Logger.Info("Done Launching App " + Name);
		}

		public class NoDeviceSpecified : Exception {}

		public override string ToString()
		{
			return GetType().Name + "(" + Name + " for " + Platform + ")";
		}

		public string GetFullAppNameForEngineApp()
		{
			return this is AndroidAppInfo ? "net.DeltaEngine." + Name : Name;
		}

		public string PlatformIcon
		{
			get
			{
				return "pack://application:,,,/DeltaEngine.Editor;component/Images/AppBuilder/" +
					Platform + ".png";
			}
		}
	}
}