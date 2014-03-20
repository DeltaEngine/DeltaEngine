using DeltaEngine.Content;
using DeltaEngine.Datatypes;
using DeltaEngine.Graphics;
using DeltaEngine.Graphics.Vertices;

namespace DeltaEngine.Rendering3D.Shapes
{
	/// <summary>
	/// Entity 3D representing a simple box.
	/// </summary>
	public class BoxMesh : Mesh
	{
		public BoxMesh(Vector3D size, Color color)
			: this(size, new Material(ShaderFlags.Colored, "") { DefaultColor = color }) {}

		public BoxMesh(Vector3D size, Material material)
			: base(CreateGeometry(size, material), material)
		{
			this.size = size;
		}

		private static Geometry CreateGeometry(Vector3D size, Material material)
		{
			var isColored = material.Shader.Flags == ShaderFlags.Colored;
			var format = isColored
				? VertexFormat.Position3DColor
				: material.Shader.Flags == ShaderFlags.LitTextured
					? VertexFormat.Position3DNormalUV : VertexFormat.Position3DUV;
			var creationData = new GeometryCreationData(format, isColored ? 8 : 24, 36);
			var geometry = ContentLoader.Create<Geometry>(creationData);
			ComputeVertices(size, material, geometry);
			return geometry;
		}

		private static void ComputeVertices(Vector3D size, Material material, Geometry geometry)
		{
			if (material.Shader.Flags == ShaderFlags.Colored)
				geometry.SetData(ComputeColorVertices(size, material.DefaultColor), ColorBoxIndices);
			else if (material.Shader.Flags == ShaderFlags.LitTextured)
				geometry.SetData(ComputeNormalUVVertices(size), UVBoxIndices);
			else
				geometry.SetData(ComputeUVVertices(size), UVBoxIndices);
		}

		private static Vertex[] ComputeColorVertices(Vector3D size, Color color)
		{
			float x1 = -size.X / 2.0f;
			float x2 = size.X / 2.0f;
			float y1 = -size.Y / 2.0f;
			float y2 = size.Y / 2.0f;
			float z1 = -size.Z / 2.0f;
			float z2 = size.Z / 2.0f;
			var vertices = new Vertex[]
			{
				new VertexPosition3DColor(new Vector3D(x1, y2, z1), color),
				new VertexPosition3DColor(new Vector3D(x2, y2, z1), color),
				new VertexPosition3DColor(new Vector3D(x1, y1, z1), color),
				new VertexPosition3DColor(new Vector3D(x2, y1, z1), color),
				new VertexPosition3DColor(new Vector3D(x2, y2, z2), color),
				new VertexPosition3DColor(new Vector3D(x1, y2, z2), color),
				new VertexPosition3DColor(new Vector3D(x2, y1, z2), color),
				new VertexPosition3DColor(new Vector3D(x1, y1, z2), color)
			};
			return vertices;
		}

		public static readonly short[] ColorBoxIndices =
		{
			0, 1, 2, 2, 1, 3,
			4, 5, 6, 6, 5, 7,
			5, 0, 7, 7, 0, 2,
			1, 4, 3, 3, 4, 6,
			5, 4, 0, 0, 4, 1,
			6, 7, 3, 3, 7, 2
		};

		private static Vertex[] ComputeNormalUVVertices(Vector3D size)
		{
			float x1 = -size.X / 2.0f;
			float x2 = size.X / 2.0f;
			float y1 = -size.Y / 2.0f;
			float y2 = size.Y / 2.0f;
			float z1 = -size.Z / 2.0f;
			float z2 = size.Z / 2.0f;
			var vertices = new Vertex[]
			{
				new VertexPosition3DNormalUV(new Vector3D(x1, y2, z1), -Vector3D.UnitZ, new Vector2D(0, 1)), 
				new VertexPosition3DNormalUV(new Vector3D(x2, y2, z1), -Vector3D.UnitZ, new Vector2D(1, 1)),
				new VertexPosition3DNormalUV(new Vector3D(x1, y1, z1), -Vector3D.UnitZ, new Vector2D(0, 0)),
				new VertexPosition3DNormalUV(new Vector3D(x2, y1, z1), -Vector3D.UnitZ, new Vector2D(1, 0)),

				new VertexPosition3DNormalUV(new Vector3D(x2, y2, z2), Vector3D.UnitZ, new Vector2D(1, 1)),
				new VertexPosition3DNormalUV(new Vector3D(x1, y2, z2), Vector3D.UnitZ, new Vector2D(0, 1)),
				new VertexPosition3DNormalUV(new Vector3D(x2, y1, z2), Vector3D.UnitZ, new Vector2D(1, 0)),
				new VertexPosition3DNormalUV(new Vector3D(x1, y1, z2), Vector3D.UnitZ, new Vector2D(0, 0)),

				new VertexPosition3DNormalUV(new Vector3D(x1, y2, z2), Vector3D.UnitY, new Vector2D(1, 0)),
				new VertexPosition3DNormalUV(new Vector3D(x2, y2, z2), Vector3D.UnitY, new Vector2D(0, 0)),
				new VertexPosition3DNormalUV(new Vector3D(x1, y2, z1), Vector3D.UnitY, new Vector2D(1, 1)),
				new VertexPosition3DNormalUV(new Vector3D(x2, y2, z1), Vector3D.UnitY, new Vector2D(0, 1)),

				new VertexPosition3DNormalUV(new Vector3D(x2, y1, z2), -Vector3D.UnitY, new Vector2D(1, 0)),
				new VertexPosition3DNormalUV(new Vector3D(x1, y1, z2), -Vector3D.UnitY, new Vector2D(0, 0)),
				new VertexPosition3DNormalUV(new Vector3D(x2, y1, z1), -Vector3D.UnitY, new Vector2D(1, 1)),
				new VertexPosition3DNormalUV(new Vector3D(x1, y1, z1), -Vector3D.UnitY, new Vector2D(0, 1)),

				new VertexPosition3DNormalUV(new Vector3D(x1, y1, z2), -Vector3D.UnitX, new Vector2D(1, 0)),
				new VertexPosition3DNormalUV(new Vector3D(x1, y2, z2), -Vector3D.UnitX, new Vector2D(0, 0)),
				new VertexPosition3DNormalUV(new Vector3D(x1, y1, z1), -Vector3D.UnitX, new Vector2D(1, 1)),
				new VertexPosition3DNormalUV(new Vector3D(x1, y2, z1), -Vector3D.UnitX, new Vector2D(0, 1)),

				new VertexPosition3DNormalUV(new Vector3D(x2, y2, z2), Vector3D.UnitX, new Vector2D(1, 0)),
				new VertexPosition3DNormalUV(new Vector3D(x2, y1, z2), Vector3D.UnitX, new Vector2D(0, 0)),
				new VertexPosition3DNormalUV(new Vector3D(x2, y2, z1), Vector3D.UnitX, new Vector2D(1, 1)),
				new VertexPosition3DNormalUV(new Vector3D(x2, y1, z1), Vector3D.UnitX, new Vector2D(0, 1))
			};
			return vertices;
		}

		private static Vertex[] ComputeUVVertices(Vector3D size)
		{
			float x1 = -size.X / 2.0f;
			float x2 = size.X / 2.0f;
			float y1 = -size.Y / 2.0f;
			float y2 = size.Y / 2.0f;
			float z1 = -size.Z / 2.0f;
			float z2 = size.Z / 2.0f;
			var vertices = new Vertex[]
			{
				new VertexPosition3DUV(new Vector3D(x1, y2, z1), new Vector2D(0, 1)), 
				new VertexPosition3DUV(new Vector3D(x2, y2, z1), new Vector2D(1, 1)),
				new VertexPosition3DUV(new Vector3D(x1, y1, z1), new Vector2D(0, 0)),
				new VertexPosition3DUV(new Vector3D(x2, y1, z1), new Vector2D(1, 0)),

				new VertexPosition3DUV(new Vector3D(x2, y2, z2), new Vector2D(1, 1)),
				new VertexPosition3DUV(new Vector3D(x1, y2, z2), new Vector2D(0, 1)),
				new VertexPosition3DUV(new Vector3D(x2, y1, z2), new Vector2D(1, 0)),
				new VertexPosition3DUV(new Vector3D(x1, y1, z2), new Vector2D(0, 0)),

				new VertexPosition3DUV(new Vector3D(x1, y2, z2), new Vector2D(1, 0)),
				new VertexPosition3DUV(new Vector3D(x2, y2, z2), new Vector2D(0, 0)),
				new VertexPosition3DUV(new Vector3D(x1, y2, z1), new Vector2D(1, 1)),
				new VertexPosition3DUV(new Vector3D(x2, y2, z1), new Vector2D(0, 1)),

				new VertexPosition3DUV(new Vector3D(x2, y1, z2), new Vector2D(1, 0)),
				new VertexPosition3DUV(new Vector3D(x1, y1, z2), new Vector2D(0, 0)),
				new VertexPosition3DUV(new Vector3D(x2, y1, z1), new Vector2D(1, 1)),
				new VertexPosition3DUV(new Vector3D(x1, y1, z1), new Vector2D(0, 1)),

				new VertexPosition3DUV(new Vector3D(x1, y1, z2), new Vector2D(1, 0)),
				new VertexPosition3DUV(new Vector3D(x1, y2, z2), new Vector2D(0, 0)),
				new VertexPosition3DUV(new Vector3D(x1, y1, z1), new Vector2D(1, 1)),
				new VertexPosition3DUV(new Vector3D(x1, y2, z1), new Vector2D(0, 1)),

				new VertexPosition3DUV(new Vector3D(x2, y2, z2), new Vector2D(1, 0)),
				new VertexPosition3DUV(new Vector3D(x2, y1, z2), new Vector2D(0, 0)),
				new VertexPosition3DUV(new Vector3D(x2, y2, z1), new Vector2D(1, 1)),
				new VertexPosition3DUV(new Vector3D(x2, y1, z1), new Vector2D(0, 1))
			};
			return vertices;
		}

		public static readonly short[] UVBoxIndices =
		{
			0+0, 0+1, 0+2, 0+2, 0+1, 0+3,
			4+0, 4+1, 4+2, 4+2, 4+1, 4+3,
			8+0, 8+1, 8+2, 8+2, 8+1, 8+3,
			12+0, 12+1, 12+2, 12+2, 12+1, 12+3,
			16+0, 16+1, 16+2, 16+2, 16+1, 16+3,
			20+0, 20+1, 20+2, 20+2, 20+1, 20+3,
		};

		public Vector3D Size
		{
			get { return size; }
			set
			{
				size = value;
				ComputeVertices(size, Material, Geometry);
			}
		}
		private Vector3D size;

		public Color Color
		{
			get { return Material.DefaultColor; }
			set
			{
				Material.DefaultColor = value;
				ComputeVertices(size, Material, Geometry);
			}
		}
	}
}