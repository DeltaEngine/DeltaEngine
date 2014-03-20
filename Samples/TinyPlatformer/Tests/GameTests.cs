using DeltaEngine.Input;
using DeltaEngine.Input.Mocks;
using DeltaEngine.Platforms;
using NUnit.Framework;

namespace TinyPlatformer.Tests
{
	[Ignore]
	public class GameTests : TestWithMocksOrVisually
	{
		[SetUp]
		public void SetUp()
		{
			var map = new Map(MockMap.JsonNode);
			player = map.Player;
			new Game(map);
			InitializeKeyboard();
		}

		private Actor player;

		private void InitializeKeyboard()
		{
			keyboard = Resolve<Keyboard>() as MockKeyboard;
			AdvanceTimeAndUpdateEntities();
		}

		private MockKeyboard keyboard;

		[Test, CloseAfterFirstFrame]
		public void PlayerDoesNotWantToMoveLeftOrRight()
		{
			Assert.IsFalse(player.WantsToGoLeft);
			Assert.IsFalse(player.WantsToGoRight);
			Assert.IsFalse(player.WantsToJump);
		}

		[Test, CloseAfterFirstFrame]
		public void CursorLeftMakesPlayerWantToMoveLeft()
		{
			PressKey(Key.CursorLeft);
			Assert.IsTrue(player.WantsToGoLeft);
			Assert.IsFalse(player.WantsToGoRight);
			Assert.IsFalse(player.WantsToJump);
		}

		private void PressKey(Key key)
		{
			if (keyboard == null)
				return; //ncrunch: no coverage
			keyboard.SetKeyboardState(key, State.Pressing);
			AdvanceTimeAndUpdateEntities();
		}

		[Test, CloseAfterFirstFrame]
		public void CursorRightMakesPlayerWantToMoveRight()
		{
			PressKey(Key.CursorRight);
			Assert.IsFalse(player.WantsToGoLeft);
			Assert.IsTrue(player.WantsToGoRight);
			Assert.IsFalse(player.WantsToJump);
		}

		[Test, CloseAfterFirstFrame]
		public void CursorUpMakesPlayerWantToJump()
		{
			PressKey(Key.CursorUp);
			Assert.IsFalse(player.WantsToGoLeft);
			Assert.IsFalse(player.WantsToGoRight);
			Assert.IsTrue(player.WantsToJump);
		}

		[Test, CloseAfterFirstFrame]
		public void SpaceMakesPlayerWantToJump()
		{
			PressKey(Key.Space);
			Assert.IsFalse(player.WantsToGoLeft);
			Assert.IsFalse(player.WantsToGoRight);
			Assert.IsTrue(player.WantsToJump);
		}

		[Test, CloseAfterFirstFrame]
		public void ReleasingCursorLeftMakesPlayerStopWantingToMoveLeft()
		{
			PressKey(Key.CursorLeft);
			ReleaseKey(Key.CursorLeft);
			Assert.IsFalse(player.WantsToGoLeft);
			Assert.IsFalse(player.WantsToGoRight);
			Assert.IsFalse(player.WantsToJump);
		}

		private void ReleaseKey(Key key)
		{
			if (keyboard == null)
				return; //ncrunch: no coverage
			keyboard.SetKeyboardState(key, State.Releasing);
			AdvanceTimeAndUpdateEntities();
		}

		[Test, CloseAfterFirstFrame]
		public void ReleasingCursorRightMakesPlayerStopWantingToMoveRight()
		{
			PressKey(Key.CursorRight);
			ReleaseKey(Key.CursorRight);
			Assert.IsFalse(player.WantsToGoLeft);
			Assert.IsFalse(player.WantsToGoRight);
			Assert.IsFalse(player.WantsToJump);
		}

		[Test, CloseAfterFirstFrame]
		public void ReleasingCursorUpMakesPlayerStopWantingToJump()
		{
			PressKey(Key.CursorUp);
			ReleaseKey(Key.CursorUp);
			Assert.IsFalse(player.WantsToGoLeft);
			Assert.IsFalse(player.WantsToGoRight);
			Assert.IsFalse(player.WantsToJump);
		}

		[Test, CloseAfterFirstFrame]
		public void ReleasingSpaceMakesPlayerStopWantingToJump()
		{
			PressKey(Key.Space);
			ReleaseKey(Key.Space);
			Assert.IsFalse(player.WantsToGoLeft);
			Assert.IsFalse(player.WantsToGoRight);
			Assert.IsFalse(player.WantsToJump);
		}
	}
}