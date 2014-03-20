using System.Collections.Generic;

namespace DeltaEngine.Entities
{
	/// <summary>
	/// Draws all entities at the same level of priority.
	/// </summary>
	public interface DrawBehavior
	{
		void Draw(List<DrawableEntity> visibleEntities);
	}
}