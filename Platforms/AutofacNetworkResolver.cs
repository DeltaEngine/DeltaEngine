using DeltaEngine.Networking;

namespace DeltaEngine.Platforms
{
	internal class AutofacNetworkResolver : NetworkResolver
	{
		//ncrunch: no coverage start
		public AutofacNetworkResolver(Resolver resolver)
		{
			this.resolver = resolver;
		}

		private readonly Resolver resolver;

		public Server ResolveServer()
		{
			return resolver.Resolve<Server>();
		}

		public Client ResolveClient()
		{
			return resolver.Resolve<Client>();
		}
	}
}