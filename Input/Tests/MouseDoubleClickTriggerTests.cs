using DeltaEngine.Commands;
using DeltaEngine.Core;
using DeltaEngine.Datatypes;
using DeltaEngine.Platforms;
using DeltaEngine.Rendering2D.Fonts;
using NUnit.Framework;

namespace DeltaEngine.Input.Tests
{
	public class MouseDoubleClickTriggerTests : TestWithMocksOrVisually
	{
		[Test]
		public void DoubleClickToCloseWindow()
		{
			new FontText(Font.Default, "Double Click Mouse Button to close window", Rectangle.One);
			new Command(() => Resolve<Window>().CloseAfterFrame()).Add(new MouseDoubleClickTrigger());
		}

		[Test, CloseAfterFirstFrame]
		public void CreateFromString()
		{
			var trigger = new MouseDoubleClickTrigger("Right");
			Assert.AreEqual(MouseButton.Right, trigger.Button);
			trigger = new MouseDoubleClickTrigger("");
			Assert.AreEqual(MouseButton.Left, trigger.Button);
		}
	}
}