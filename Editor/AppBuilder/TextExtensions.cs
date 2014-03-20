namespace DeltaEngine.Editor.AppBuilder
{
	/// <summary>
	/// Provides some helper methods which are required for values of some ViewModel's.
	/// </summary>
	public static class TextExtensions
	{
		public static string GetCountAndWordInPluralIfNeeded(this string wordInSingular, int count)
		{
			return count + " " + wordInSingular + (count != 1 ? "s" : "");
		}
	}
}