using DeltaEngine.ScreenSpaces;

namespace DeltaEngine.Platforms
{
	public class AutofacScreenSpaceResolver : ScreenSpaceResolver
	{
		public AutofacScreenSpaceResolver(AppRunner resolver)
		{
			this.resolver = resolver;
		}

		private readonly AppRunner resolver;

		public ScreenSpace ResolveScreenSpace<T>() where T : ScreenSpace
		{
			return resolver.Resolve<T>();
		}
	}
}