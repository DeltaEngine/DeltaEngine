using System.Collections.Generic;
using DeltaEngine.Extensions;

namespace DeltaEngine.Entities
{
	/// <summary>
	/// Used to manage entities in connection with their behaviors, which can change during Update.
	/// </summary>
	internal class PrioritizedEntities
	{
		public readonly ChangeableList<Entity> entities = new ChangeableList<Entity>();
		public readonly Dictionary<UpdateBehavior, List<Entity>> behaviors =
			new Dictionary<UpdateBehavior, List<Entity>>();
		private int behaviorsEnumerationDepth;
		internal readonly Dictionary<UpdateBehavior, List<Entity>>
			behaviorsToBeAdded = new Dictionary<UpdateBehavior, List<Entity>>();
		private readonly Dictionary<UpdateBehavior, List<Entity>>
			behaviorsToBeRemoved = new Dictionary<UpdateBehavior, List<Entity>>();

		public void IncreaseBehaviorsEnumerationCount()
		{
			behaviorsEnumerationDepth++;
		}

		public void ReduceBehaviorsEnumerationCount()
		{
			behaviorsEnumerationDepth--;
			if (behaviorsEnumerationDepth != 0 || behaviorsToBeAdded.Count == 0 && behaviorsToBeRemoved.Count == 0)
				return;
			foreach (var pair in behaviorsToBeAdded)
				foreach (var entity in pair.Value)
					AddToBehaviors(behaviors, pair.Key, entity);
			behaviorsToBeAdded.Clear();
			foreach (var pair in behaviorsToBeRemoved)
				foreach (var entity in pair.Value)
					if (behaviors.ContainsKey(pair.Key))
						behaviors[pair.Key].Remove(entity);
			behaviorsToBeRemoved.Clear();
		}

		public void AddEntityToBehavior(UpdateBehavior behavior, Entity entity)
		{
			AddToBehaviors(behaviorsEnumerationDepth > 0 ? behaviorsToBeAdded : behaviors, behavior,
				entity);
		}

		private static void AddToBehaviors(
			Dictionary<UpdateBehavior, List<Entity>> dictionary, UpdateBehavior behavior,
			Entity entity)
		{
			if (dictionary.ContainsKey(behavior))
			{
				if (entity != null && !dictionary[behavior].Contains(entity))
					dictionary[behavior].Add(entity);
			}
			else
			{
				var list = new List<Entity>();
				if (entity != null)
					list.Add(entity);
				dictionary.Add(behavior, list);
			}
		}

		public bool RemoveEntityFromBehaviors(Entity entity, UpdateBehavior updateBehavior)
		{
			if (behaviorsEnumerationDepth > 0)
			{
				if (behaviorsToBeAdded.ContainsKey(updateBehavior))
					behaviorsToBeAdded[updateBehavior].Remove(entity);
				AddToBehaviors(behaviorsToBeRemoved, updateBehavior, entity);
				return true;
			}
			return behaviors.ContainsKey(updateBehavior) && behaviors[updateBehavior].Remove(entity);
		}

		public void Clear()
		{
			entities.Clear();
			behaviors.Clear();
		}
	}
}