using System.Collections.Generic;
using System.Linq;
using DeltaEngine.Content;
using DeltaEngine.Datatypes;
using DeltaEngine.Entities;
using DeltaEngine.Rendering2D;

namespace GameOfDeath
{
	public class FadeSprite : Sprite
	{
		public FadeSprite(Material material, Rectangle drawArea, float fadeTime)
			: base(material, drawArea)
		{
			this.fadeTime = fadeTime;
			Start<FadeUpdate>();
		}

		private float elapsedTime;

		private class FadeUpdate : UpdateBehavior
		{
			public override void Update(IEnumerable<Entity> entities)
			{
				foreach (var fadeSprite in entities.OfType<FadeSprite>())
				{
					fadeSprite.elapsedTime += Time.Delta;
					fadeSprite.Color = Color.White.Lerp(Color.TransparentWhite,
						fadeSprite.elapsedTime / fadeSprite.fadeTime);
					if (fadeSprite.elapsedTime >= fadeSprite.fadeTime)
						fadeSprite.IsActive = false;
				}
			}
		}

		private readonly float fadeTime;
	}
}