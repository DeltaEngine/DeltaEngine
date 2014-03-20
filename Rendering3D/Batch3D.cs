using System;
using DeltaEngine.Content;
using DeltaEngine.Core;
using DeltaEngine.Extensions;
using DeltaEngine.Graphics;
using DeltaEngine.Graphics.Vertices;
using DeltaEngine.Rendering2D;

namespace DeltaEngine.Rendering3D
{
	/// <summary>
	/// For ordered geometry into batches by Shader and BlendMode, allows for more efficient rendering.
	/// </summary>
	public class Batch3D : Batch
	{
		public Batch3D(Material material, BlendMode blendMode,
			int minimumNumberOfQuads = MinNumberOfQuads)
		{
			Material = material;
			BlendMode = blendMode;
			minimumNumberOfQuads = MathExtensions.Max(minimumNumberOfQuads, MinNumberOfQuads);
			hasUV = ((material.Shader.Flags & ShaderFlags.Textured) != 0);
			hasColor = ((material.Shader.Flags & ShaderFlags.Colored) != 0);
			indices = new short[minimumNumberOfQuads * IndicesPerQuad];
			if (!hasUV)
				verticesColor = new VertexPosition3DColor[minimumNumberOfQuads * VerticesPerQuad]; //ncrunch: no coverage
			else if (hasColor)
				verticesColorUV = new VertexPosition3DColorUV[minimumNumberOfQuads * VerticesPerQuad];
			else
				verticesUV = new VertexPosition3DUV[minimumNumberOfQuads * VerticesPerQuad]; //ncrunch: no coverage
		}

		private VertexPosition3DColor[] verticesColor;
		public VertexPosition3DColorUV[] verticesColorUV;
		private VertexPosition3DUV[] verticesUV;
		//ncrunch: no coverage start
		protected override void ResizeColorVertices(int newNumberOfVertices)
		{
			var newVerticesColor = new VertexPosition3DColor[newNumberOfVertices];
			Array.Copy(verticesColor, newVerticesColor, verticesColor.Length);
			verticesColor = newVerticesColor;
		}

		protected override void ResizeColorUVVertices(int newNumberOfVertices)
		{
			var newVerticesColorUV = new VertexPosition3DColorUV[newNumberOfVertices];
			Array.Copy(verticesColorUV, newVerticesColorUV, verticesColorUV.Length);
			verticesColorUV = newVerticesColorUV;
		}

		protected override void ResizeUVVertices(int newNumberOfVertices)
		{
			var newVerticesUV = new VertexPosition3DUV[newNumberOfVertices];
			Array.Copy(verticesUV, newVerticesUV, verticesUV.Length);
			verticesUV = newVerticesUV;
		}
		
		public override void Draw(Drawing drawing)
		{
			if (indicesIndex == 0)
				return;
			if (verticesUV != null)
				drawing.Add(Material, BlendMode, verticesUV, indices, verticesIndex, indicesIndex);
			else if (verticesColorUV != null)
				drawing.Add(Material, BlendMode, verticesColorUV, indices, verticesIndex, indicesIndex);
			else if (verticesColor != null)
				drawing.Add(Material, BlendMode, verticesColor, indices, verticesIndex, indicesIndex);
		}
	}
}