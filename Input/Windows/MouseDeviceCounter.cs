using System;
using System.Runtime.InteropServices;

namespace DeltaEngine.Input.Windows
{
	/// <summary>
	/// Helper class to count the number of mouse devices connected to the computer.
	/// This makes it possible to check the availability of Mouse support.
	/// </summary>
	public class MouseDeviceCounter
	{
		//ncrunch: no coverage start
		public int GetNumberOfAvailableMice()
		{
			int numberOfMice = 0;
			uint deviceCount = GetDeviceCount();
			IntPtr rawInputDeviceListHandle = GetRawInputDeviceListHandle(deviceCount);
			for (int index = 0; index < deviceCount; index++)
				if (IsDeviceMouseDevice(index, rawInputDeviceListHandle))
					numberOfMice++;
			Marshal.FreeHGlobal(rawInputDeviceListHandle);
			return numberOfMice;
		}

		private readonly int deviceListSize = Marshal.SizeOf(typeof(RawInputDeviceList));

		private uint GetDeviceCount()
		{
			uint deviceCount = 0;
			NativeMethods.GetRawInputDeviceList(IntPtr.Zero, ref deviceCount, deviceListSize);
			return deviceCount;
		}

		private IntPtr GetRawInputDeviceListHandle(uint deviceCount)
		{
			IntPtr rawInputDeviceListHandle = Marshal.AllocHGlobal((int)deviceCount * deviceListSize);
			NativeMethods.GetRawInputDeviceList(rawInputDeviceListHandle, ref deviceCount, deviceListSize);
			return rawInputDeviceListHandle;
		}

		private static bool IsDeviceMouseDevice(int index, IntPtr rawInputDeviceListHandle)
		{
			Type deviceListType = typeof(RawInputDeviceList);
			int dataOffset = index * Marshal.SizeOf(deviceListType);
			var dataPointer = new IntPtr(rawInputDeviceListHandle.ToInt32() + dataOffset);
			var rawInputDevice = (RawInputDeviceList)Marshal.PtrToStructure(dataPointer, deviceListType);

			if (rawInputDevice.Type != RimTypeMouseId)
				return false;

			string name = GetDeviceName(rawInputDevice.DeviceHandle);
			return !IsNameWindowsTerminalRdp(name);
		}

		private const int RimTypeMouseId = 0;

		private static string GetDeviceName(IntPtr deviceHandle)
		{
			int nameSize = 0;
			NativeMethods.GetRawInputDeviceInfo(deviceHandle, RidiDevicenameId, IntPtr.Zero, 
				ref nameSize);
			var nameData = new char[nameSize];
			NativeMethods.GetRawInputDeviceInfo(deviceHandle, RidiDevicenameId, nameData, ref nameSize);
			return new string(nameData);
		}

		private const int RidiDevicenameId = 0x20000007;

		private static bool IsNameWindowsTerminalRdp(string name)
		{
			return name.Contains(@"?\Root#RDP_MOU#0000#");
		}

#pragma warning disable 649

		private struct RawInputDeviceList
		{
			public readonly IntPtr DeviceHandle;
			public readonly int Type;
		}
	}
}