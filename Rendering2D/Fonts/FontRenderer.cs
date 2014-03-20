using System.Collections.Generic;
using DeltaEngine.Core;
using DeltaEngine.Datatypes;
using DeltaEngine.Entities;
using DeltaEngine.Extensions;
using DeltaEngine.Graphics.Vertices;
using DeltaEngine.ScreenSpaces;

namespace DeltaEngine.Rendering2D.Fonts
{
	internal class FontRenderer : DrawBehavior
	{
		public FontRenderer(BatchRenderer2D renderer)
		{
			this.renderer = renderer;
		}

		private readonly BatchRenderer2D renderer;

		public void Draw(List<DrawableEntity> visibleEntities)
		{
			foreach (var entity in visibleEntities)
				AddVerticesToBatch((FontText)entity);
		}

		private void AddVerticesToBatch(FontText text)
		{
			glyphs = text.Get<GlyphDrawData[]>();
			var batch =
				(Batch2D)renderer.FindOrCreateBatch(text.CachedMaterial, BlendMode.Normal, glyphs.Length);
			drawArea = text.Get<Rectangle>();
			color = text.Get<Color>();
			size = text.Get<Size>();
			position = new Vector2D(GetHorizontalPosition(text), GetVerticalPosition(text));
			foreach (GlyphDrawData glyph in glyphs)
				AddIndicesAndVerticesForGlyph(batch, glyph);
		}

		private Rectangle drawArea;
		private Color color;
		private Vector2D position;
		private GlyphDrawData[] glyphs;
		private Size size;

		private float GetHorizontalPosition(FontText text)
		{
			var alignment = text.HorizontalAlignment;
			if (alignment == HorizontalAlignment.Left)
				return ScreenSpace.Current.ToPixelSpace(drawArea.TopLeft).X;
			if (alignment == HorizontalAlignment.Right)
				return (ScreenSpace.Current.ToPixelSpace(drawArea.TopRight).X - size.Width).Round();
			return
				(ScreenSpace.Current.ToPixelSpace(drawArea.Center).X - (size.Width / 2) +
				UnevenNumberOffset).Round();
		}

		private const float UnevenNumberOffset = 0.25f;

		private float GetVerticalPosition(FontText text)
		{
			var alignment = text.VerticalAlignment;
			if (alignment == VerticalAlignment.Top)
				return ScreenSpace.Current.ToPixelSpace(drawArea.TopLeft).Y;
			if (alignment == VerticalAlignment.Bottom)
				return (ScreenSpace.Current.ToPixelSpace(drawArea.BottomLeft).Y - size.Height).Round();
			return
				(ScreenSpace.Current.ToPixelSpace(drawArea.Center).Y - (size.Height / 2) +
				UnevenNumberOffset).Round();
		}

		private void AddIndicesAndVerticesForGlyph(Batch2D batch, GlyphDrawData glyph)
		{
			batch.AddIndices();
			batch.verticesColorUV[batch.verticesIndex++] = new VertexPosition2DColorUV(
				position + glyph.DrawArea.TopLeft, color, glyph.UV.TopLeft);
			batch.verticesColorUV[batch.verticesIndex++] = new VertexPosition2DColorUV(
				position + glyph.DrawArea.BottomLeft, color, glyph.UV.BottomLeft);
			batch.verticesColorUV[batch.verticesIndex++] = new VertexPosition2DColorUV(
				position + glyph.DrawArea.BottomRight, color, glyph.UV.BottomRight);
			batch.verticesColorUV[batch.verticesIndex++] = new VertexPosition2DColorUV(
				position + glyph.DrawArea.TopRight, color, glyph.UV.TopRight);
		}
	}
}