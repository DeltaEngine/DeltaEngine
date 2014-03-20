using System;
using System.Collections.Generic;
using System.IO;
using DeltaEngine.Core;
using DeltaEngine.Extensions;

namespace DeltaEngine.Content
{
	/// <summary>
	/// Loads content types like images, sounds, xml files, levels, etc. Returns cached useable
	/// instances and provides quick and easy access to all cached data plus creation of dynamic data.
	/// </summary>
	public abstract class ContentLoader : IDisposable
	{
		/// <summary>
		/// Normally set in Platforms.AppRunner to DeveloperOnlineContentLoader by creating it.
		/// Alternatively an application can create a different one (e.g. DiskContentLoader) before.
		/// </summary>
		public static void Use<T>() where T : ContentLoader
		{
			if (current != null)
				throw new ContentLoaderAlreadyExistsItIsOnlyAllowedToSetBeforeTheAppStarts();
			Type = typeof(T);
		}

		internal static ContentLoader current;

		public class ContentLoaderAlreadyExistsItIsOnlyAllowedToSetBeforeTheAppStarts : Exception {}

		public static Type Type { get; private set; }

		public static void DisposeIfInitialized()
		{
			if (current != null)
				current.Dispose();
			resolver = null;
			Type = null;
		}

		protected ContentLoader()
		{
			current = this;
			ContentProjectPath = Path.Combine("Content",
				AssemblyExtensions.GetEntryAssemblyForProjectName());
		}

		public string ContentProjectPath { get; protected set; }

		public static Content Load<Content>(string contentName) where Content : ContentData
		{
			if (!IsGeneratedContentName(contentName) && Path.HasExtension(contentName))
				throw new ContentNameShouldNotHaveExtension();
			return Load(typeof(Content), contentName) as Content;
		}

		private static bool IsGeneratedContentName(string contentName)
		{
			return contentName.StartsWith("<Generated");
		}

		public class ContentNameShouldNotHaveExtension : Exception {}

		internal static ContentData Load(Type contentClassType, string contentName)
		{
			MakeSureContentLoaderHasBeenCreated();
			if (IsGeneratedContentName(contentName))
				return resolver.Resolve(contentClassType, contentName);
			resolver.MakeSureResolverIsInitializedAndContentIsReady();
			if (!current.resources.ContainsKey(contentName))
				return current.LoadAndCacheContent(contentClassType, contentName);
			if (!current.resources[contentName].IsDisposed)
				return current.GetCachedResource(contentClassType, contentName);
			current.resources.Remove(contentName);
			return current.LoadAndCacheContent(contentClassType, contentName);
		}

		private static void MakeSureContentLoaderHasBeenCreated()
		{
			if (Type == null)
				throw new ContentLoaderUseWasNotCalled();
			if (resolver == null)
				resolver = new ContentLoaderResolver();
			if (current == null)
				current = resolver.ResolveContentLoader(Type);
		}

		internal class ContentLoaderUseWasNotCalled : Exception {}

		internal static ContentLoaderResolver resolver;

		public static bool Exists(string contentName)
		{
			return GetMetaDataFromCurrentLoader(contentName) != null;
		}

		private static ContentMetaData GetMetaDataFromCurrentLoader(string contentName)
		{
			MakeSureContentLoaderHasBeenCreated();
			resolver.MakeSureResolverIsInitializedAndContentIsReady();
			return current.GetMetaData(contentName);
		}

		public static bool Exists(string contentName, ContentType type)
		{
			var metaData = GetMetaDataFromCurrentLoader(contentName);
			return metaData != null && metaData.Type == type;
		}

		private readonly Dictionary<string, ContentData> resources =
			new Dictionary<string, ContentData>();

		private ContentData LoadAndCacheContent(Type contentType, string contentName)
		{
			var contentData = resolver.Resolve(contentType, contentName);
			LoadMetaDataAndContent(contentData);
			resources.Add(contentName, contentData);
			return contentData;
		}

		public abstract ContentMetaData GetMetaData(string contentName, Type contentClassType = null);

		private void LoadMetaDataAndContent(ContentData contentData)
		{
			contentData.MetaData = GetMetaData(contentData.Name, contentData.GetType());
			if (contentData.MetaData != null)
				contentData.InternalLoad(GetContentDataStream);
			else if (contentData.InternalAllowCreationIfContentNotFound)
				LoadContentDefaultDataWhenNotFound(contentData);
			else
				throw new ContentNotFound(contentData.Name);
		}

		private static void LoadContentDefaultDataWhenNotFound(ContentData contentData)
		{
			if (!current.GetType().Name.StartsWith("Mock"))
				Logger.Warning("Content not found: " + contentData); //ncrunch: no coverage
			contentData.InternalCreateDefault();
		}

		public class ContentNotFound : Exception
		{
			public ContentNotFound(string contentName)
				: base(contentName) {}
		}

		protected virtual Stream GetContentDataStream(ContentData content)
		{
			if (String.IsNullOrEmpty(content.MetaData.LocalFilePath))
				return Stream.Null;
			string filePath = Path.Combine(ContentProjectPath, content.MetaData.LocalFilePath);
			try
			{
				return TryGetContentDataStream(filePath);
			}
			catch (Exception ex)
			{
				throw new ContentFileDoesNotExist(filePath, ex);
			}
		}

		// ncrunch: no coverage start
		private static Stream TryGetContentDataStream(string filePath)
		{
			return File.OpenRead(filePath);
		} // ncrunch: no coverage end

		public class ContentFileDoesNotExist : Exception
		{
			public ContentFileDoesNotExist(string filePath, Exception innerException)
				: base(filePath, innerException) {}
		}

		private ContentData GetCachedResource(Type contentType, string contentName)
		{
			var cachedResource = resources[contentName];
			if (contentType.IsInstanceOfType(cachedResource))
				return cachedResource;
			throw new CachedResourceExistsButIsOfTheWrongType("Content '" + contentName + "' of type '" +
				contentType + "' requested - but type '" + cachedResource.GetType() + "' found in cache" +
				"\n '" + contentName + "' should not be in meta data files twice with different suffixes!");
		}

		public class CachedResourceExistsButIsOfTheWrongType : Exception
		{
			public CachedResourceExistsButIsOfTheWrongType(string message)
				: base(message) {}
		}

		public static T Create<T>(ContentCreationData creationData) where T : ContentData
		{
			MakeSureContentLoaderHasBeenCreated();
			return resolver.Resolve(typeof(T), creationData) as T;
		}

		public static void ReloadContent(string contentName)
		{
			var content = current.resources[contentName];
			current.LoadMetaDataAndContent(content);
			content.FireContentChangedEvent();
		}

		public virtual void Dispose()
		{
			current = null;
			resolver = null;
		}

		public static string ContentLocale
		{
			get { return current.locale; }
			set
			{
				if (string.IsNullOrEmpty(value))
					return;
				current.locale = value;
			}
		}

		private string locale = "en";

		internal static bool HasValidContentForStartup()
		{
			return current.HasValidContentAndMakeSureItIsLoaded();
		}

		protected abstract bool HasValidContentAndMakeSureItIsLoaded();

		protected string ContentMetaDataFilePath
		{
			get { return Path.Combine(ContentProjectPath, "ContentMetaData.xml"); }
		}

		public abstract DateTime LastTimeUpdated { get; }

		public bool StartedToRequestOnlineContent { get; protected set; }

		public static void RemoveResource(string contentName)
		{
			if (current.resources.ContainsKey(contentName))
				current.resources.Remove(contentName);
		}

		//ncrunch: no coverage start, only used in the EditorContentLoader
		protected void ClearBufferedResources()
		{
			if (resources != null)
				resources.Clear();
		}
	}
}