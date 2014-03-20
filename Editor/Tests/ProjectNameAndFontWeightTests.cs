using System.Windows;
using NUnit.Framework;

namespace DeltaEngine.Editor.Tests
{
	public class ProjectNameAndFontWeightTests
	{
		[Test]
		public void EqualObjectsNeedToHaveSameNameAndSameFontWeight()
		{
			var project = CreateProject();
			Assert.AreEqual(project, new ProjectNameAndFontWeight("EmptyApp", FontWeights.Normal));
			Assert.AreNotEqual(project, new ProjectNameAndFontWeight("EmptyApp", FontWeights.Bold));
			Assert.AreNotEqual(project, new ProjectNameAndFontWeight("GhostWars", FontWeights.Normal));
			Assert.AreNotEqual(project, new ProjectNameAndFontWeight("GhostWars", FontWeights.Bold));
		}

		private static ProjectNameAndFontWeight CreateProject()
		{
			return new ProjectNameAndFontWeight("EmptyApp", FontWeights.Normal);
		}

		[Test]
		public void ToStringReturnsNameOnly()
		{
			Assert.AreEqual("EmptyApp", CreateProject().ToString());
		}
	}
}