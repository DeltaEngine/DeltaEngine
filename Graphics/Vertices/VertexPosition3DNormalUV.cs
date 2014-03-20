using System.Diagnostics;
using System.Runtime.InteropServices;
using DeltaEngine.Datatypes;

namespace DeltaEngine.Graphics.Vertices
{
	/// <summary>
	/// Vertex struct that describes 3D position, vertex color and texture coordinate.
	/// </summary>
	[DebuggerDisplay("VertexPosition3DNormalUV({Position}, {Normal}, {UV})")]
	[StructLayout(LayoutKind.Sequential)]
	public struct VertexPosition3DNormalUV : Lerp<VertexPosition3DNormalUV>, Vertex
	{
		public VertexPosition3DNormalUV(Vector3D position, Vector3D normal, Vector2D uv)
		{
			Position = position;
			Normal = normal;
			UV = uv;
		}

		public Vector3D Position;
		public Vector3D Normal;
		public Vector2D UV;

		public VertexPosition3DNormalUV(Vector2D position, Vector3D normal, Vector2D uv)
		{
			Position = new Vector3D(position.X, position.Y, 0.0f);
			Normal = normal;
			UV = uv;
		}

		public static readonly int SizeInBytes = VertexFormat.Position3DNormalUV.Stride;

		public VertexPosition3DNormalUV Lerp(VertexPosition3DNormalUV other, float interpolation)
		{
			return new VertexPosition3DNormalUV(Position.Lerp(other.Position, interpolation),
				Normal.Lerp(other.Normal, interpolation), UV);
		}

		public VertexFormat Format
		{
			get { return VertexFormat.Position3DNormalUV; }
		}
	}
}