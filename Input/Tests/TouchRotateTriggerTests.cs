using DeltaEngine.Commands;
using DeltaEngine.Datatypes;
using DeltaEngine.Input.Mocks;
using DeltaEngine.Platforms;
using DeltaEngine.Rendering2D.Fonts;
using DeltaEngine.Rendering2D.Shapes;
using NUnit.Framework;

namespace DeltaEngine.Input.Tests
{
	public class TouchRotateTriggerTests : TestWithMocksOrVisually
	{
		[Test]
		public void ShowRedSquare()
		{
			new FontText(Font.Default, "Pinch screen to show red circle", Rectangle.One);
			var rect = new FilledRect(new Rectangle(0.1f, 0.1f, 0.1f, 0.1f), Color.Red);
			var trigger = new TouchRotateTrigger();
			var touch = Resolve<Touch>();
			new Command(() =>
			//ncrunch: no coverage start
			{
				rect.Center = touch.GetPosition(0);
				rect.RotationCenter = rect.Center;
				rect.Rotation = trigger.Angle;
			}).Add(trigger);
			//ncrunch: no coverage end
		}

		[Test, CloseAfterFirstFrame]
		public void Angle()
		{
			var touch = Resolve<Touch>() as MockTouch;
			if (touch == null)
				return; //ncrunch: no coverage
			var trigger = new TouchRotateTrigger();
				touch.SetTouchState(0, State.Pressing, new Vector2D(0.5f, 0.1f));
			touch.SetTouchState(1, State.Pressed, new Vector2D(0.5f, 0.7f));
			touch.Update(new[] { trigger });
			Assert.AreEqual(0f, trigger.Angle);
			touch.SetTouchState(0, State.Pressing, new Vector2D(0.1f, 0.5f));
			touch.SetTouchState(1, State.Pressed, new Vector2D(0.7f, 0.5f));
			touch.Update(new[] { trigger });
			Assert.AreEqual(1.57079637f, trigger.Angle);
			touch.SetTouchState(0, State.Pressing, new Vector2D(0.5f, 0.7f));
			touch.SetTouchState(1, State.Pressed, new Vector2D(0.5f, 0.1f));
			touch.Update(new[] { trigger });
			Assert.AreEqual(3.14159274f, trigger.Angle);
			touch.SetTouchState(0, State.Pressing, new Vector2D(0.7f, 0.5f));
			touch.SetTouchState(1, State.Pressed, new Vector2D(0.1f, 0.5f));
			touch.Update(new[] { trigger });
			Assert.AreEqual(4.71238899f, trigger.Angle);
		}

		[Test, CloseAfterFirstFrame]
		public void Creation()
		{
			Assert.DoesNotThrow(() => new TouchRotateTrigger(""));
			Assert.DoesNotThrow(() => new TouchRotateTrigger(null));
			Assert.Throws<TouchRotateTrigger.TouchRotateTriggerHasNoParameters>(
				() => new TouchRotateTrigger("Left"));
		}
	}
}