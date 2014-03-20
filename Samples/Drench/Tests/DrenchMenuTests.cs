using DeltaEngine.Datatypes;
using DeltaEngine.Input;
using DeltaEngine.Input.Mocks;
using DeltaEngine.Platforms;
using Drench.Games;
using NUnit.Framework;

namespace Drench.Tests
{
	public class DrenchMenuTests : TestWithMocksOrVisually
	{
		//ncrunch: no coverage start
		[SetUp]
		public void Setup()
		{
			menu = new DrenchMenu();
			Assert.True(menu.IsPauseable);
			mouse = Resolve<Mouse>() as MockMouse;
			if (mouse != null)
				mouse.SetNativePosition(Vector2D.Zero);
		}

		private DrenchMenu menu;
		private MockMouse mouse;

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

		[Test, CloseAfterFirstFrame, Ignore]
		public void ClickSingleHumanGame()
		{
			ClickMouse(new Vector2D(0.11f, 0.26f));
			Assert.AreEqual(typeof(SingleHumanGame), menu.game.GetType());
		}

		[Test, CloseAfterFirstFrame, Ignore]
		public void ClickHumanVsDumbAiLogic()
		{
			ClickMouse(new Vector2D(0.11f, 0.34f));
			Assert.AreEqual(typeof(HumanVsAiGame), menu.game.GetType());
		}

		[Test, CloseAfterFirstFrame, Ignore]
		public void ClickHumanVsSmartAiLogic()
		{
			ClickMouse(new Vector2D(0.11f, 0.43f));
			Assert.AreEqual(typeof(HumanVsAiGame), menu.game.GetType());
		}

		[Test, CloseAfterFirstFrame, Ignore]
		public void ClickTwoHumanGame()
		{
			ClickMouse(new Vector2D(0.11f, 0.51f));
			Assert.AreEqual(typeof(TwoHumanGame), menu.game.GetType());
		}

		[Test, CloseAfterFirstFrame, Ignore]
		public void ChangeGameWidth()
		{
			Assert.AreEqual("Board Size: 10 x 10", menu.boardSize.Text);
			SetMouseState(State.Pressing, new Vector2D(0.75f, 0.31f));
			SetMouseState(State.Pressed, new Vector2D(0.8f, 0.31f));
			SetMouseState(State.Released, new Vector2D(0.8f, 0.31f));
			if (!IsMockResolver)
				return;
			Assert.AreEqual("Board Size: 12 x 10", menu.boardSize.Text);
			SetMouseState(State.Pressing, new Vector2D(0.75f, 0.376f));
			SetMouseState(State.Pressed, new Vector2D(0.8f, 0.376f));
			SetMouseState(State.Released, new Vector2D(0.8f, 0.376f));
			Assert.AreEqual("Board Size: 12 x 12", menu.boardSize.Text);
		}

		[Test, CloseAfterFirstFrame, Ignore]
		public void NetworkGame()
		{
			ClickMouse(new Vector2D(0.11f, 0.6f));
			ClickMouse(new Vector2D(0.11f, 0.68f));
		}
	}
}