using System.Collections.Generic;
using System.Linq;
using DeltaEngine.Content;
using DeltaEngine.Entities;
using DeltaEngine.Graphics;
using DeltaEngine.Graphics.Vertices;
using DeltaEngine.ScreenSpaces;

namespace DeltaEngine.Rendering2D.Shapes
{
	public class GradientRectRenderer : DrawBehavior
	{
		public GradientRectRenderer(Drawing draw)
		{
			this.draw = draw;
			material = new Material(ShaderFlags.Position2DColored, "");
		}

		private readonly Drawing draw;
		private readonly Material material;

		private void AddToBatch(GradientFilledRect entity)
		{
			var startColor = entity.Color;
			var finalColor = entity.FinalColor;
			vertices[0] = new VertexPosition2DColor(
				ScreenSpace.Current.ToPixelSpaceRounded(entity.Points[0]), startColor);
			vertices[1] = new VertexPosition2DColor(
				ScreenSpace.Current.ToPixelSpaceRounded(entity.Points[1]), finalColor);
			vertices[2] = new VertexPosition2DColor(
				ScreenSpace.Current.ToPixelSpaceRounded(entity.Points[2]), finalColor);
			vertices[3] = new VertexPosition2DColor(
				ScreenSpace.Current.ToPixelSpaceRounded(entity.Points[3]), startColor);
			draw.Add(material, vertices, Indices);
		}

		private readonly VertexPosition2DColor[] vertices = new VertexPosition2DColor[4];
		private static readonly short[] Indices = { 0, 1, 2, 0, 2, 3 };

		public void Draw(List<DrawableEntity> visibleEntities)
		{
			foreach (var entity in visibleEntities.OfType<GradientFilledRect>())
				AddToBatch(entity);
		}
	}
}