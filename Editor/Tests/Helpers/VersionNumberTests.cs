using System;
using System.Reflection;
using DeltaEngine.Editor.Helpers;
using NUnit.Framework;

namespace DeltaEngine.Editor.Tests.Helpers
{
	public class VersionNumberTests
	{
		[Test]
		public void VersionNumberForStableMilestoneReleaseIsMajorPlusMinor()
		{
			var versionNumber = new VersionNumber("1.1.0.0");
			Assert.AreEqual("v1.1", versionNumber.ToString());
		}

		[Test]
		public void VersionNumberForNightlyBetaReleaseIsMajorPlusMinorPlusBuild()
		{
			var versionNumber = new VersionNumber("1.1.1.0");
			Assert.AreEqual("v1.1.1", versionNumber.ToString());
		}

		[Test]
		public void CheckCurrentVersion()
		{
			var versionNumber = new VersionNumber();
			Version expectedVersion = Assembly.GetExecutingAssembly().GetName().Version;
			Assert.AreEqual(expectedVersion, versionNumber.Version);
		}
	}
}