using DeltaEngine.Commands;
using DeltaEngine.Core;
using DeltaEngine.Datatypes;
using DeltaEngine.Platforms;
using DeltaEngine.Rendering2D.Fonts;
using DeltaEngine.Rendering2D.Shapes;
using NUnit.Framework;

namespace DeltaEngine.Input.Tests
{
	public class KeyTriggerTests : TestWithMocksOrVisually
	{
		[Test]
		public void PressEscapeToCloseWindow()
		{
			new FontText(Font.Default, "Press ESC to close the window", Rectangle.One);
			new Command(() => Resolve<Window>().CloseAfterFrame()).Add(new KeyTrigger(Key.Escape,
				State.Pressed));
		}

		[Test]
		public void PressEscapeToCloseWindowViaRegisteredCommands()
		{
			new FontText(Font.Default, "Press ESC to close the window", Rectangle.One);
			Command.Register("Exit", new KeyTrigger(Key.Escape, State.Pressed));
			new Command("Exit", () => Resolve<Window>().CloseAfterFrame());
		}

		[Test]
		public void PressCursorKeysToShowCircles()
		{
			var centers = new[]
			{
				new Vector2D(0.5f, 0.4f), new Vector2D(0.5f, 0.6f), new Vector2D(0.3f, 0.6f),
				new Vector2D(0.7f, 0.6f)
			};
			var size = new Size(0.1f, 0.1f);
			CreateFontTexts(centers, size);
			AddCirclesAndInputCommands(centers, size);
		}

		private static void CreateFontTexts(Vector2D[] centers, Size size)
		{
			new FontText(Font.Default, "Up", Rectangle.FromCenter(centers[0], size));
			new FontText(Font.Default, "Down", Rectangle.FromCenter(centers[1], size));
			new FontText(Font.Default, "Left", Rectangle.FromCenter(centers[2], size));
			new FontText(Font.Default, "Right", Rectangle.FromCenter(centers[3], size));
		}

		private static void AddCirclesAndInputCommands(Vector2D[] centers, Size size)
		{
			var up = new Ellipse(centers[0], size.Width, size.Height, Color.Orange);
			var down = new Ellipse(centers[1], size.Width, size.Height, Color.Orange);
			var left = new Ellipse(centers[2], size.Width, size.Height, Color.Orange);
			var right = new Ellipse(centers[3], size.Width, size.Height, Color.Orange);
			new Command(() => up.IsVisible = true).Add(new KeyTrigger(Key.CursorUp, State.Pressed));
			new Command(() => up.IsVisible = false).Add(new KeyTrigger(Key.CursorUp, State.Released));
			new Command(() => down.IsVisible = true).Add(new KeyTrigger(Key.CursorDown, State.Pressed));
			new Command(() => down.IsVisible = false).Add(new KeyTrigger(Key.CursorDown, State.Released));
			new Command(() => left.IsVisible = true).Add(new KeyTrigger(Key.CursorLeft, State.Pressed));
			new Command(() => left.IsVisible = false).Add(new KeyTrigger(Key.CursorLeft, State.Released));
			new Command(() => right.IsVisible = true).Add(new KeyTrigger(Key.CursorRight, State.Pressed));
			new Command(() => right.IsVisible = false).Add(new KeyTrigger(Key.CursorRight,
				State.Released));
		}

		[Test, CloseAfterFirstFrame]
		public void Create()
		{
			var trigger = new KeyTrigger(Key.Z, State.Pressed);
			Assert.AreEqual(Key.Z, trigger.Key);
			Assert.AreEqual(State.Pressed, trigger.State);
		}

		[Test, CloseAfterFirstFrame]
		public void CreateFromString()
		{
			var trigger = new KeyTrigger("Z Pressed");
			Assert.AreEqual(Key.Z, trigger.Key);
			Assert.AreEqual(State.Pressed, trigger.State);
			Assert.Throws<KeyTrigger.CannotCreateKeyTriggerWithoutKey>(() => new KeyTrigger(""));
		}

		[Test]
		public void UseMoveDirectlyCommandToMoveCircleAround()
		{
			var circle = new Ellipse(Vector2D.Half, 0.25f, 0.25f, Color.Orange);
			new Command(Command.MoveDirectly, position =>
				circle.DrawArea = circle.DrawArea.Move(position / 24.0f)); //ncrunch: no coverage
		}

		[Test]
		public void ZoomCircleWithPageUpPageDown()
		{
			var circle = new Ellipse(Vector2D.Half, 0.25f, 0.25f, Color.Orange);
			new Command(Command.Zoom, zoom =>
				circle.DrawArea = circle.DrawArea.Increase(new Size(zoom / 24.0f))); //ncrunch: no coverage
		}
	}
}