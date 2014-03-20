using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using DeltaEngine.Core;
using DeltaEngine.Extensions;

namespace DeltaEngine.Platforms
{
	internal class AssemblyTypeLoader
	{
		public AssemblyTypeLoader(Resolver resolver)
		{
			this.resolver = resolver;
		}

		private readonly Resolver resolver;

		public void RegisterAllTypesFromAllAssemblies<ContentDataType, UpdateType, DrawType>()
		{
			var assemblies = LoadAllUnloadedAssemblies(AppDomain.CurrentDomain.GetAssemblies());
			foreach (Assembly assembly in assemblies)
			{
				var name = assembly.GetName().Name;
				if (AssemblyExtensions.IsPlatformAssembly(name) || !assembly.IsAllowed() ||
					name == "DeltaEngine.Graphics" || name == "DeltaEngine.Input" ||
					//name == "DeltaEngine.Physics2D" || name == "DeltaEngine.Physics3D" ||
					name == "DeltaEngine.Logging" || name == "DeltaEngine.Networking" ||
					name.StartsWith("DeltaEngine.Content") && !name.EndsWith(".Tests") ||
					name.EndsWith(".Mocks"))
					continue;
				Type[] assemblyTypes = GetAssemblyTypes(assembly);
				if (assemblyTypes == null)
					continue; //ncrunch: no coverage
				RegisterAllTypesInAssembly<ContentDataType>(assemblyTypes, false);
				RegisterAllTypesInAssembly<UpdateType>(assemblyTypes, true);
				RegisterAllTypesInAssembly<DrawType>(assemblyTypes, true);
				resolver.RegisterAllTypesInAssembly(assemblyTypes);
			}
		}

		private static IEnumerable<Assembly> LoadAllUnloadedAssemblies(Assembly[] loadedAssemblies)
		{
			if (StackTraceExtensions.ForceUseOfMockResolver())
				return loadedAssemblies;
			//ncrunch: no coverage start
			var assemblies = new List<Assembly>(loadedAssemblies);
			var dllFiles = Directory.GetFiles(Directory.GetCurrentDirectory(), "*.dll");
			foreach (var filePath in dllFiles)
				try
				{
					assemblies = TryLoadAllUnloadedAssemblies(assemblies, filePath);
				}
				catch (Exception ex)
				{
					Logger.Warning("Failed to load assembly " + filePath + ": " + ex.Message);
				}
			foreach (var assembly in loadedAssemblies)
				if (assembly.IsAllowed() && !AssemblyExtensions.IsPlatformAssembly(assembly.GetName().Name))
					LoadDependentAssemblies(assembly, assemblies);
			return assemblies;
		}

		private static List<Assembly> TryLoadAllUnloadedAssemblies(List<Assembly> assemblies,
			string filePath)
		{
			string name = Path.GetFileNameWithoutExtension(filePath);
			if (AssemblyExtensions.IsManagedAssembly(filePath) && new AssemblyName(name).IsAllowed() &&
				!AssemblyExtensions.IsPlatformAssembly(name) && !name.EndsWith(".Mocks") &&
				!name.EndsWith(".Tests") && assemblies.All(a => a.GetName().Name != name))
				assemblies.Add(Assembly.LoadFrom(filePath));
			return assemblies;
		}

		private static void LoadDependentAssemblies(Assembly assembly, List<Assembly> assemblies)
		{
			foreach (var dependency in assembly.GetReferencedAssemblies())
				if (!IsConflictingDependency(dependency) && dependency.IsAllowed() &&
					!dependency.Name.EndsWith(".Mocks") &&
					assemblies.All(loaded => dependency.Name != loaded.GetName().Name))
					assemblies.Add(Assembly.Load(dependency));
		}

		/// <summary>
		/// Needed in Win 8.1 since Windows.Storage (referenced by Windows.Foundation) cannot be loaded.
		/// </summary>
		private static bool IsConflictingDependency(AssemblyName dependency)
		{
			return dependency.Name == "Windows.Storage";
		} //ncrunch: no coverage end

		private static Type[] GetAssemblyTypes(Assembly assembly)
		{
			try
			{
				return TryGetAssemblyTypes(assembly);
			}
			//ncrunch: no coverage start
			catch (Exception ex)
			{
				string errorText = ex.ToString();
				var loaderError = ex as ReflectionTypeLoadException;
				if (loaderError != null)
					foreach (var error in loaderError.LoaderExceptions)
						errorText += "\n\n" + error;
				Logger.Warning("Failed to load types from " + assembly.GetName().Name +
					" (maybe outdated?): " + errorText);
				return null;
			} //ncrunch: no coverage end
		}

		private static Type[] TryGetAssemblyTypes(Assembly assembly)
		{
			return assembly.GetTypes();
		}

		private void RegisterAllTypesInAssembly<T>(Type[] assemblyTypes, bool registerAsSingleton)
		{
			foreach (Type type in assemblyTypes)
				if (typeof(T).IsAssignableFrom(type) && IsTypeResolveable(type))
					if (registerAsSingleton)
						resolver.RegisterSingleton(type);
					else
						resolver.Register(type);
		}

		/// <summary>
		/// Allows to ignore most types. IsAbstract will also check if the class is static.
		/// </summary>
		public static bool IsTypeResolveable(Type type)
		{
			if (type.IsEnum || type.IsAbstract || type.IsInterface || type.IsValueType ||
				typeof(Exception).IsAssignableFrom(type) || type == typeof(Action) ||
				type == typeof(Action<>) || typeof(MulticastDelegate).IsAssignableFrom(type))
				return false;
			var typeName = type.Name;
			if (typeName == "Program" || typeName.StartsWith("<") || typeName.Contains("`1") ||
				typeName.StartsWith("#") || typeName.StartsWith("Mock") || typeName.EndsWith("Tests"))
				return false;
			return !IgnoreForResolverAttribute.IsTypeIgnored(type);
		}
	}
}