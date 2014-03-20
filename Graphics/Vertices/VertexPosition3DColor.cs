using System.Diagnostics;
using System.Runtime.InteropServices;
using DeltaEngine.Datatypes;

namespace DeltaEngine.Graphics.Vertices
{
	/// <summary>
	/// Simplest vertex format with just 3D positions and vertex colors (12 + 4 bytes).
	/// </summary>
	[DebuggerDisplay("VertexPosition3DColor({Position}, {Color})")]
	[StructLayout(LayoutKind.Sequential)]
	public struct VertexPosition3DColor : Lerp<VertexPosition3DColor>, Vertex
	{
		public VertexPosition3DColor(Vector3D position, Color color)
		{
			Position = position;
			Color = color;
		}

		public Vector3D Position;
		public Color Color;

		public VertexPosition3DColor(Vector2D position, Color color)
			: this(new Vector3D(position.X, position.Y, 0.0f), color) {}

		public static readonly int SizeInBytes = VertexFormat.Position3DColor.Stride;

		public VertexPosition3DColor Lerp(VertexPosition3DColor other, float interpolation)
		{
			return new VertexPosition3DColor(Position.Lerp(other.Position, interpolation),
				Color.Lerp(other.Color, interpolation));
		}

		public VertexFormat Format
		{
			get { return VertexFormat.Position3DColor; }
		}
	}
}