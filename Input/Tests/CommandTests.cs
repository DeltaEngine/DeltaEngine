using DeltaEngine.Commands;
using DeltaEngine.Core;
using DeltaEngine.Datatypes;
using DeltaEngine.Platforms;
using DeltaEngine.Rendering2D.Shapes;
using NUnit.Framework;

namespace DeltaEngine.Input.Tests
{
	public class CommandTests : TestWithMocksOrVisually
	{
		[SetUp]
		public void GetWindow()
		{
			window = Resolve<Window>();
		}

		private Window window;

		[Test]
		public void CallingExitWillCloseTheProject()
		{
			new Command(Command.Exit, () => window.CloseAfterFrame());
		}

		[Test]
		public void ClickingWillChangeBackgroundColor()
		{
			new Command(Command.Click, () => window.BackgroundColor = Color.GetRandomColor());
		}

		[Test]
		public void MiddleClickingWillChangeBackgroundColor()
		{
			new Command(Command.MiddleClick, () => window.BackgroundColor = Color.GetRandomColor());
		}

		[Test]
		public void RightClickingWillChangeBackgroundColor()
		{
			new Command(Command.RightClick, () => window.BackgroundColor = Color.GetRandomColor());
		}

		[Test]
		public void ZoomWillChangeBackgroundColor()
		{
			new Command(Command.Zoom, () => window.BackgroundColor = Color.GetRandomColor());
		}

		[Test]
		public void MovingARectangleWithTheArrows()
		{
			var rect = new FilledRect(new Rectangle(0.4f, 0.4f, 0.2f, 0.2f), Color.Green);
			new Command(Command.MoveDown, () => rect.DrawArea = rect.DrawArea.Move(0, 0.01f));
			new Command(Command.MoveUp, () => rect.DrawArea = rect.DrawArea.Move(0, -0.01f));
			new Command(Command.MoveLeft, () => rect.DrawArea = rect.DrawArea.Move(-0.01f, 0));
			new Command(Command.MoveRight, () => rect.DrawArea = rect.DrawArea.Move(0.01f, 0));
		}

		[Test]
		public void MovingDirectlyWillChange()
		{
			new Command(Command.MoveDirectly, () => window.BackgroundColor = Color.GetRandomColor());
		}

		[Test]
		public void RotateRectangleWithMouse()
		{
			var rect = new FilledRect(new Rectangle(0.4f, 0.4f, 0.2f, 0.2f), Color.Green);
			new Command(Command.RotateDirectly, delegate(Vector2D point) { rect.Rotation += 1; });
		}

		[Test]
		public void BackWillRemoveRectangle()
		{
			var rect = new FilledRect(new Rectangle(0.4f, 0.4f, 0.2f, 0.2f), Color.Green);
			new Command(Command.Back, () => { rect.IsActive = false; });
		}

		[Test]
		public void TouchToMoveTheRectangleARound()
		{
			var rect = new FilledRect(new Rectangle(0.4f, 0.4f, 0.2f, 0.2f), Color.Green);
			new Command(Command.Drag, point => { rect.Center = point; });
		}

		[Test]
		public void FlickToChangeTheColor()
		{
			new Command(Command.Flick, (Vector2D point) => window.BackgroundColor = Color.GetRandomColor());
		}

		[Test]
		public void PitchToChangeTheScale()
		{
			var rect = new FilledRect(new Rectangle(0.4f, 0.4f, 0.2f, 0.2f), Color.Green);
			new Command(Command.Pinch, point => rect.DrawArea = rect.DrawArea.Reduce((Size)point));
		}

		[Test]
		public void NotMovingWillChangeTheColor()
		{
			new Command(Command.Hold, () => { window.BackgroundColor = Color.GetRandomColor(); });
		}

		[Test]
		public void DoubleCLickingWillChangeTheColor()
		{
			new Command(Command.DoubleClick, () => { window.BackgroundColor = Color.GetRandomColor(); });
		}

		[Test]
		public void UsingRotationOnATouchPadWillRotateTheRectangle()
		{
			var rect = new FilledRect(new Rectangle(0.4f, 0.4f, 0.2f, 0.2f), Color.Green);
			new Command(Command.Rotate, point => rect.Rotation += point.X);
		}

		[Test]
		public void TestPausable()
		{
			var command = new Command(() => { });
			Assert.IsFalse(command.IsPauseable);
		}
	}
}