using System;
using System.IO;
using DeltaEngine.Extensions;
using NUnit.Framework;

namespace DeltaEngine.Tests.Extensions
{
	[Category("Slow")]
	public class AssemblyExtensionsTests
	{
		//ncrunch: no coverage start
		[Test]
		public void CheckIfManagedAssembly()
		{
			string managedAssemblyFile = Path.Combine(Directory.GetCurrentDirectory(), "DeltaEngine.dll");
			Assert.IsTrue(AssemblyExtensions.IsManagedAssembly(managedAssemblyFile));
		}

		[Test]
		public void CheckIfNotManagedAssembly()
		{
			string unmanagedAssemblyFile =
				Path.Combine(Environment.ExpandEnvironmentVariables("%WinDir%"), "System32", "user32.dll");
			Assert.IsFalse(AssemblyExtensions.IsManagedAssembly(unmanagedAssemblyFile));
		}
	}
}