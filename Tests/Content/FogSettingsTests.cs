using DeltaEngine.Content;
using DeltaEngine.Datatypes;
using NUnit.Framework;

namespace DeltaEngine.Tests.Content
{
	public class FogSettingsTests
	{
		[Test]
		public void CurrentFogSettingsWillBeFilledWithDefaultValuesIfNoSpecificSettingsAreSet()
		{
			FogSettings defaultSettings = FogSettings.Current;
			Assert.AreNotEqual(0, defaultSettings.FogStart);
			Assert.AreNotEqual(0, defaultSettings.FogEnd);
			Assert.AreNotEqual(0, defaultSettings.FogColor.A);
		}

		[Test]
		public void CreatingOwnFogSettingsWillChangeTheCurrentSettings()
		{
			FogSettings defaultSettings = FogSettings.Current;
			var ownFogSettings = new FogSettings(Color.Green, 1, 2);
			Assert.AreNotEqual(defaultSettings, ownFogSettings);
			Assert.AreEqual(ownFogSettings, FogSettings.Current);
		}
	}
}