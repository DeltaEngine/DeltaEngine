using System.Diagnostics;
using System.Runtime.InteropServices;
using DeltaEngine.Datatypes;

namespace DeltaEngine.Graphics.Vertices
{
	/// <summary>
	/// Simplest vertex format with just 2D positions and vertex colors (8 + 4 bytes).
	/// </summary>
	[DebuggerDisplay("VertexPosition2DColor({Position}, {Color})")]
	[StructLayout(LayoutKind.Sequential)]
	public struct VertexPosition2DColor : Lerp<VertexPosition2DColor>, Vertex
	{
		public VertexPosition2DColor(Vector2D position, Color color)
		{
			Position = position;
			Color = color;
		}

		public Vector2D Position;
		public Color Color;

		public static readonly int SizeInBytes = VertexFormat.Position2DColor.Stride;

		public VertexPosition2DColor Lerp(VertexPosition2DColor other, float interpolation)
		{
			return new VertexPosition2DColor(Position.Lerp(other.Position, interpolation),
				Color.Lerp(other.Color, interpolation));
		}

		public VertexFormat Format
		{
			get { return VertexFormat.Position2DColor; }
		}
	}
}