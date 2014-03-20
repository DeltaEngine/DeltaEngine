using DeltaEngine.Datatypes;

namespace DeltaEngine.Input
{
	/// <summary>
	/// Touch input message for remote input via networking.
	/// </summary>
	public class TouchMessage
	{
		public TouchMessage(Vector2D[] positions, bool[] pressedTouches)
		{
			Positions = positions;
			PressedTouches = pressedTouches;
		}

		public Vector2D[] Positions { get; private set; }
		public bool[] PressedTouches { get; private set; }

		public const int MaxNumberOfTouches = 10;
	}
}