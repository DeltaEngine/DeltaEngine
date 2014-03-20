using System.Collections.Generic;
using DeltaEngine.Content.Mocks;
using NUnit.Framework;

namespace DeltaEngine.Content.Xml.Tests
{
	internal class LocalizationTests
	{
		[Test]
		public void GetLocalizedString()
		{
			ContentLoader.Use<MockContentLoader>();
			var localization = ContentLoader.Load<Localization>("Texts");
			localization.TwoLetterLanguageName = "en";
			Assert.AreEqual(localization.GetText("Go"), "Go");
			localization.TwoLetterLanguageName = "de";
			Assert.AreEqual(localization.GetText("Go"), "Los");
			localization.TwoLetterLanguageName = "es";
			Assert.AreEqual(localization.GetText("Go"), "¡vamos!");
			Assert.Throws<KeyNotFoundException>(
				() => localization.GetText("ThatIsATestExampleToThrowOneException"));
			ContentLoader.DisposeIfInitialized();
		}
	}
}