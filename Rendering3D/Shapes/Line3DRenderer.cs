using System.Collections.Generic;
using DeltaEngine.Content;
using DeltaEngine.Datatypes;
using DeltaEngine.Entities;
using DeltaEngine.Graphics;
using DeltaEngine.Graphics.Vertices;

namespace DeltaEngine.Rendering3D.Shapes
{
	/// <summary>
	/// Responsible for rendering 3D lines
	/// </summary>
	public class Line3DRenderer : DrawBehavior
	{
		public Line3DRenderer(Drawing draw)
		{
			this.draw = draw;
			material = new Material(ShaderFlags.Colored, "");
		}

		private readonly Drawing draw;
		private readonly Material material;

		public void Draw(List<DrawableEntity> visibleEntities)
		{
			foreach (var entity in visibleEntities)
				AddVerticesFromLine(entity);
			if (vertices.Count == 0)
				return; //ncrunch: no coverage
			draw.AddLines(material, vertices.ToArray());
			vertices.Clear();
		}

		private void AddVerticesFromLine(DrawableEntity entity)
		{
			var color = entity.Get<Color>();
			var points = entity.GetInterpolatedList<Vector3D>();
			foreach (Vector3D point in points)
				vertices.Add(new VertexPosition3DColor(point, color));
		}

		private readonly List<VertexPosition3DColor> vertices = new List<VertexPosition3DColor>();
	}
}