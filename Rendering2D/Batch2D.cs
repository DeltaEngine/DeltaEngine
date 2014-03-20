using System;
using DeltaEngine.Content;
using DeltaEngine.Core;
using DeltaEngine.Datatypes;
using DeltaEngine.Extensions;
using DeltaEngine.Graphics;
using DeltaEngine.Graphics.Vertices;
using DeltaEngine.ScreenSpaces;

namespace DeltaEngine.Rendering2D
{
	/// <summary>
	/// For rendering sprites in batches, use verticesUV, verticesColorUV or verticesColor to add
	/// 2D vertices data to be rendered and call AddIndices or build your own indices.
	/// </summary>
	public class Batch2D : Batch
	{
		public Batch2D(Material material, BlendMode blendMode, 
			int minimumNumberOfQuads = MinNumberOfQuads)
		{
			Material = material;
			BlendMode = blendMode;
			minimumNumberOfQuads = MathExtensions.Max(minimumNumberOfQuads, MinNumberOfQuads);
			hasUV = ((material.Shader.Flags & ShaderFlags.Textured) != 0);
			hasColor = ((material.Shader.Flags & ShaderFlags.Colored) != 0);
			indices = new short[minimumNumberOfQuads * IndicesPerQuad];
			if (!hasUV)
				verticesColor = new VertexPosition2DColor[minimumNumberOfQuads * VerticesPerQuad];
			else if (hasColor)
				verticesColorUV = new VertexPosition2DColorUV[minimumNumberOfQuads * VerticesPerQuad];
			else
				verticesUV = new VertexPosition2DUV[minimumNumberOfQuads * VerticesPerQuad];
		}

		public VertexPosition2DColor[] verticesColor;
		public VertexPosition2DColorUV[] verticesColorUV;
		public VertexPosition2DUV[] verticesUV;

		protected override void ResizeColorVertices(int newNumberOfVertices)
		{
			var newVerticesColor = new VertexPosition2DColor[newNumberOfVertices];
			Array.Copy(verticesColor, newVerticesColor, verticesColor.Length);
			verticesColor = newVerticesColor;
		}

		protected override void ResizeColorUVVertices(int newNumberOfVertices)
		{
			var newVerticesColorUV = new VertexPosition2DColorUV[newNumberOfVertices];
			Array.Copy(verticesColorUV, newVerticesColorUV, verticesColorUV.Length);
			verticesColorUV = newVerticesColorUV;
		}

		protected override void ResizeUVVertices(int newNumberOfVertices)
		{
			var newVerticesUV = new VertexPosition2DUV[newNumberOfVertices];
			Array.Copy(verticesUV, newVerticesUV, verticesUV.Length);
			verticesUV = newVerticesUV;
		}

		public void AddIndicesAndVertices(Sprite sprite)
		{
			if (!HasSomethingToRender(sprite))
				return; //ncrunch: no coverage
			AddIndices();
			AddVertices(sprite);
		}

		private bool HasSomethingToRender(Sprite sprite)
		{
			var data = sprite.Get<RenderingData>();
			if (data == null)
				return false; //ncrunch: no coverage, only happens with broken material data in the Editor
			drawArea = data.DrawArea;
			uv = data.AtlasUV;
			isAtlasRotated = data.IsAtlasRotated;
			screen = ScreenSpace.Current;
			rotation = sprite.Get<float>();
			return data.HasSomethingToRender;
		}

		private Rectangle drawArea;
		private Rectangle uv;
		private bool isAtlasRotated;
		private ScreenSpace screen;
		private float rotation;

		private void AddVertices(Sprite sprite)
		{
			if (isAtlasRotated && rotation == 0)
				AddVerticesAtlasRotated(sprite); //ncrunch: no coverage
			else if (isAtlasRotated)
				AddVerticesAtlasAndDrawAreaRotated(sprite, sprite.RotationCenter); //ncrunch: no coverage
			else if (rotation == 0)
				AddVerticesNotRotated(sprite);
			else
				AddVerticesRotated(sprite, sprite.RotationCenter);
		}

		//ncrunch: no coverage start
		private void AddVerticesAtlasRotated(Sprite sprite)
		{
			if (!hasUV)
			{
				var color = sprite.Color;
				verticesColor[verticesIndex++] = new VertexPosition2DColor(
					ScreenSpace.Current.ToPixelSpaceRounded(drawArea.TopLeft), color);
				verticesColor[verticesIndex++] = new VertexPosition2DColor(
					ScreenSpace.Current.ToPixelSpaceRounded(drawArea.BottomLeft), color);
				verticesColor[verticesIndex++] = new VertexPosition2DColor(
						ScreenSpace.Current.ToPixelSpaceRounded(drawArea.BottomRight), color);
				verticesColor[verticesIndex++] = new VertexPosition2DColor(
					ScreenSpace.Current.ToPixelSpaceRounded(drawArea.TopRight), color);
			}
			else if (hasColor)
			{
				var color = sprite.Color;
				verticesColorUV[verticesIndex++] = new VertexPosition2DColorUV(
					ScreenSpace.Current.ToPixelSpaceRounded(drawArea.TopLeft), color, uv.TopLeft);
				verticesColorUV[verticesIndex++] = new VertexPosition2DColorUV(
					ScreenSpace.Current.ToPixelSpaceRounded(drawArea.BottomLeft), color, uv.BottomLeft);
				verticesColorUV[verticesIndex++] = new VertexPosition2DColorUV(
					ScreenSpace.Current.ToPixelSpaceRounded(drawArea.BottomRight), color, uv.BottomRight);
				verticesColorUV[verticesIndex++] = new VertexPosition2DColorUV(
					ScreenSpace.Current.ToPixelSpaceRounded(drawArea.TopRight), color, uv.TopRight);
			}
			else
			{
				verticesUV[verticesIndex++] =
					new VertexPosition2DUV(screen.ToPixelSpaceRounded(drawArea.TopLeft), uv.TopLeft);
				verticesUV[verticesIndex++] =
					new VertexPosition2DUV(screen.ToPixelSpaceRounded(drawArea.BottomLeft), uv.BottomLeft);
				verticesUV[verticesIndex++] =
					new VertexPosition2DUV(screen.ToPixelSpaceRounded(drawArea.BottomRight), uv.BottomRight);
				verticesUV[verticesIndex++] =
					new VertexPosition2DUV(screen.ToPixelSpaceRounded(drawArea.TopRight), uv.TopRight);
			}
		}

		private void AddVerticesAtlasAndDrawAreaRotated(Sprite sprite, Vector2D rotationCenter)
		{
			float sin = MathExtensions.Sin(rotation);
			float cos = MathExtensions.Cos(rotation);
			if (!hasUV)
			{
				var color = sprite.Color;
				verticesColor[verticesIndex++] = new VertexPosition2DColor(
					ScreenSpace.Current.ToPixelSpaceRounded(
					drawArea.TopLeft.RotateAround(rotationCenter, sin, cos)), color);
				verticesColor[verticesIndex++] = new VertexPosition2DColor(
					ScreenSpace.Current.ToPixelSpaceRounded(
					drawArea.BottomLeft.RotateAround(rotationCenter, sin, cos)), color);
				verticesColor[verticesIndex++] = new VertexPosition2DColor(
						ScreenSpace.Current.ToPixelSpaceRounded(
						drawArea.BottomRight.RotateAround(rotationCenter, sin, cos)), color);
				verticesColor[verticesIndex++] = new VertexPosition2DColor(
					ScreenSpace.Current.ToPixelSpaceRounded(
					drawArea.TopRight.RotateAround(rotationCenter, sin, cos)), color);
			}
			else if (hasColor)
			{
				var color = sprite.Color;
				verticesColorUV[verticesIndex++] = new VertexPosition2DColorUV(
					ScreenSpace.Current.ToPixelSpaceRounded(
					drawArea.TopLeft.RotateAround(rotationCenter, sin, cos)), color, uv.TopLeft);
				verticesColorUV[verticesIndex++] = new VertexPosition2DColorUV(
					ScreenSpace.Current.ToPixelSpaceRounded(
					drawArea.BottomLeft.RotateAround(rotationCenter, sin, cos)), color, uv.BottomLeft);
				verticesColorUV[verticesIndex++] = new VertexPosition2DColorUV(
					ScreenSpace.Current.ToPixelSpaceRounded(
					drawArea.BottomRight.RotateAround(rotationCenter, sin, cos)), color, uv.BottomRight);
				verticesColorUV[verticesIndex++] = new VertexPosition2DColorUV(
					ScreenSpace.Current.ToPixelSpaceRounded(
					drawArea.TopRight.RotateAround(rotationCenter, sin, cos)), color, uv.TopRight);
			}
			else
			{
				verticesUV[verticesIndex++] = new VertexPosition2DUV(
					screen.ToPixelSpaceRounded(drawArea.TopLeft.RotateAround(rotationCenter, sin, cos)),
					uv.TopLeft);
				verticesUV[verticesIndex++] = new VertexPosition2DUV(
					screen.ToPixelSpaceRounded(drawArea.BottomLeft.RotateAround(rotationCenter, sin, cos)),
					uv.BottomLeft);
				verticesUV[verticesIndex++] = new VertexPosition2DUV(
					screen.ToPixelSpaceRounded(drawArea.BottomRight.RotateAround(rotationCenter, sin, cos)),
					uv.BottomRight);
				verticesUV[verticesIndex++] = new VertexPosition2DUV(
					screen.ToPixelSpaceRounded(drawArea.TopRight.RotateAround(rotationCenter, sin, cos)),
					uv.TopRight);
			}
		} //ncrunch: no coverage end

		private void AddVerticesNotRotated(Sprite sprite)
		{
			if (!hasUV)
			{
				var color = sprite.Color;
				verticesColor[verticesIndex++] = new VertexPosition2DColor(
					ScreenSpace.Current.ToPixelSpaceRounded(drawArea.TopLeft), color);
				verticesColor[verticesIndex++] = new VertexPosition2DColor(
					ScreenSpace.Current.ToPixelSpaceRounded(drawArea.BottomLeft), color);
				verticesColor[verticesIndex++] = new VertexPosition2DColor(
					ScreenSpace.Current.ToPixelSpaceRounded(drawArea.BottomRight), color);
				verticesColor[verticesIndex++] = new VertexPosition2DColor(
					ScreenSpace.Current.ToPixelSpaceRounded(drawArea.TopRight), color);
			}
			else if (hasColor)
			{
				var color = sprite.Color;
				verticesColorUV[verticesIndex++] = new VertexPosition2DColorUV(
					ScreenSpace.Current.ToPixelSpaceRounded(drawArea.TopLeft), color, uv.TopLeft);
				verticesColorUV[verticesIndex++] = new VertexPosition2DColorUV(
					ScreenSpace.Current.ToPixelSpaceRounded(drawArea.BottomLeft), color, uv.BottomLeft);
				verticesColorUV[verticesIndex++] = new VertexPosition2DColorUV(
					ScreenSpace.Current.ToPixelSpaceRounded(drawArea.BottomRight), color, uv.BottomRight);
				verticesColorUV[verticesIndex++] = new VertexPosition2DColorUV(
					ScreenSpace.Current.ToPixelSpaceRounded(drawArea.TopRight), color, uv.TopRight);
			}
			else
			{
				verticesUV[verticesIndex++] =
					new VertexPosition2DUV(screen.ToPixelSpaceRounded(drawArea.TopLeft), uv.TopLeft);
				verticesUV[verticesIndex++] =
					new VertexPosition2DUV(screen.ToPixelSpaceRounded(drawArea.BottomLeft), uv.BottomLeft);
				verticesUV[verticesIndex++] =
					new VertexPosition2DUV(screen.ToPixelSpaceRounded(drawArea.BottomRight), uv.BottomRight);
				verticesUV[verticesIndex++] =
					new VertexPosition2DUV(screen.ToPixelSpaceRounded(drawArea.TopRight), uv.TopRight);
			}
		}

		private void AddVerticesRotated(Sprite sprite, Vector2D rotationCenter)
		{
			float sin = MathExtensions.Sin(rotation);
			float cos = MathExtensions.Cos(rotation);
			if (!hasUV)
			{
				var color = sprite.Color;
				verticesColor[verticesIndex++] = new VertexPosition2DColor(
					ScreenSpace.Current.ToPixelSpaceRounded(
					drawArea.TopLeft.RotateAround(rotationCenter, sin, cos)), color);
				verticesColor[verticesIndex++] = new VertexPosition2DColor(
					ScreenSpace.Current.ToPixelSpaceRounded(
					drawArea.BottomLeft.RotateAround(rotationCenter, sin, cos)), color);
				verticesColor[verticesIndex++] = new VertexPosition2DColor(
						ScreenSpace.Current.ToPixelSpaceRounded(
						drawArea.BottomRight.RotateAround(rotationCenter, sin, cos)), color);
				verticesColor[verticesIndex++] = new VertexPosition2DColor(
					ScreenSpace.Current.ToPixelSpaceRounded(
					drawArea.TopRight.RotateAround(rotationCenter, sin, cos)), color);
			}
			else if (hasColor)
			{
				var color = sprite.Color;
				verticesColorUV[verticesIndex++] = new VertexPosition2DColorUV(
					ScreenSpace.Current.ToPixelSpaceRounded(
					drawArea.TopLeft.RotateAround(rotationCenter, sin, cos)), color, uv.TopLeft);
				verticesColorUV[verticesIndex++] = new VertexPosition2DColorUV(
					ScreenSpace.Current.ToPixelSpaceRounded(
					drawArea.BottomLeft.RotateAround(rotationCenter, sin, cos)), color, uv.BottomLeft);
				verticesColorUV[verticesIndex++] = new VertexPosition2DColorUV(
					ScreenSpace.Current.ToPixelSpaceRounded(
					drawArea.BottomRight.RotateAround(rotationCenter, sin, cos)), color, uv.BottomRight);
				verticesColorUV[verticesIndex++] = new VertexPosition2DColorUV(
					ScreenSpace.Current.ToPixelSpaceRounded(
					drawArea.TopRight.RotateAround(rotationCenter, sin, cos)), color, uv.TopRight);
			}
			else
			{
				verticesUV[verticesIndex++] = new VertexPosition2DUV(
					screen.ToPixelSpaceRounded(drawArea.TopLeft.RotateAround(rotationCenter, sin, cos)),
					uv.TopLeft);
				verticesUV[verticesIndex++] = new VertexPosition2DUV(
					screen.ToPixelSpaceRounded(drawArea.BottomLeft.RotateAround(rotationCenter, sin, cos)),
					uv.BottomLeft);
				verticesUV[verticesIndex++] = new VertexPosition2DUV(
					screen.ToPixelSpaceRounded(drawArea.BottomRight.RotateAround(rotationCenter, sin, cos)),
					uv.BottomRight);
				verticesUV[verticesIndex++] = new VertexPosition2DUV(
					screen.ToPixelSpaceRounded(drawArea.TopRight.RotateAround(rotationCenter, sin, cos)),
					uv.TopRight);
			}
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