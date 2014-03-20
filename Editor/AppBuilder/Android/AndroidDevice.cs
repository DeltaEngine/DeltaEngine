using DeltaEngine.Core;

namespace DeltaEngine.Editor.AppBuilder.Android
{
	/// <summary>
	/// Represents a Android device (emulator or real connected one) that provides the functionality
	/// to install, uninstall and launch applications on it.
	/// </summary>
	public class AndroidDevice : Device
	{
		public AndroidDevice(AndroidDebugBridgeRunner adbRunner, AndroidDeviceInfo deviceInfo)
		{
			this.adbRunner = adbRunner;
			AdbId = deviceInfo.AdbDeviceId;
			state = deviceInfo.DeviceState;
			Name = adbRunner.GetDeviceName(AdbId);
			IsEmulator = Name.StartsWith("emulator");
			Logger.Info("\t" + this);
		}

		private readonly AndroidDebugBridgeRunner adbRunner;
		internal string AdbId { get; private set; }
		private readonly string state;

		public bool IsConnected
		{
			get { return state == ConnectedState; }
		}

		private const string ConnectedState = "device";
		private const string DisconnectedState = "offline";

		public override bool IsAppInstalled(AppInfo app)
		{
			return adbRunner.IsAppInstalled(this, app.GetFullAppNameForEngineApp());
		}

		protected override void InstallApp(AppInfo app)
		{
			adbRunner.InstallPackage(this, app.FilePath);
		}

		protected override void UninstallApp(AppInfo app)
		{
			adbRunner.UninstallPackage(this, app.GetFullAppNameForEngineApp());
		}

		protected override void LaunchApp(AppInfo app)
		{
			adbRunner.StartEngineBuiltApplication(this, app.GetFullAppNameForEngineApp());
		}

		public override string ToString()
		{
			return GetType().Name + "(" + Name + ")";
		}
	}
}