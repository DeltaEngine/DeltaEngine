using DeltaEngine.Core;
using DeltaEngine.Datatypes;
using DeltaEngine.Entities;
using DeltaEngine.Input;
using DeltaEngine.Input.Mocks;
using DeltaEngine.Mocks;
using DeltaEngine.Platforms;
using NUnit.Framework;

namespace Snake.Tests
{
	public class SnakeGameTests : TestWithMocksOrVisually
	{
		[SetUp]
		public void Init()
		{
			gridSize = 25;
			blockSize = 1.0f / gridSize;
			moveSpeed = 0.15f;
		}

		private float blockSize;
		private int gridSize;
		private float moveSpeed;

		[Test, CloseAfterFirstFrame]
		public void StartGame()
		{
			new Game(Resolve<Window>());
		}

		[Test, CloseAfterFirstFrame]
		public void RespawnChunkIfCollidingWithSnake()
		{
			var game = new Game(Resolve<Window>());
			game.StartGame();
			game.Chunk.DrawArea = game.Snake.Get<Body>().BodyParts[0].DrawArea;
			Assert.IsTrue(game.Chunk.IsCollidingWithSnake(game.Snake.Get<Body>().BodyParts));
			game.RespawnChunk();
			Assert.IsFalse(game.Chunk.IsCollidingWithSnake(game.Snake.Get<Body>().BodyParts));
		}

		[Test, CloseAfterFirstFrame]
		public void SnakeEatsChunk()
		{
			var game = new Game(Resolve<Window>());
			game.StartGame();
			var snakeHead = game.Snake.Get<Body>().BodyParts[0].DrawArea;
			var direction = game.Snake.Get<Body>().Direction;
			var originalLength = game.Snake.Get<Body>().BodyParts.Count;
			game.Chunk.DrawArea =
				new Rectangle(new Vector2D(snakeHead.Left + direction.X, snakeHead.Top + direction.Y),
					new Size(blockSize));
			game.MoveUp();
			AdvanceTimeAndUpdateEntities(moveSpeed);
			Assert.AreEqual(originalLength + 1, game.Snake.Get<Body>().BodyParts.Count);
		}

		[Test, CloseAfterFirstFrame]
		public void DisplayGameOver()
		{
			var game = new Game(Resolve<Window>());
			game.StartGame();
			game.Reset();
		}

		[Test, CloseAfterFirstFrame]
		public void QuitGracefullyDisposes()
		{
			var game = new Game(Resolve<Window>());
			Assert.DoesNotThrow(game.CloseGame);
		}

		[Test, CloseAfterFirstFrame]
		public void RestartGameInitializesAnew()
		{
			var game = new Game(Resolve<Window>());
			game.StartGame();
			AdvanceTimeAndUpdateEntities();
			game.Reset();
			var keyboard = Resolve<Keyboard>();
			if (keyboard.GetType() != typeof(MockKeyboard))
				return; //ncrunch: no coverage (This is for NOT ncrunch...)
			var mockKeyboard = (MockKeyboard)keyboard;
			mockKeyboard.SetKeyboardState(Key.Y, State.Pressing);
			Assert.DoesNotThrow(() => AdvanceTimeAndUpdateEntities());
		}

		[Test, CloseAfterFirstFrame]
		public void MoveByTouchPositionLeft()
		{
			var game = new Game(Resolve<Window>());
			game.StartGame();
			game.MoveAccordingToTouchPosition(new Vector2D(-1, 0.5f));
			Assert.AreEqual(new Vector2D(-(1.0f / 25.0f), 0), game.Snake.Get<Body>().Direction);
		}

		[Test, CloseAfterFirstFrame]
		public void MoveByTouchPositionRight()
		{
			var game = new Game(Resolve<Window>());
			game.StartGame();
			game.MoveAccordingToTouchPosition(new Vector2D(1, 0.5f));
			Assert.AreEqual(new Vector2D(1.0f / 25.0f, 0), game.Snake.Get<Body>().Direction);
		}

		[Test, CloseAfterFirstFrame]
		public void ResizeWindow()
		{
			var window = Resolve<Window>();
			if (window.GetType() != typeof(MockWindow))
				return; //ncrunch: no coverage (security measure for non-mock, would crash)
			var mockWindow = (MockWindow)window;
			var game = new Game(mockWindow);
			mockWindow.ViewportPixelSize = new Size(200.0f, 100.0f);
			Assert.AreEqual(0.5f, game.screenSpace.Zoom);
		}

		[Test, CloseAfterFirstFrame]
		public void GoingBackToMenuDisposesOfSnake()
		{
			var game = new Game(Resolve<Window>());
			game.StartGame();
			AdvanceTimeAndUpdateEntities();
			game.Reset();
			var keyboard = Resolve<Keyboard>();
			if (keyboard.GetType() != typeof(MockKeyboard))
				return; //ncrunch: no coverage (This is for NOT ncrunch...)
			var mockKeyboard = (MockKeyboard)keyboard;
			mockKeyboard.SetKeyboardState(Key.N, State.Pressing);
			Assert.DoesNotThrow(() => AdvanceTimeAndUpdateEntities());
			Assert.AreEqual(0, EntitiesRunner.Current.GetEntitiesOfType<Snake>().Count);
		}
	}
}