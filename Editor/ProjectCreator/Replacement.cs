namespace DeltaEngine.Editor.ProjectCreator
{
	/// <summary>
	/// Holds an old and a new string used for string replacements.
	/// </summary>
	public class Replacement
	{
		public Replacement(string oldValue, string newValue)
		{
			OldValue = oldValue;
			NewValue = newValue;
		}

		public string OldValue { get; private set; }
		public string NewValue { get; private set; }
	}
}