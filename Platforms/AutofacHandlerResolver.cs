using System;
using DeltaEngine.Entities;

namespace DeltaEngine.Platforms
{
	internal class AutofacHandlerResolver : BehaviorResolver
	{
		public AutofacHandlerResolver(Resolver resolver)
		{
			this.resolver = resolver;
		}

		private readonly Resolver resolver;

		public UpdateBehavior ResolveUpdateBehavior(Type handlerType)
		{
			return resolver.Resolve(handlerType) as UpdateBehavior;
		}

		public DrawBehavior ResolveDrawBehavior(Type handlerType)
		{
			return resolver.Resolve(handlerType) as DrawBehavior;
		}
	}
}
