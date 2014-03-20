using System;
using DeltaEngine.Content;
using DeltaEngine.Content.Online;
using DeltaEngine.Networking.Tcp;

namespace DeltaEngine.Platforms
{
	internal class AutofacContentLoaderResolver : ContentLoaderResolver
	{
		public AutofacContentLoaderResolver(Resolver resolver)
		{
			this.resolver = resolver;
		}

		private readonly Resolver resolver;

		public override ContentLoader ResolveContentLoader(Type contentLoaderType)
		{
			if (contentLoaderType == typeof(DeveloperOnlineContentLoader))
				return new DeveloperOnlineContentLoader(resolver.Resolve<OnlineServiceConnection>()); //ncrunch: no coverage
			return base.ResolveContentLoader(contentLoaderType);
		}

		public override ContentData Resolve(Type contentType, string contentName)
		{
			return resolver.Resolve(contentType, contentName) as ContentData;
		}

		public override ContentData Resolve(Type contentType, ContentCreationData data)
		{
			return resolver.Resolve(contentType, data) as ContentData;
		}

		public override void MakeSureResolverIsInitializedAndContentIsReady()
		{
			resolver.MakeSureContentManagerIsReady();
		}
	}
}
