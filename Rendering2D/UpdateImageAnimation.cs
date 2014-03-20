using System.Collections.Generic;
using System.Linq;
using DeltaEngine.Entities;

namespace DeltaEngine.Rendering2D
{
	/// <summary>
	/// Updates current frame for a sprite animation
	/// </summary>
	public class UpdateImageAnimation : UpdateBehavior
	{
		public UpdateImageAnimation()
			: base(Priority.First) {}

		public override void Update(IEnumerable<Entity> entities)
		{
			foreach (var sprite in entities.OfType<Sprite>().Where(sprite => sprite.IsPlaying))
				UpdateAnimation(sprite);
		}

		private static void UpdateAnimation(Sprite animation)
		{
			var animationData = animation.Material.Animation;
			animation.Elapsed += Time.Delta;
			if (animation.Elapsed >= animation.Material.Duration)
				animation.InvokeAnimationEndedAndReset();
			animation.CurrentFrame =
				(int)(animationData.Frames.Length * animation.Elapsed / animation.Material.Duration);
			animationData.UpdateMaterialDiffuseMap(animation.CurrentFrame, animation.Material);
		}
	}
}