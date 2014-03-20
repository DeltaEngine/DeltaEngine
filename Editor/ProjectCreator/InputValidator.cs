using System.Text.RegularExpressions;

namespace DeltaEngine.Editor.ProjectCreator
{
	/// <summary>
	/// Validates user input to meet the coding standards and avoid exceptions.
	/// </summary>
	public static class InputValidator
	{
		// Useful links:
		// RegEx Tester - http://regexpal.com/
		// RegEx examples - http://gskinner.com/RegExr/
		public static bool IsValidProjectName(string validate)
		{
			return !string.IsNullOrEmpty(validate) && Regex.IsMatch(validate, "^[a-zA-Z][a-zA-Z0-9.]*$");
		}

		public static bool IsValidAbsolutePath(string path)
		{
			return !string.IsNullOrEmpty(path) && Regex.IsMatch(path, "^[A-Za-z]:(\\[A-Za-z0-9]+)*");
		}
	}
}