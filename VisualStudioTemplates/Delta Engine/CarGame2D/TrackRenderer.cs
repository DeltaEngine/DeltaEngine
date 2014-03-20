using System.Collections.Generic;
using DeltaEngine.Core;
using DeltaEngine.Datatypes;
using DeltaEngine.Entities;
using DeltaEngine.Graphics.Vertices;
using DeltaEngine.Rendering2D;
using DeltaEngine.ScreenSpaces;

namespace $safeprojectname$
{
	public class TrackRenderer : DrawBehavior
	{
		public TrackRenderer(BatchRenderer2D renderer)
		{
			this.renderer = renderer;
		}

		private readonly BatchRenderer2D renderer;

		public void Draw(List<DrawableEntity> visibleEntities)
		{
			foreach (var entity in visibleEntities)
				AddVerticesToBatch((Track)entity);
		}

		private void AddVerticesToBatch(Track track)
		{
			screen = ScreenSpace.Current;
			innerBounds = track.TrackInnerBounds;
			outerBounds = track.TrackOuterBounds;
			var batch =
				(Batch2D)renderer.FindOrCreateBatch(track.Material, BlendMode.Normal, innerBounds.Length);
			for (int index = 0; index < innerBounds.Length; index++)
				AddIndicesAndVerticesForTrackPart(batch, index);
		}

		private ScreenSpace screen;
		private Vector2D[] innerBounds;
		private Vector2D[] outerBounds;

		private void AddIndicesAndVerticesForTrackPart(Batch2D batch, int index)
		{
			batch.AddIndices();
			float uvStep = (index % UVLength) / (float)UVLength;
			batch.verticesUV[batch.verticesIndex++] = new VertexPosition2DUV(
				screen.ToPixelSpace(outerBounds[index]), new Vector2D(0, uvStep));
			batch.verticesUV[batch.verticesIndex++] = new VertexPosition2DUV(
				screen.ToPixelSpace(innerBounds[index]), new Vector2D(1, uvStep));
			uvStep = ((index % UVLength) + 1) / (float)UVLength;
			batch.verticesUV[batch.verticesIndex++] = new VertexPosition2DUV(
				screen.ToPixelSpace(innerBounds[(index + 1) % innerBounds.Length]), new Vector2D(1, uvStep));
			batch.verticesUV[batch.verticesIndex++] = new VertexPosition2DUV(
				screen.ToPixelSpace(outerBounds[(index + 1) % outerBounds.Length]), new Vector2D(0, uvStep));
		}

		private const int UVLength = 9;
	}
}