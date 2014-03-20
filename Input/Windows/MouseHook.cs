using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using DeltaEngine.Extensions;

namespace DeltaEngine.Input.Windows
{
	/// <summary>
	/// Native hook on the windows messaging pipeline to grab mouse input data.
	/// </summary>
	internal class MouseHook : WindowsHook
	{
		internal MouseHook()
			: base(MouseHookId, null)
		{
			messageAction = HandleMouseMessage;
		}

		public int ScrollWheelValue { get; private set; }

		internal State ProcessButtonQueue(State previousState, MouseButton button)
		{
			if (previousState == State.Released && currentlyPressed[(int)button] == false &&
				wasReleasedThisFrame[(int)button])
				return State.Pressing; //ncrunch: no coverage
			wasReleasedThisFrame[(int)button] = false;
			return previousState.UpdateOnNativePressing(currentlyPressed[(int)button]);
		}

		private readonly bool[] currentlyPressed = new bool[MouseButton.Left.GetCount()];
		private readonly bool[] wasReleasedThisFrame = new bool[MouseButton.Left.GetCount()];

		private void HandleMouseMessage(IntPtr wParam, IntPtr lParam, int msg) //ncrunch: no coverage start
		{
			var data = new int[6];
			Marshal.Copy(lParam, data, 0, 6);
			UpdateMouseButtonsAndWheel(wParam.ToInt32(), data[5]);
		}

		private void UpdateMouseButtonsAndWheel(int intParam, int mouseData)
		{
			if (intParam == WMMouseWheel)
				ScrollWheelValue += mouseData / MouseWheelDivider;
			else
				UpdateMouseButton(intParam, mouseData);
		}

		internal const int WMMouseWheel = 0x020A;
		/// <summary>
		/// Values are multiples of 120 and only the high order word is used (shift one word = 16 bit).
		/// http://msdn.microsoft.com/en-us/library/windows/desktop/ms645617(v=vs.85).aspx
		/// </summary>
		private const int MouseWheelDivider = 120 << 16;

		private void UpdateMouseButton(int intParam, int mouseData)
		{
			bool isPressed = IsPressed(intParam);
			MouseButton button = GetMessageButton(intParam, mouseData);
			currentlyPressed[(int)button] = isPressed;
			if (isPressed == false)
				wasReleasedThisFrame[(int)button] = true;
		} //ncrunch: no coverage end

		internal static bool IsPressed(int wParam)
		{
			return IsAnyId(wParam, DownButtonIds);
		}

		internal static bool IsReleased(int wParam)
		{
			return IsAnyId(wParam, UpButtonIds);
		}

		internal static MouseButton GetMessageButton(int intParam, int mouseData)
		{
			if (IsAnyId(intParam, LeftButtonIds))
				return MouseButton.Left;
			if (IsAnyId(intParam, RightButtonIds))
				return MouseButton.Right;
			if (IsAnyId(intParam, MiddleButtonIds))
				return MouseButton.Middle;

			return mouseData == 65536 ? MouseButton.X1 : MouseButton.X2;
		}

		private static bool IsAnyId(int value, IEnumerable<int> ids)
		{
			return ids.Any(id => id == value);
		}

		private static readonly int[] LeftButtonIds =
		{
			0x0201, 0x0202, 0x0203, 0x00A1, 0x00A2, 0x00A3
		};

		private static readonly int[] RightButtonIds =
		{
			0x0204, 0x0205, 0x0206, 0x00A4, 0x00A5, 0x00A6
		};

		private static readonly int[] MiddleButtonIds =
		{
			0x0207, 0x0208, 0x0209, 0x00A7, 0x00A8, 0x00A9
		};

		private static readonly int[] DownButtonIds =
		{
			0x0201, 0x0203, 0x0204, 0x0206, 0x0207, 0x0209, 0x020B, 0x00A1, 0x00A3, 0x00A4, 0x00A6,
			0x00A7, 0x00A9, 0x00AB
		};

		private static readonly int[] UpButtonIds =
		{
			0x0202, 0x00A2, 0x0205, 0x00A5, 0x0208, 0x00A8, 0x020C, 0x00AC
		};
	}
}