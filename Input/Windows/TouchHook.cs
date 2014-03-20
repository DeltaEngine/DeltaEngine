using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using DeltaEngine.Core;
using DeltaEngine.Platforms.Windows;

namespace DeltaEngine.Input.Windows
{
	/// <summary>
	/// Native hook on the windows messaging pipeline to grab touch input data.
	/// </summary>
	public class TouchHook : WindowsHook
	{
		public TouchHook(Window window)
			: base(WMTouch, null)
		{
			messageAction = HandleTouchMessage;
			nativeTouches = new List<NativeTouchInput>();
			windowHandle = window.Handle;
			NativeMethods.RegisterTouchWindow(windowHandle, 0);
			RegisterNativeTouchEvent(window);
		}

		internal readonly List<NativeTouchInput> nativeTouches;
		private readonly IntPtr windowHandle;

		private void RegisterNativeTouchEvent(Window window)
		{
			if (!(window is FormsWindow))
				return;
			//ncrunch: no coverage start. We couldn't test that with MockWindows
			var formsWindow = window as FormsWindow;
			formsWindow.NativeEvent +=
				(ref Message message) => messageAction(message.WParam, message.LParam, message.Msg);
		}

		private void HandleTouchMessage(IntPtr wParam, IntPtr lParam, int msg)
		{
			if (msg != WMTouch)
				return;
			int inputCount = wParam.ToInt32();
			IEnumerable<NativeTouchInput> newTouches = GetTouchDataFromHandle(inputCount, lParam);
			NativeMethods.CloseTouchInputHandle(lParam);
			if (newTouches != null)
				nativeTouches.AddRange(newTouches);
		} //ncrunch: no coverage end

		internal static IEnumerable<NativeTouchInput> GetTouchDataFromHandle(int inputCount,
			IntPtr handle)
		{
			var inputs = new NativeTouchInput[inputCount];
			bool isTouchProcessed = NativeMethods.GetTouchInputInfo(handle, inputCount, inputs,
				NativeTouchByteSize);
			return isTouchProcessed == false ? null : inputs;
		}

		private static readonly int NativeTouchByteSize = Marshal.SizeOf(typeof(NativeTouchInput));

		public override void Dispose()
		{
			if (windowHandle != IntPtr.Zero)
				NativeMethods.UnregisterTouchWindow(windowHandle); //ncrunch: no coverage. Is always zero
			base.Dispose();
		}
	}
}