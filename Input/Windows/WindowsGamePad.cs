using System;
using System.Runtime.InteropServices;
using DeltaEngine.Datatypes;
using DeltaEngine.Extensions;

namespace DeltaEngine.Input.Windows
{
	/// <summary>
	/// Native Windows implementation of the GamePad interface.
	/// http://www.activevb.de/cgi-bin/apiwiki/JOYINFOEX
	/// Not all features of the Xbox GamePad are available.
	/// </summary>
	public class WindowsGamePad : GamePad
	{
		public WindowsGamePad()
			: this(GamePadNumber.Any) {}

		public WindowsGamePad(GamePadNumber number)
			: base(number)
		{
			joyCapsSize = Marshal.SizeOf(typeof(JoyCaps));
			states = new State[GamePadButton.A.GetCount()];
			for (int i = 0; i < states.Length; i++)
				states[i] = State.Released;
		}

		private readonly int joyCapsSize;
		private readonly State[] states;

		public override void Vibrate(float strength) {} // ncrunch: no coverage

		private float triggerLeft;
		private float triggerRight;
		private float xAxisLeft;
		private float yAxisLeft;
		private float xAxisRight;
		private float yAxisRight;

		private void UpdateAllButtons(JoystickButtons buttons)
		{
			UpdateNormalButtons(buttons);
			UpdateStickAndShoulderButtons(buttons);
		}

		private void UpdateNormalButtons(JoystickButtons buttons)
		{
			UpdateButton(IsButtonPressed(buttons, JoystickButtons.A), GamePadButton.A);
			UpdateButton(IsButtonPressed(buttons, JoystickButtons.B), GamePadButton.B);
			UpdateButton(IsButtonPressed(buttons, JoystickButtons.X), GamePadButton.X);
			UpdateButton(IsButtonPressed(buttons, JoystickButtons.Y), GamePadButton.Y);
			UpdateButton(IsButtonPressed(buttons, JoystickButtons.Back), GamePadButton.Back);
			UpdateButton(IsButtonPressed(buttons, JoystickButtons.Start), GamePadButton.Start);
		}

		private static bool IsButtonPressed(JoystickButtons buttons, JoystickButtons button)
		{
			return (buttons | button) == buttons;
		}

		private void UpdateStickAndShoulderButtons(JoystickButtons buttons)
		{
			UpdateButton(IsButtonPressed(buttons, JoystickButtons.ShoulderLeft),
				GamePadButton.LeftShoulder);
			UpdateButton(IsButtonPressed(buttons, JoystickButtons.ThumbStickLeft),
				GamePadButton.LeftStick);
			UpdateButton(IsButtonPressed(buttons, JoystickButtons.ShoulderRight),
				GamePadButton.RightShoulder);
			UpdateButton(IsButtonPressed(buttons, JoystickButtons.ThumbStickRight),
				GamePadButton.RightStick);
		}

		private void UpdateButton(bool pressed, GamePadButton button)
		{
			var buttonIndex = (int)button;
			states[buttonIndex] = states[buttonIndex].UpdateOnNativePressing(pressed);
		}

		private void UpdateDPadButtons(uint pov)
		{
			UpdateButton(pov == 22500 || pov == 18000 || pov == 13500, GamePadButton.Down);
			UpdateButton(pov == 0 || pov == 31500 || pov == 4500, GamePadButton.Up);
			UpdateButton(pov == 31500 || pov == 27000 || pov == 22500, GamePadButton.Left);
			UpdateButton(pov == 4500 || pov == 9000 || pov == 13500, GamePadButton.Right);
		}

		public override void Dispose() {}
		public override bool IsAvailable
		{
			get { return GetPresence(GetJoystickByNumber()); }
			protected set { } //ncrunch: no coverage (senselesss regarding what "get" does)
		}

		private bool GetPresence(uint index)
		{
			JoyCaps caps;
			return joyGetDevCaps(index, out caps, joyCapsSize) == 0;
		}

		private uint GetJoystickByNumber()
		{
			if (Number == GamePadNumber.Any)
				return GetAnyJoystick();
			if (Number == GamePadNumber.Two)
				return 1;
			if (Number == GamePadNumber.Three)
				return 2;
			return Number == GamePadNumber.Four ? 3u : 0;
		}

		private uint GetAnyJoystick()
		{
			if (GetPresence(1))
				return 1;		//ncrunch: no coverage
			if (GetPresence(2))
				return 2;		//ncrunch: no coverage
			return GetPresence(3) ? 3u : 0;
		}

		public override Vector2D GetLeftThumbStick()
		{
			return new Vector2D(xAxisLeft, yAxisLeft);
		}

		public override Vector2D GetRightThumbStick()
		{
			return new Vector2D(xAxisRight, yAxisRight);
		}

		public override float GetLeftTrigger()
		{
			return triggerLeft;
		}

		public override float GetRightTrigger()
		{
			return triggerRight;
		}

		public override State GetButtonState(GamePadButton button)
		{
			return states[(int)button];
		}

		[StructLayout(LayoutKind.Sequential)]
		private struct JoyInfoEx
		{
			public uint Size;
			public uint Flags;
			public readonly uint Xpos;
			public readonly uint Ypos;
			public readonly uint Zpos;
			public readonly uint Rpos;
			public readonly uint Upos;
			private readonly uint Vpos;
			public readonly uint Buttons;
			private readonly uint ButtonNumber;
			public readonly uint Pov;
			private readonly uint reserved1;
			private readonly uint reserved2;
		}

		[StructLayout(LayoutKind.Sequential)]
		private struct JoyCaps
		{
			private readonly UInt16 wMid;
			private readonly UInt16 wPid;
			[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
			private readonly string szPname;
			public readonly Int32 wXmin;
			public readonly Int32 wXmax;
			public readonly Int32 wYmin;
			public readonly Int32 wYmax;
			private readonly Int32 wZmin;
			private readonly Int32 wZmax;
			private readonly Int32 wNumButtons;
			private readonly Int32 wPeriodMin;
			private readonly Int32 wPeriodMax;
			public readonly Int32 wRmin;
			public readonly Int32 wRmax;
			public readonly Int32 wUmin;
			public readonly Int32 wUmax;
			private readonly Int32 wVmin;
			private readonly Int32 wVmax;
			private readonly Int32 wCaps;
			private readonly Int32 wMaxAxes;
			private readonly Int32 wNumAxes;
			private readonly Int32 wMaxButtons;
			[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
			private readonly string szRegKey;
			[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
			private readonly string szOEMVxD;
		}

		[DllImport(WinmmLib, EntryPoint = "joyGetDevCaps")]
		private static extern uint joyGetDevCaps(uint id, out JoyCaps caps, int cbjc);

		[DllImport(WinmmLib, EntryPoint = "joyGetPosEx")]
		private static extern unsafe uint joyGetPosEx(uint id, JoyInfoEx* info);

		private const string WinmmLib = "Winmm.dll";

		[Flags]
		private enum JoystickButtons : uint
		{
			A = 1,
			B = 2,
			X = 4,
			Y = 8,
			ShoulderLeft = 16,
			ShoulderRight = 32,
			Back = 64,
			Start = 128,
			ThumbStickLeft = 256,
			ThumbStickRight = 512,
		}

		[Flags]
		private enum JoystickDwFlags : uint
		{
			JoyX = 0x1,
			JoyY = 0x2,
			JoyZ = 0x4,
			JoyR = 0x8,
			JoyU = 0x10,
			JoyV = 0x20,
			JoyPov = 0x40,
			JoyButtons = 0x80,
			JoyAll = JoyX | JoyY | JoyZ | JoyR | JoyU | JoyV | JoyPov | JoyButtons
		}

		protected unsafe override void UpdateGamePadStates()
		{
			uint index = GetJoystickByNumber();
			JoyCaps caps;
			joyGetDevCaps(index, out caps, joyCapsSize);
			var info = new JoyInfoEx
			{
				Size = (uint)Marshal.SizeOf(typeof(JoyInfoEx)),
				Flags = (uint)JoystickDwFlags.JoyAll
			};
			joyGetPosEx(index, &info);
			UpdateAllButtons((JoystickButtons)info.Buttons);
			UpdateDPadButtons(info.Pov);
			triggerLeft = info.Zpos == 65407 ? 1 : 0;
			triggerRight = info.Zpos == 127 ? 1 : 0;
			xAxisLeft = ((info.Xpos - caps.wXmin) * (2f / (caps.wXmax - caps.wXmin))) - 1f;
			yAxisLeft = ((info.Ypos - caps.wYmin) * (2f / (caps.wYmax - caps.wYmin))) - 1f;
			xAxisRight = ((info.Upos - caps.wUmin) * (2f / (caps.wUmax - caps.wUmin))) - 1f;
			yAxisRight = ((info.Rpos - caps.wRmin) * (2f / (caps.wRmax - caps.wRmin))) - 1f;			
		}
	}
}