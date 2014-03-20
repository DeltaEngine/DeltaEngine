using System.Diagnostics;
using System.Runtime.InteropServices;
using DeltaEngine.Datatypes;

namespace DeltaEngine.Graphics.Vertices
{
	/// <summary>
	/// Vertex struct that describes 3D position and texture coordinate.
	/// </summary>
	[DebuggerDisplay("VertexPosition3DUV({Position}, {UV})")]
	[StructLayout(LayoutKind.Sequential)]
	public struct VertexPosition3DUV : Lerp<VertexPosition3DUV>, Vertex
	{
		public VertexPosition3DUV(Vector3D position, Vector2D uv)
		{
			Position = position;
			UV = uv;
		}

		public Vector3D Position;
		public Vector2D UV;

		public VertexPosition3DUV(Vector2D position, Vector2D uv)
		{
			Position = new Vector3D(position.X, position.Y, 0.0f);
			UV = uv;
		}

		public static readonly int SizeInBytes = VertexFormat.Position3DUV.Stride;

		public VertexPosition3DUV Lerp(VertexPosition3DUV other, float interpolation)
		{
			return new VertexPosition3DUV(Position.Lerp(other.Position, interpolation), UV);
		}

		public VertexFormat Format
		{
			get { return VertexFormat.Position3DUV; }
		}
	}
}