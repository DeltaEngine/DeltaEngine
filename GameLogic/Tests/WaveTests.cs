using NUnit.Framework;

namespace DeltaEngine.GameLogic.Tests
{
	public class WaveTests
	{
		[Test]
		public void CopyingWaveCopiesTheWaveData()
		{
			var wave = new Wave(0.1f, 0.5f, "Cloth", "");
			var copiedWave = new Wave(wave);
			Assert.AreEqual("0.1, 0.5, 0, 1, Cloth", copiedWave.ToString());
		}

		[Test]
		public void TestWaveXmlData()
		{
			var wave = new Wave(0.1f, 1.0f, "Paper, Cloth", "Dummy");
			var waveXml = wave.AsXmlData();
			Assert.AreEqual("Dummy", waveXml.GetAttributeValue("ShortName"));
			Assert.AreEqual("0.1", waveXml.GetAttributeValue("WaitTime"));
			Assert.AreEqual("1", waveXml.GetAttributeValue("SpawnInterval"));
			Assert.AreEqual("Paper, Cloth", waveXml.GetAttributeValue("SpawnTypeList"));
			Assert.AreEqual("1", waveXml.GetAttributeValue("MaxSpawnItems"));
			Assert.AreEqual("0", waveXml.GetAttributeValue("MaxTime"));
		}
	}
}