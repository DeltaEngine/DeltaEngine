using System;
using DeltaEngine.Extensions;
using NUnit.Framework;

namespace DeltaEngine.Editor.Tests
{
	public class AppTests
	{
		[Test]
		public void WithoutEnvironmentVariableDefaultSourceCodePathShouldBeReturned()
		{
			MakeSureEnvironmentVariableIsDeleted();
			var defaultSourcePath = App.GetInstalledOrFallbackEnginePath();
			Assert.IsTrue(defaultSourcePath == @"C:\Code\DeltaEngine" ||
				defaultSourcePath == @"C:\Development\DeltaEngine", defaultSourcePath);
		}

		private static void MakeSureEnvironmentVariableIsDeleted()
		{
			if (PathExtensions.IsDeltaEnginePathEnvironmentVariableAvailable())
				Environment.SetEnvironmentVariable(PathExtensions.EnginePathEnvironmentVariableName, null);
		}
	}
}