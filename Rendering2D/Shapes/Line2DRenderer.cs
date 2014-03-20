using System.Collections.Generic;
using DeltaEngine.Content;
using DeltaEngine.Datatypes;
using DeltaEngine.Entities;
using DeltaEngine.Graphics;
using DeltaEngine.Graphics.Vertices;
using DeltaEngine.ScreenSpaces;

namespace DeltaEngine.Rendering2D.Shapes
{
	/// <summary>
	/// Responsible for rendering all kinds of 2D lines (Line2D, Circle, etc)
	/// </summary>
	public class Line2DRenderer : DrawBehavior
	{
		public Line2DRenderer(Drawing draw)
		{
			this.draw = draw;
			material = new Material(ShaderFlags.Position2DColored, "");
		}

		private readonly Drawing draw;
		private readonly Material material;

		public void Draw(List<DrawableEntity> visibleEntities)
		{
			foreach (var entity in visibleEntities)
				AddVerticesFromLine(entity);
			if (vertices.Count == 0)
				return;
			draw.AddLines(material, vertices.ToArray());
			vertices.Clear();
		}

		private void AddVerticesFromLine(DrawableEntity entity)
		{
			var color = entity.Get<Color>();
			var points = entity.GetInterpolatedList<Vector2D>();
			foreach (Vector2D point in points)
				vertices.Add(new VertexPosition2DColor(ScreenSpace.Current.ToPixelSpaceRounded(point),
					color));
		}

		private readonly List<VertexPosition2DColor> vertices = new List<VertexPosition2DColor>();
	}
}