using System.Collections.Generic;

namespace DeltaEngine.Entities
{
	/// <summary>
	/// Helper for keeping lists of draw behaviors with all associated entities together.
	/// </summary>
	internal class SortedDrawBehaviors
	{
		public SortedDrawBehaviors(int renderLayer)
		{
			RenderLayer = renderLayer;
		}

		public readonly int RenderLayer;
		public readonly Dictionary<DrawBehavior, List<DrawableEntity>> behaviors =
			new Dictionary<DrawBehavior, List<DrawableEntity>>();

		public void Add(DrawBehavior behavior, DrawableEntity entity)
		{
			if (behaviors.ContainsKey(behavior))
				behaviors[behavior].Add(entity);
			else
				behaviors.Add(behavior, new List<DrawableEntity> { entity });
		}
	}
}