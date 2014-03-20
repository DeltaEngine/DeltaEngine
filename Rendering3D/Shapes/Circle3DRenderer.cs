using System.Collections.Generic;
using DeltaEngine.Content;
using DeltaEngine.Datatypes;
using DeltaEngine.Entities;
using DeltaEngine.Graphics;
using DeltaEngine.Graphics.Vertices;

namespace DeltaEngine.Rendering3D.Shapes
{
	public class Circle3DRenderer : DrawBehavior
	{
		public Circle3DRenderer(Drawing draw)
		{
			this.draw = draw;
			material = new Material(ShaderFlags.Colored, "");
		}

		private readonly Drawing draw;
		private readonly Material material;

		public void Draw(List<DrawableEntity> visibleEntities)
		{
			foreach (var entity in visibleEntities)
				AddVerticesFromEllipse(entity);
			if (vertices.Count == 0)
				return; //ncrunch: no coverage
			draw.AddLines(material, vertices.ToArray());
			vertices.Clear();
		}

		private readonly List<VertexPosition3DColor> vertices = new List<VertexPosition3DColor>();

		private void AddVerticesFromEllipse(DrawableEntity entity)
		{
			var color = entity.Get<Color>();
			var center = entity.Get<Vector3D>();
			var radius = entity.Get<float>();
			var point = Vector3D.UnitX * radius;
			for (int i = 0; i < 360; i += 5)
			{
				var rotatedPoint = point.RotateAround(Vector3D.UnitZ, i);
				vertices.Add(new VertexPosition3DColor(rotatedPoint + center, color));
			}
		}
	}
}
