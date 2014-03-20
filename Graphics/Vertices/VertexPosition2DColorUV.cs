using System.Diagnostics;
using System.Runtime.InteropServices;
using DeltaEngine.Datatypes;

namespace DeltaEngine.Graphics.Vertices
{
	/// <summary>
	/// Vertex struct that describes 3D position, vertex color and texture coordinate.
	/// </summary>
	[DebuggerDisplay("VertexPosition2DColorUV({Position}, {Color}, {UV})")]
	[StructLayout(LayoutKind.Sequential)]
	public struct VertexPosition2DColorUV : Lerp<VertexPosition2DColorUV>, Vertex
	{
		public VertexPosition2DColorUV(Vector2D position, Color color, Vector2D uv)
		{
			Position = position;
			Color = color;
			UV = uv;
		}

		public Vector2D Position;
		public Color Color;
		public Vector2D UV;

		public static readonly int SizeInBytes = VertexFormat.Position2DColorUV.Stride;

		public VertexPosition2DColorUV Lerp(VertexPosition2DColorUV other, float interpolation)
		{
			return new VertexPosition2DColorUV(Position.Lerp(other.Position, interpolation),
				Color.Lerp(other.Color, interpolation), UV);
		}

		public VertexFormat Format
		{
			get { return VertexFormat.Position2DColorUV; }
		}
	}
}