using System;
using System.IO;
using System.Linq;
using System.Reflection;

namespace DeltaEngine.Editor.Core
{
	public static class EmbeddedResourcesLoader
	{
		//ncrunch: no coverage start
		static EmbeddedResourcesLoader()
		{
			var editorExePath = Path.Combine(Directory.GetCurrentDirectory(), "DeltaEngine.Editor.exe");
			var appDomain = AppDomain.CreateDomain("EmbeddedResourcesLoader");
			EditorAssembly = appDomain.Load(File.ReadAllBytes(editorExePath));
		}

		private static readonly Assembly EditorAssembly;

		public static string GetFullResourceName(string resourceName)
		{
			var resourceNameWithNamespace = BuildFullResourceName(EditorAssembly, resourceName);
			if (ResourceExists(EditorAssembly, resourceNameWithNamespace))
				return resourceNameWithNamespace;
			throw new EmbeddedResourceNotFound(resourceName);
		}

		private static string BuildFullResourceName(Assembly callingAssembly, string resourceName)
		{
			return callingAssembly.GetName().Name + "." + resourceName;
		}

		private static bool ResourceExists(Assembly callingAssembly, string resourceNameWithNamespace)
		{
			var manifestResourceNames = callingAssembly.GetManifestResourceNames().ToList();
			return manifestResourceNames.Contains(resourceNameWithNamespace);
		}

		internal class EmbeddedResourceNotFound : Exception
		{
			public EmbeddedResourceNotFound(string resourceName)
				: base(resourceName) {}
		}

		public static Stream GetEmbeddedResourceStream(string resourceName)
		{
			var fullResourceName = GetFullResourceName(resourceName);
			return EditorAssembly.GetManifestResourceStream(fullResourceName);
		}
	}
}