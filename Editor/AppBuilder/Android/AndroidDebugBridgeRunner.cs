using System;
using System.Collections.Generic;
using System.IO;
using DeltaEngine.Core;
using DeltaEngine.Editor.Core;
using DeltaEngine.Extensions;

namespace DeltaEngine.Editor.AppBuilder.Android
{
	/// <summary>
	/// Runs the ADB tool (which is provided by the Android SDK) via command line.
	/// <see cref="http://developer.android.com/tools/help/adb.html"/>
	/// </summary>
	public class AndroidDebugBridgeRunner
	{
		public AndroidDebugBridgeRunner()
		{
			adbProvider = new AdbPathProvider();
		}

		private readonly AdbPathProvider adbProvider;

		public AndroidDeviceInfo[] GetInfosOfAvailableDevices()
		{
			var androidDevicesNames = new List<AndroidDeviceInfo>();
			var processRunner = CreateAdbProcess("devices");
			processRunner.StandardOutputEvent += outputMessage =>
			{
				if (IsDeviceName(outputMessage))
					androidDevicesNames.Add(CreateDeviceInfo(outputMessage));
			};
			RunAdbProcess(processRunner);
			return androidDevicesNames.ToArray();
		}

		private ProcessRunner CreateAdbProcess(string arguments)
		{
			return new ProcessRunner(adbProvider.GetAdbPath(), arguments, 15 * 1000);
		}

		private static bool IsDeviceName(string devicesRequestMessage)
		{
			return !(devicesRequestMessage.StartsWith("list", StringComparison.OrdinalIgnoreCase) ||
				String.IsNullOrWhiteSpace(devicesRequestMessage));
		}

		private static AndroidDeviceInfo CreateDeviceInfo(string deviceInfoString)
		{
			string[] infoParts = deviceInfoString.Split('\t');
			return new AndroidDeviceInfo
			{
				AdbDeviceId = infoParts[0],
				DeviceState = infoParts.Length > 1 ? infoParts[1] : ""
			};
		}

		private static void RunAdbProcess(ProcessRunner adbProcess)
		{
			try
			{
				TryRunAdbProcess(adbProcess);
			}
			catch (ProcessRunner.ProcessTerminatedWithError)
			{
				Logger.Warning("Output:" + adbProcess.Output);
				Logger.Warning("Error:" + adbProcess.Errors);
				throw;
			}
		}

		private static void TryRunAdbProcess(ProcessRunner adbProcess)
		{
			adbProcess.Start();
		}

		public void InstallPackage(AndroidDevice device, string apkFilePath)
		{
			try
			{
				TryInstallPackage(device, apkFilePath);
			}
			catch (ProcessRunner.ProcessTerminatedWithError)
			{
				throw new InstallationFailedOnDevice(device.Name);
			}
		}

		private void TryInstallPackage(AndroidDevice device, string apkFilePath)
		{
			RunAdbProcess("-s " + device.AdbId + " install -r " +
				PathExtensions.GetAbsolutePath(apkFilePath));
		}

		private void RunAdbProcess(string arguments)
		{
			ProcessRunner adbProcess = CreateAdbProcess(arguments);
			RunAdbProcess(adbProcess);
		}

		public class InstallationFailedOnDevice : Exception
		{
			public InstallationFailedOnDevice(string deviceName) : base(deviceName) {}
		}

		public void UninstallPackage(AndroidDevice device, string fullAppName)
		{
			try
			{
				TryUninstallPackage(device, fullAppName);
			}
			// ncrunch: no coverage start
			catch (ProcessRunner.ProcessTerminatedWithError)
			{
				throw new UninstallationFailedOnDevice(device.Name);
			}			
		}

		private void TryUninstallPackage(AndroidDevice device, string fullAppName)
		{
			RunAdbProcess("-s " + device.AdbId + " shell pm uninstall " + fullAppName);
		}

		public class UninstallationFailedOnDevice : Exception
		{
			public UninstallationFailedOnDevice(string deviceName) : base(deviceName) {}
		}
		// ncrunch: no coverage end

		public bool IsAppInstalled(AndroidDevice device, string fullAppName)
		{
			ProcessRunner adbProcess = CreateAdbProcess("-s " + device.AdbId + " shell pm list packages");
			RunAdbProcess(adbProcess);
			return adbProcess.Output.Contains(fullAppName);
		}

		public void StartEngineBuiltApplication(AndroidDevice device, string fullAppName)
		{
			RunAdbProcess("-s " + device.AdbId + " shell am start -a android.intent.action.MAIN" +
				" -n " + fullAppName + "/.DeltaEngineActivity");
		}

		public string GetDeviceName(string adbDeviceId)
		{
			try
			{
				return TryGetDeviceName(adbDeviceId);
			}
			catch (ProcessRunner.ProcessTerminatedWithError)
			{
				throw new DeterminationDeviceNameFailed(adbDeviceId);
			}
		}

		private string TryGetDeviceName(string adbDeviceId)
		{
			// Reference:
			// http://stackoverflow.com/questions/6377444/can-i-use-adb-exe-to-find-a-description-of-a-phone
			string manufacturer = GetDeviceManufacturerName(adbDeviceId);
			string modelName = GetDeviceModelName(adbDeviceId);
			string fullDeviceName = manufacturer + " " + modelName;
			return fullDeviceName.Contains("not found")
				? "Device (AdbId=" + adbDeviceId + ")" : fullDeviceName;
		}

		private string GetDeviceManufacturerName(string adbDeviceId)
		{
			string manufacturerName = GetGrepInfo(adbDeviceId, "ro.product.manufacturer");
			return manufacturerName.IsFirstCharacterInLowerCase()
				? manufacturerName.ConvertFirstCharacterToUpperCase() : manufacturerName;
		}

		private string GetDeviceModelName(string adbDeviceId)
		{
			return GetGrepInfo(adbDeviceId, "ro.product.model");
		}

		private string GetGrepInfo(string adbDeviceId, string grepParameter)
		{
			ProcessRunner adbProcess = CreateAdbProcess("-s " + adbDeviceId +
				" shell cat /system/build.prop | grep \"" + grepParameter + "\"");
			RunAdbProcess(adbProcess);

			return adbProcess.Output.Replace(grepParameter + "=", "").Trim();
		}

		public class DeterminationDeviceNameFailed : Exception
		{
			public DeterminationDeviceNameFailed(string adbDeviceId) : base(adbDeviceId) { }
		}
	}
}