using System;

namespace DeltaEngine.Entities
{
	/// <summary>
	/// Allows entities to be easily filtered so that only specific ones are updated.
	/// </summary>
	public interface Filtered
	{
		Func<Entity, bool> Filter { get; }
	}
}