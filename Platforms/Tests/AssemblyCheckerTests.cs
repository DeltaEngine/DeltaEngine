using System;
using System.IO;
using System.Linq;
using System.Reflection;
using DeltaEngine.Core;
using DeltaEngine.Extensions;
using DeltaEngine.Mocks;
using NUnit.Framework;

namespace DeltaEngine.Platforms.Tests
{
	/// <summary>
	/// AssemblyChecker.IsAllowed is used whenever we have to check all loaded assemblies for types.
	/// Examples include BinaryDataExtensions and AutofacResolver.RegisterAllTypesFromAllAssemblies
	/// </summary>
	[Category("Slow")]
	public class AssemblyCheckerTests : TestWithMocksOrVisually
	{
		//ncrunch: no coverage start
		[TearDown]
		public void Dispose()
		{
			File.Delete(Path.Combine(Directory.GetCurrentDirectory(), "test.dll"));
		}

		[Test]
		public void MakeSureToOnlyIncludeAllowedDeltaEngineAndUserAssemblies()
		{
			Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
			var assembliesAllowed =
				assemblies.Where(assembly => assembly.IsAllowed()).Select(a => a.GetName().Name).ToList();
			Assert.Greater(assembliesAllowed.Count, 0, "Assemblies: " + assembliesAllowed.ToText());
		}
	}
}