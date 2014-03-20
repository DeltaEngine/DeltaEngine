using System;
using NUnit.Framework;

namespace DeltaEngine.Editor.Core.Tests
{
	/// <summary>
	/// Tests the conversion of DeltaEngineFrameworks to the internal platforms names
	/// </summary>
	public class DeltaEngineFrameworkExtensionsTests
	{
		[TestCase(DeltaEngineFramework.GLFW, "WindowsGLFW3")]
		[TestCase(DeltaEngineFramework.MonoGame, "WindowsMonoGame")]
		[TestCase(DeltaEngineFramework.OpenTK, "WindowsOpenTK20")]
		[TestCase(DeltaEngineFramework.SharpDX, "WindowsSharpDX")]
		[TestCase(DeltaEngineFramework.SlimDX, "WindowsSlimDX")]
		[TestCase(DeltaEngineFramework.Xna, "WindowsXna")]
		public void MapPublicToInternalName(DeltaEngineFramework publicName, string internalName)
		{
			Assert.AreEqual(internalName, publicName.ToInternalName());
		}

		[Test]
		public void TryToMapPublicToInternalNameThrows()
		{
			Assert.Throws<ArgumentException>(() => DeltaEngineFramework.Default.ToInternalName());
		}

		[TestCase(DeltaEngineFramework.GLFW, "GLFW3")]
		[TestCase(DeltaEngineFramework.MonoGame, "MonoGame")]
		[TestCase(DeltaEngineFramework.OpenTK, "OpenTK20")]
		[TestCase(DeltaEngineFramework.SharpDX, "SharpDX")]
		[TestCase(DeltaEngineFramework.SlimDX, "SlimDX")]
		[TestCase(DeltaEngineFramework.Xna, "Xna")]
		public void MapPublicToInternalShortName(DeltaEngineFramework publicName, string internalName)
		{
			Assert.AreEqual(internalName, publicName.ToInternalShortName());
		}

		[Test]
		public void TryToMapPublicToInternalShortNameThrows()
		{
			Assert.Throws<ArgumentException>(() => DeltaEngineFramework.Default.ToInternalShortName());
		}

		[TestCase(DeltaEngineFramework.GLFW, "GLFW")]
		[TestCase(DeltaEngineFramework.MonoGame, "MonoGame")]
		[TestCase(DeltaEngineFramework.OpenTK, "OpenTK")]
		[TestCase(DeltaEngineFramework.SharpDX, "SharpDX")]
		[TestCase(DeltaEngineFramework.SlimDX, "SlimDX")]
		[TestCase(DeltaEngineFramework.Xna, "Xna")]
		public void CreateFromString(DeltaEngineFramework framework, string fromString)
		{
			Assert.AreEqual(framework, DeltaEngineFrameworkExtensions.FromString(fromString));
		}
	}
}