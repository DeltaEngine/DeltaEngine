using System;
using System.Runtime.InteropServices;
using SysPoint = System.Drawing.Point;

namespace DeltaEngine.Input.Windows
{
	internal static class NativeMethods
	{
		internal delegate int HookProc(int code, IntPtr wParam, IntPtr lParam);

		[DllImport("User32.dll")]
		internal static extern uint GetRawInputDeviceList(IntPtr rawInputDeviceListHandle,
			ref uint uiNumDevices, int cbSize);

		[DllImport("User32.dll", CharSet = CharSet.Unicode)]
		internal static extern int GetRawInputDeviceInfo(IntPtr deviceHandle, int uiCommand,
			[In, Out, MarshalAs(UnmanagedType.AsAny)] Object data, ref int dataSize);

		[DllImport("user32.dll")]
		internal static extern bool ScreenToClient(IntPtr windowHandle, ref SysPoint position);

		[DllImport("user32.dll")]
		internal static extern bool ClientToScreen(IntPtr windowHandle, ref SysPoint position);

		[DllImport("User32.dll")]
		internal static extern bool SetCursorPos(int x, int y);

		[DllImport("user32.dll")]
		internal static extern bool GetCursorPos(ref SysPoint position);

		[DllImport("kernel32.dll")]
		internal static extern int GetCurrentThreadId();

		[DllImport("user32.dll")]
		internal static extern IntPtr SetWindowsHookEx(int code, HookProc func, IntPtr hInstance,
			int threadID);

		[DllImport("user32.dll")]
		internal static extern int UnhookWindowsHookEx(IntPtr hook);

		[DllImport("user32.dll")]
		internal static extern int CallNextHookEx(IntPtr hook, int code, IntPtr wParam, IntPtr lParam);

		[DllImport("User32")]
		internal static extern bool RegisterTouchWindow(IntPtr handle, UInt32 flags);

		[DllImport("User32")]
		internal static extern bool UnregisterTouchWindow(IntPtr handle);

		[DllImport("user32")]
		internal static extern void CloseTouchInputHandle(IntPtr lParam);

		[DllImport("user32")]
		internal static extern bool GetTouchInputInfo(IntPtr hTouchInput, int cInputs,
			[In, Out] NativeTouchInput[] pInputs, int cbSize);
	}
}
