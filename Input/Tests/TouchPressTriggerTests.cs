using DeltaEngine.Commands;
using DeltaEngine.Datatypes;
using DeltaEngine.Platforms;
using DeltaEngine.Rendering2D.Fonts;
using DeltaEngine.Rendering2D.Shapes;
using NUnit.Framework;

namespace DeltaEngine.Input.Tests
{
	public class TouchPressTriggerTests : TestWithMocksOrVisually
	{
		//ncrunch: no coverage start
		[Test, Category("Slow")]
		public void ShowRedCircleOnTouch()
		{
			new FontText(Font.Default, "Touch screen to show red circle", Rectangle.One);
			var ellipse = new Ellipse(new Rectangle(0.1f, 0.1f, 0.1f, 0.1f), Color.Red);
			new Command(() => ellipse.Center = Vector2D.Half).Add(new TouchPressTrigger(State.Pressed));
			new Command(() => ellipse.Center = Vector2D.Zero).Add(new TouchPressTrigger(State.Released));
		} //ncrunch: no coverage end

		[Test, CloseAfterFirstFrame]
		public void Create()
		{
			var trigger = new TouchPressTrigger(State.Pressed);
			Assert.AreEqual(State.Pressed, trigger.State);
		}

		[Test, CloseAfterFirstFrame]
		public void CreateFromString()
		{
			var trigger = new TouchPressTrigger("Pressed");
			Assert.AreEqual(State.Pressed, trigger.State);
		}
	}
}