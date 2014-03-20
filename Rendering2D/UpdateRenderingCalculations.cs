using System.Collections.Generic;
using DeltaEngine.Content;
using DeltaEngine.Entities;

namespace DeltaEngine.Rendering2D
{
	internal class UpdateRenderingCalculations : UpdateBehavior
	{
		public UpdateRenderingCalculations()
			: base(Priority.Last, false) {}

		public override void Update(IEnumerable<Entity> entities)
		{
			foreach (Entity entity in entities)
				UpdateSpriteRenderingCalculations((Sprite)entity);
		}

		private static void UpdateSpriteRenderingCalculations(Sprite sprite)
		{
			RenderingData data = sprite.renderingData;
			if (data.RequestedDrawArea != sprite.DrawArea)
				sprite.renderingData = sprite.Material.RenderingCalculator.GetUVAndDrawArea(data.RequestedUserUV,
					sprite.DrawArea, data.FlipMode);
			RenderingData lastData = sprite.lastRenderingData;
			if (lastData.RequestedDrawArea != sprite.LastDrawArea)
				sprite.lastRenderingData =
					sprite.Material.RenderingCalculator.GetUVAndDrawArea(lastData.RequestedUserUV,
						sprite.LastDrawArea, lastData.FlipMode);
		}
	}
}