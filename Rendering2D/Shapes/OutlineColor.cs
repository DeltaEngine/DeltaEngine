using DeltaEngine.Datatypes;

namespace DeltaEngine.Rendering2D.Shapes
{
	/// <summary>
	/// Stores the color of the outline to a 2D shape.
	/// </summary>
	public class OutlineColor : Lerp<OutlineColor>
	{
		public OutlineColor(Color color)
		{
			Value = color;
		}

		public Color Value { get; set; }

		public OutlineColor Lerp(OutlineColor other, float interpolation)
		{
			return new OutlineColor(Value.Lerp(other.Value, interpolation));
		}
	}
}