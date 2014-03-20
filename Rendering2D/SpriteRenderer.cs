using System.Collections.Generic;
using DeltaEngine.Entities;

namespace DeltaEngine.Rendering2D
{
	public class SpriteRenderer : DrawBehavior
	{
		public SpriteRenderer(BatchRenderer2D renderer)
		{
			this.renderer = renderer;
		}

		private readonly BatchRenderer2D renderer;

		public void Draw(List<DrawableEntity> visibleEntities)
		{
			foreach (var entity in visibleEntities)
				AddVerticesToBatch((Sprite)entity);
		}

		private void AddVerticesToBatch(Sprite sprite)
		{
			var batch = renderer.FindOrCreateBatch(sprite.Material, sprite.BlendMode);
			((Batch2D)batch).AddIndicesAndVertices(sprite);
		}
	}
}