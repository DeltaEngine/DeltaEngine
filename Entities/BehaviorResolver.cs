using System;

namespace DeltaEngine.Entities
{
	/// <summary>
	/// Abstract factory to provide access to create entity behaviors on demand via the resolver.
	/// </summary>
	public interface BehaviorResolver
	{
		UpdateBehavior ResolveUpdateBehavior(Type behaviorType);
		DrawBehavior ResolveDrawBehavior(Type behaviorType);
	}
}