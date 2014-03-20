using DeltaEngine.Core;
using DeltaEngine.Datatypes;
using DeltaEngine.Input;
using DeltaEngine.Input.Mocks;
using DeltaEngine.Platforms;
using DeltaEngine.ScreenSpaces;
using NUnit.Framework;

namespace Breakout.Tests
{
	public class MainMenuTests : TestWithMocksOrVisually
	{
		[SetUp, CloseAfterFirstFrame]
		public void Init()
		{
			Resolve<Game>();
			menu = Resolve<MainMenu>();
		}

		private MainMenu menu;

		[Test]
		public void StartGame()
		{
			if (!IsMockResolver)
				return; //ncrunch: no coverage
			bool started = false;
			menu.InitGame += () => started = true;
			var mouse = Resolve<MockMouse>();
			ClickAtPosition(mouse, new Vector2D(0.31f, 0.31f));
			Assert.IsTrue(started);
		}

		private void ClickAtPosition(MockMouse mouse, Vector2D position)
		{
			SetStateAndPosition(mouse, State.Pressing, position);
			SetStateAndPosition(mouse, State.Releasing, position);
		}

		private void SetStateAndPosition(MockMouse mouse, State state, Vector2D position)
		{
			mouse.SetNativePosition(position);
			mouse.SetButtonState(MouseButton.Left, state);
			AdvanceTimeAndUpdateEntities();
		}

		[Test]
		public void QuitGame()
		{
			if (!IsMockResolver)
				return; //ncrunch: no coverage
			bool quit = false;
			menu.QuitGame += () => quit = true;
			var mouse = Resolve<MockMouse>();
			ClickAtPosition(mouse, new Vector2D(0.31f, 0.73f));
			Assert.IsTrue(quit);
		}

		//ncrunch: no coverage start
		[Test, Ignore]
		public void HowToPlay()
		{
			if (!IsMockResolver)
				return; //ncrunch: no coverage
			var mouse = Resolve<MockMouse>();
			ClickAtPosition(mouse, new Vector2D(0.31f, 0.45f));
			Assert.IsFalse(menu.Controls[0].IsVisible);
			ClickAtPosition(mouse, new Vector2D(0.31f, ScreenSpace.Current.Bottom - 0.19f));
			Assert.IsTrue(menu.Controls[0].IsVisible);
		}

		[Test, Ignore]
		public void Options()
		{
			if (!IsMockResolver)
				return; //ncrunch: no coverage
			var mouse = Resolve<MockMouse>();
			Settings.Current.MusicVolume = 0.5f;
			Settings.Current.SoundVolume = 0.5f;
			ClickAtPosition(mouse, new Vector2D(0.31f, 0.59f));
			Assert.IsFalse(menu.Controls[0].IsVisible);
			bool changed = false;
			MainMenu.SettingsChanged += () => changed = true;
			DragMouse(mouse, 0.81f, ScreenSpace.Current.Viewport.Top + 0.46f);
			Assert.IsTrue(changed);
			changed = false;
			DragMouse(mouse, 0.81f, ScreenSpace.Current.Viewport.Top + 0.6f);
			Assert.IsTrue(changed);
			ClickAtPosition(mouse, new Vector2D(0.31f, ScreenSpace.Current.Bottom - 0.19f));
			Assert.IsTrue(menu.Controls[0].IsVisible);
		}

		private void DragMouse(MockMouse mouse, float x, float y)
		{
			SetStateAndPosition(mouse, State.Pressing, new Vector2D(x, y));
			SetStateAndPosition(mouse, State.Pressed, new Vector2D(x - 0.1f, y));
			SetStateAndPosition(mouse, State.Releasing, new Vector2D(x - 0.2f, y));
		}
	}
}