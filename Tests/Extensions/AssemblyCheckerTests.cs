using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using DeltaEngine.Extensions;
using NUnit.Framework;

namespace DeltaEngine.Tests.Extensions
{
	/// <summary>
	/// AssemblyChecker.IsAllowed is used whenever we have to check all loaded assemblies for types.
	/// Used in BinaryDataExtensions, a more complex example is in DeltaEngine.Platforms.All.
	/// </summary>
	public class AssemblyCheckerTests
	{
		//ncrunch: no coverage start
		[Test, Ignore]
		public void MakeSureToOnlyIncludeAllowedDeltaEngineAndUserAssemblies()
		{
			Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
			var assembliesAllowed = new List<string>();
			foreach (Assembly assembly in assemblies.Where(assembly => assembly.IsAllowed()))
				assembliesAllowed.Add(assembly.GetName().Name);
			// At this point only DeltaEngine.Tests, DeltaEngine, DeltaEngine.Mocks should be loaded
			Assert.AreEqual(3, assembliesAllowed.Count, "Assemblies: " + assembliesAllowed.ToText());
		} 
	}
}