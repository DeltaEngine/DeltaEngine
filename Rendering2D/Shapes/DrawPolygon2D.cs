using System;
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
	/// Responsible for rendering filled 2D shapes defined by their border points
	/// </summary>
	public class DrawPolygon2D : DrawBehavior
	{
		public DrawPolygon2D(Drawing draw)
		{
			this.draw = draw;
			material = new Material(ShaderFlags.Position2DColored, "");
		}

		private readonly Drawing draw;
		private readonly Material material;

		public void Draw(List<DrawableEntity> visibleEntities)
		{
			foreach (var entity in visibleEntities)
				AddToBatch((Entity2D)entity);
			DrawBatch();
		}

		private void AddToBatch(Entity2D entity)
		{
			var points = entity.GetInterpolatedList<Vector2D>();
			if (points.Count < 3)
				return;
			if (points.Count > CircularBuffer.TotalMaximumVerticesLimit)
				throw new TooManyVerticesForPolygon(points.Count); //ncrunch: no coverage
			var color = entity.Color;
			if (offset + points.Count > vertices.Length)
				ResizeVertices(); //ncrunch: no coverage
			for (int num = 0; num < points.Count; num++)
				vertices[offset + num] =
					new VertexPosition2DColor(ScreenSpace.Current.ToPixelSpace(points[num]), color);
			BuildIndices(points.Count);
			offset += points.Count;
		}

		//ncrunch: no coverage start, involving loads of vertices
		private class TooManyVerticesForPolygon : Exception
		{
			public TooManyVerticesForPolygon(int numberOfPoints)
				: base(
					"Points: " + numberOfPoints + ", Maximum: " + CircularBuffer.TotalMaximumVerticesLimit) {}
		} //ncrunch: no coverage end

		private int offset;
		private VertexPosition2DColor[] vertices = new VertexPosition2DColor[InitialVertices];
		private const int InitialVertices = 4096;

		//ncrunch: no coverage start, involving loads of vertices
		private void ResizeVertices()
		{
			if (offset > 0)
				DrawBatch();
			if (vertices.Length >= CircularBuffer.TotalMaximumVerticesLimit)
				return;
			vertices = new VertexPosition2DColor[vertices.Length * 2];
			indices = new short[vertices.Length * 3];
		} //ncrunch: no coverage end

		private short[] indices = new short[InitialVertices * 3];

		private void DrawBatch()
		{
			draw.Add(material, vertices, indices, offset, numberOfIndicesUsed);
			offset = 0;
			numberOfIndicesUsed = 0;
		}

		private int numberOfIndicesUsed;

		private void BuildIndices(int numberOfPoints)
		{
			for (int num = 0; num < numberOfPoints - 2; num++)
			{
				indices[numberOfIndicesUsed++] = (short)offset;
				indices[numberOfIndicesUsed++] = (short)(offset + num + 1);
				indices[numberOfIndicesUsed++] = (short)(offset + num + 2);
			}
		}
	}
}