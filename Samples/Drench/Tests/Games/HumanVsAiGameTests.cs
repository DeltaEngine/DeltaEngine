using DeltaEngine.Core;
using DeltaEngine.Datatypes;
using DeltaEngine.Input;
using DeltaEngine.Input.Mocks;
using DeltaEngine.Platforms;
using DeltaEngine.ScreenSpaces;
using Drench.Games;
using Drench.Logics;
using NUnit.Framework;
using Randomizer = DeltaEngine.Core.Randomizer;

namespace Drench.Tests.Games
{
	[Ignore]
	public class HumanVsAiGameTests : TestWithMocksOrVisually
	{
		//ncrunch: no coverage start
		[SetUp, CloseAfterFirstFrame]
		public void SetUp()
		{
			Randomizer.Use(new FixedRandom(new[] { 0.1f, 0.6f, 0.7f, 0.2f }));
			InitializeMouse();
			AdvanceTimeAndUpdateEntities();
			game = new HumanVsAiGame(new HumanVsDumbAiLogic(BoardTests.Width, BoardTests.Height));
		}

		private Game game;

		private void InitializeMouse()
		{
			mouse = Resolve<Mouse>() as MockMouse;
			if (mouse != null)
				mouse.SetNativePosition(Vector2D.Zero);
		}

		private MockMouse mouse;

		[Test]
		public void NewGameInstructions()
		{
			Assert.AreEqual("*** Player 1: 1  ***", game.upperText.Text);
			Assert.AreEqual("Player 2: 1  ", game.lowerText.Text);
		}

		[Test]
		public void ClickInvalidSquare()
		{
			var firstSquare = new Vector2D(ScreenSpace.Current.Left + Game.Border + 0.01f,
				ScreenSpace.Current.Top + Game.Border + 0.01f);
			ClickMouse(firstSquare);
			Assert.AreEqual("*** Player 1: 1 - Invalid Move! ***", game.upperText.Text);
		}

		private void ClickMouse(Vector2D position)
		{
			SetMouseState(State.Pressing, position);
			SetMouseState(State.Releasing, position);
		}

		private void SetMouseState(State state, Vector2D position)
		{
			if (mouse == null)
				return; //ncrunch: no coverage
			mouse.SetNativePosition(position);
			mouse.SetButtonState(MouseButton.Left, state);
			AdvanceTimeAndUpdateEntities();
		}

		[Test]
		public void ClickValidSquare()
		{
			ClickMouse(Vector2D.Half);
			Assert.AreEqual("*** Player 1: 3 Game Over! Player 1 wins! ***", game.upperText.Text);
		}
	}
}