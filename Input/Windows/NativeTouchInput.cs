using System;
using System.Runtime.InteropServices;

namespace DeltaEngine.Input.Windows
{
	/// <summary>
	/// Touch structure of the native windows touch events containing all state information.
	/// http://msdn.microsoft.com/en-us/library/windows/desktop/dd317334(v=vs.85).aspx
	/// </summary>
	[StructLayout(LayoutKind.Sequential)]
	internal struct NativeTouchInput
	{
		public NativeTouchInput(int flags, int id, int x, int y)
			: this()
		{
			Flags = flags;
			Id = id;
			X = x;
			Y = y;
		}

		public readonly int X;
		public readonly int Y;
		private readonly IntPtr sourcePointer;
		public readonly int Id;
		public readonly int Flags;
		private readonly int mask;
		private readonly int timeStamp;
		private readonly IntPtr extraInfo;
		private readonly int contactX;
		private readonly int contactY;

		public const int FlagTouchDown = 0x0001;
		public const int FlagTouchMove = 0x0002;
		public const int FlagTouchDownOrMoved = FlagTouchDown | FlagTouchMove;
	}
}