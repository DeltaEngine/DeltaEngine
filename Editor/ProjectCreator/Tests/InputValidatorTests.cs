using NUnit.Framework;

namespace DeltaEngine.Editor.ProjectCreator.Tests
{
	/// <summary>
	/// Tests for the user input validation.
	/// </summary>
	public class InputValidatorTests
	{
		[TestCase("Name"), TestCase("name"), TestCase("ProjectName"),
		 TestCase("DeltaEngine.Editor.ProjectCreator.Tests")]
		public void ValidFolderNames(string name)
		{
			Assert.IsTrue(InputValidator.IsValidProjectName(name));
		}

		[TestCase(""), TestCase("1Name"), TestCase("Project Name")]
		public void InvalidFolderNames(string name)
		{
			Assert.IsFalse(InputValidator.IsValidProjectName(name));
		}
	}
}