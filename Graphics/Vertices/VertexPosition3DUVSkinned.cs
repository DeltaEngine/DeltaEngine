using System.Diagnostics;
using System.Runtime.InteropServices;
using DeltaEngine.Datatypes;

namespace DeltaEngine.Graphics.Vertices
{
	/// <summary>
	/// Vertex format with 3D position, texture UV and vertex skinning data (12 + 8 + 8 + 8 bytes).
	/// </summary>
	[DebuggerDisplay("VertexPosition3DUVSkinned({Position}, {UV}, {SkinningData})")]
	[StructLayout(LayoutKind.Sequential)]
	public struct VertexPosition3DUVSkinned : Lerp<VertexPosition3DUVSkinned>, Vertex
	{
		public VertexPosition3DUVSkinned(Vector3D position, Vector2D uv, SkinningData skinning)
		{
			Position = position;
			UV = uv;
			Skinning = skinning;
		}

		public Vector3D Position;
		public Vector2D UV;
		public SkinningData Skinning;

		public static readonly int SizeInBytes = VertexFormat.Position3DColorSkinned.Stride;

		public VertexPosition3DUVSkinned Lerp(VertexPosition3DUVSkinned other, float interpolation)
		{
			return new VertexPosition3DUVSkinned(Position.Lerp(other.Position, interpolation),
				UV.Lerp(other.UV, interpolation), Skinning.Lerp(other.Skinning, interpolation));
		}

		public VertexFormat Format
		{
			get { return VertexFormat.Position3DUVSkinned; }
		}
	}
}