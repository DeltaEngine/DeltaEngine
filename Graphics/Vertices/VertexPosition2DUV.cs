using System.Diagnostics;
using System.Runtime.InteropServices;
using DeltaEngine.Datatypes;

namespace DeltaEngine.Graphics.Vertices
{
	/// <summary>
	/// Vertex struct that describes 2D position and texture coordinate.
	/// </summary>
	[DebuggerDisplay("VertexPosition2DUV({Position}, {UV})")]
	[StructLayout(LayoutKind.Sequential)]
	public struct VertexPosition2DUV : Lerp<VertexPosition2DUV>, Vertex
	{
		public VertexPosition2DUV(Vector2D position, Vector2D uv)
		{
			Position = position;
			UV = uv;
		}

		public Vector2D Position;
		public Vector2D UV;

		public static readonly int SizeInBytes = VertexFormat.Position2DUV.Stride;

		public VertexPosition2DUV Lerp(VertexPosition2DUV other, float interpolation)
		{
			return new VertexPosition2DUV(Position.Lerp(other.Position, interpolation), UV);
		}

		public VertexFormat Format
		{
			get { return VertexFormat.Position2DUV; }
		}
	}
}