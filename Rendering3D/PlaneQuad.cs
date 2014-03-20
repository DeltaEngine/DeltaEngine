using DeltaEngine.Content;
using DeltaEngine.Datatypes;
using DeltaEngine.Graphics;
using DeltaEngine.Graphics.Vertices;

namespace DeltaEngine.Rendering3D
{
	/// <summary>
	/// Textured rectangular mesh
	/// </summary>
	public class PlaneQuad : Mesh
	{
		public PlaneQuad(Size size, Material material)
			: base(CreatePlaneGeometry(size, material), material)
		{
			this.size = size;
		}

		private Size size;

		private static Geometry CreatePlaneGeometry(Size size, Material material)
		{
			var shader = material.Shader as ShaderWithFormat;
			var creationData = new GeometryCreationData(shader.Format, 4, 6);
			var geometry = ContentLoader.Create<Geometry>(creationData);
			if (((shader.Flags & ShaderFlags.Colored) != 0))
				geometry.SetData(SetDataColored(size, shader, material), GetQuadIndices());
			else
				geometry.SetData(SetDataNoColor(size, shader, material), GetQuadIndices());
			return geometry;
		}

		private static Vertex[] SetDataColored(Size size, ShaderWithFormat shader, Material material)
		{
			if ((shader.Flags & ShaderFlags.Textured) != 0)
				return CreateVerticesColoredTextured(size, material.DefaultColor);
			return CreateVerticesColored(size, material.DefaultColor);

		}

		private static Vertex[] SetDataNoColor(Size size, ShaderWithFormat shader, Material material)
		{
				return CreateVerticesTextured(size);
		}

		private static Vertex[] CreateVerticesColoredTextured(Size size, Color color)
		{
			float xWidthHalf = size.Width / 2;
			float zWidthHalf = size.Height / 2;
			var vertices = new Vertex[]
			{
				new VertexPosition3DColorUV(new Vector3D(-xWidthHalf, 0, zWidthHalf), color, Vector2D.Zero),
				new VertexPosition3DColorUV(new Vector3D(xWidthHalf, 0, zWidthHalf), color, Vector2D.UnitX),
				new VertexPosition3DColorUV(new Vector3D(xWidthHalf, 0, -zWidthHalf), color, Vector2D.One),
				new VertexPosition3DColorUV(new Vector3D(-xWidthHalf, 0, -zWidthHalf), color, Vector2D.UnitY)
			};
			return vertices;
		}

		private static Vertex[] CreateVerticesTextured(Size size)
		{
			float xWidthHalf = size.Width / 2;
			float zWidthHalf = size.Height / 2;
			var vertices = new Vertex[]
			{
				new VertexPosition3DUV(new Vector3D(-xWidthHalf, 0, zWidthHalf), Vector2D.Zero),
				new VertexPosition3DUV(new Vector3D(xWidthHalf, 0, zWidthHalf), Vector2D.UnitX),
				new VertexPosition3DUV(new Vector3D(xWidthHalf, 0, -zWidthHalf), Vector2D.One),
				new VertexPosition3DUV(new Vector3D(-xWidthHalf, 0, -zWidthHalf), Vector2D.UnitY)
			};
			return vertices;
		}

		private static Vertex[] CreateVerticesColored(Size size, Color color)
		{
			float xWidthHalf = size.Width / 2;
			float zWidthHalf = size.Height / 2;
			var vertices = new Vertex[]
			{
				new VertexPosition3DColor(new Vector3D(-xWidthHalf, 0, zWidthHalf), color),
				new VertexPosition3DColor(new Vector3D(xWidthHalf, 0, zWidthHalf), color),
				new VertexPosition3DColor(new Vector3D(xWidthHalf, 0, -zWidthHalf), color),
				new VertexPosition3DColor(new Vector3D(-xWidthHalf, 0, -zWidthHalf), color)
			};
			return vertices;
		}

		private static short[] GetQuadIndices()
		{
			return new short[] { 0, 2, 1, 0, 3, 2 };
		}

		public Size Size
		{
			get { return size; }
			set
			{
				var shader = Material.Shader as ShaderWithFormat;
				size = value;
				var vertices = (shader.Flags | ShaderFlags.Colored) != 0
					? SetDataColored(size, shader, Material) : SetDataNoColor(size, shader, Material);
				Geometry.SetData(vertices, GetQuadIndices());
			}
		}
	}
}