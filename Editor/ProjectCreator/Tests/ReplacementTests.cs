using NUnit.Framework;

namespace DeltaEngine.Editor.ProjectCreator.Tests
{
	/// <summary>
	/// Tests for the class used for string replacements.
	/// </summary>
	public class ReplacementTests
	{
		[Test]
		public void Create()
		{
			var replacement = new Replacement("old string", "new string");
			Assert.AreEqual("old string", replacement.OldValue);
			Assert.AreEqual("new string", replacement.NewValue);
		}
	}
}