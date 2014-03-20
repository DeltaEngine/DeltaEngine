using System.Collections.Generic;

namespace DeltaEngine.Entities
{
	/// <summary>
	/// Goes through all entities of a specific type each update tick.
	/// </summary>
	public abstract class UpdateBehavior
	{
		protected UpdateBehavior(Priority priority = Priority.Normal, bool isPauseable = true)
		{
			this.priority = priority;
			this.isPauseable = isPauseable;
		}

		internal readonly Priority priority;
		internal readonly bool isPauseable;

		public abstract void Update(IEnumerable<Entity> entities);
	}
}