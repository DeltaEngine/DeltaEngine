using DeltaEngine.Platforms;
using NUnit.Framework;

namespace Asteroids.Tests
{
	internal class HudTests : TestWithMocksOrVisually
	{
		[Test, CloseAfterFirstFrame]
		public void SetScore()
		{
			var hud = new HudInterface();
			hud.SetScoreText(5);
			Assert.AreEqual("5", hud.ScoreDisplay.Text);
		}

		[Test, CloseAfterFirstFrame]
		public void SetIngameOrGameOver()
		{
			var hudIngame = new HudInterface();
			hudIngame.SetInGameMode();
			var hudGameOver = new HudInterface();
			hudGameOver.SetGameOverText();
		}

		[Test, CloseAfterFirstFrame]
		public void DisposeInterfaceRemovesEntities()
		{
			var hud = new HudInterface();
			hud.Dispose();
			Assert.IsFalse(hud.ScoreDisplay.IsActive);
			Assert.IsFalse(hud.GameOverText.IsActive);
		}
	}
}