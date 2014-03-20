using System;
using System.Collections.Generic;

namespace DeltaEngine.Platforms
{
	internal class ConsoleCommandResolver
	{
		public ConsoleCommandResolver(Resolver resolver)
		{
			this.resolver = resolver;
		}

		private readonly Resolver resolver;

		public IEnumerable<Type> RegisteredTypes
		{
			get { return resolver.RegisteredTypes; }
		}

		public object Resolve(Type type)
		{
			return resolver.Resolve(type);
		}
	}
}