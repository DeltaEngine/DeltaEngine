using System.Collections.Generic;

namespace DeltaEngine.Content.Xml
{
	/// <summary>
	/// For localizing an application to a language.
	/// </summary>
	public class Localization : XmlContent
	{
		protected Localization(string contentName)
			: base(contentName) {}

		public string GetText(string languageKey)
		{
			var keyNode = Data.GetChild(languageKey);
			if (keyNode == null)
				throw new KeyNotFoundException(languageKey);
			return keyNode.GetAttributeValue(TwoLetterLanguageName);
		}

		public string TwoLetterLanguageName { get; set; }
	}
}