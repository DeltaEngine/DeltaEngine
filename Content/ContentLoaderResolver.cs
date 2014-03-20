using System;
using System.Globalization;
using System.Reflection;

namespace DeltaEngine.Content
{
	/// <summary>
	/// Simple factory to provide access to create content data on demand without any resolver.
	/// Once the <see cref="Resolver"/> is started it will replace this functionality.
	/// </summary>
	public class ContentLoaderResolver
	{
		internal ContentLoaderResolver() {}

		public virtual ContentLoader ResolveContentLoader(Type contentLoaderType)
		{
			var parameters = CreationParameterForContentLoader != null
				? new[] { CreationParameterForContentLoader } : null;
			CreationParameterForContentLoader = null;
			return Activator.CreateInstance(contentLoaderType, PrivateBindingFlags, Type.DefaultBinder,
				parameters, CultureInfo.CurrentCulture) as ContentLoader;
		}

		internal static object CreationParameterForContentLoader;

		private const BindingFlags PrivateBindingFlags =
			BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;

		public virtual ContentData Resolve(Type contentType, string contentName)
		{
			return Activator.CreateInstance(contentType, PrivateBindingFlags, Type.DefaultBinder,
				new object[] { contentName }, CultureInfo.CurrentCulture) as ContentData;
		}

		public virtual ContentData Resolve(Type contentType, ContentCreationData data)
		{
			return Activator.CreateInstance(contentType, PrivateBindingFlags, Type.DefaultBinder,
				new object[] { data }, CultureInfo.CurrentCulture) as ContentData;
		}

		public virtual void MakeSureResolverIsInitializedAndContentIsReady() {}
	}
}