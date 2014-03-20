namespace DeltaEngine.Input
{
	/// <summary>0
	/// GamePads feature lots of buttons, but most commonly A, B, X, Y are used for Triggers.
	/// </summary>
	public enum GamePadButton
	{
		/// <summary>
		/// Green A Button on the Xbox 360 controller, Blue Cross (X) on the PS3 controller.
		/// </summary>
		A,

		/// <summary>
		/// Red B Button on the Xbox 360 controller, Orange Circle (O) on the PS3 controller.
		/// </summary>
		B,

		/// <summary>
		/// Blue X Button on the Xbox 360 controller, Pink Square ([]) on the PS3 controller.
		/// </summary>
		X,

		/// <summary>
		/// Yellow Y Button on the Xbox 360 controller, Green Triangle (/\) the PS3 controller.
		/// </summary>
		Y,

		/// <summary>
		/// Left Button on the D-Pad (Digital Pad), the same for Xbox 360 and PS3
		/// </summary>
		Left,

		/// <summary>
		/// Right Button on the D-Pad (Digital Pad), the same for Xbox 360 and PS3
		/// </summary>
		Right,

		/// <summary>
		/// Up Button on the D-Pad (Digital Pad), the same for Xbox 360 and PS3
		/// </summary>
		Up,

		/// <summary>
		/// Down Button on the D-Pad (Digital Pad), the same for Xbox 360 and PS3
		/// </summary>
		Down,

		/// <summary>
		/// Left thumb stick, which is usually used for movement, but can also be pressed.
		/// </summary>
		LeftStick,

		/// <summary>
		/// right thumb stick, which is used often for looking around, but can also be pressed.
		/// </summary>
		RightStick,

		/// <summary>
		/// Start Button on the Xbox 360 and PS3 controller (to start the game or level).
		/// </summary>
		Start,

		/// <summary>
		/// Back Button on the Xbox 360 controller, Select Button on the PS3 controller.
		/// </summary>
		Back,

		/// <summary>
		/// Left Shoulder button on the Xbox 360 controller, L1 Button on the PS3.
		/// </summary>
		LeftShoulder,

		/// <summary>
		/// Right Shoulder button on the Xbox 360 controller, R1 Button on the PS3.
		/// </summary>
		RightShoulder,

		/// <summary>
		/// Big Button if supported on a Xbox 360 controller. Not supported on a standard controller.
		/// </summary>
		BigButton,
	}
}