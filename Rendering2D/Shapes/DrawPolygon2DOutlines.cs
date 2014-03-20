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
	/// Responsible for rendering the outline of 2D shapes defined by their border points
	/// </summary>
	public class DrawPolygon2DOutlines : DrawBehavior
	{
		public DrawPolygon2DOutlines(Drawing draw)
		{
			this.draw = draw;
			material = new Material(ShaderFlags.Position2DColored, "");
		}

		private readonly Drawing draw;
		private readonly Material material;

		public void Draw(List<DrawableEntity> visibleEntities)
		{
			foreach (Entity2D entity in visibleEntities)
				AddToBatch(entity);
			if (vertices.Count == 0)
				return; //ncrunch: no coverage
			draw.AddLines(material, vertices.ToArray());
			vertices.Clear();
		}

		private void AddToBatch(Entity2D entity)
		{
			var color = entity.Get<OutlineColor>().Value;
			List<Vector2D> points = null;
			if (entity.Contains<List<Vector2D>>())
				points = entity.Get<List<Vector2D>>();
			if (points == null || points.Count <= 1)
			{
				points = new List<Vector2D>();
				points.Add(entity.DrawArea.TopLeft);
				points.Add(entity.DrawArea.BottomLeft);
				points.Add(entity.DrawArea.BottomRight);
				points.Add(entity.DrawArea.TopRight);
			}
			lastPoint = points[points.Count - 1];
			foreach (Vector2D point in points)
				AddLine(point, color);
		}

		private Vector2D lastPoint;

		private void AddLine(Vector2D point, Color color)
		{
			vertices.Add(new VertexPosition2DColor(ScreenSpace.Current.ToPixelSpaceRounded(lastPoint),
				color));
			vertices.Add(new VertexPosition2DColor(ScreenSpace.Current.ToPixelSpaceRounded(point), color));
			lastPoint = point;
		}

		private readonly List<VertexPosition2DColor> vertices = new List<VertexPosition2DColor>();
	}
}