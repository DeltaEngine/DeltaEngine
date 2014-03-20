using DeltaEngine.Commands;
using DeltaEngine.Datatypes;
using DeltaEngine.Input.Mocks;
using DeltaEngine.Platforms;
using DeltaEngine.Rendering2D.Shapes;
using NUnit.Framework;

namespace DeltaEngine.Input.Tests
{
	public class MouseZoomTriggerTests : TestWithMocksOrVisually
	{
		[Test]
		public void ResizeEllipseByZoomTrigger()
		{
			var ellipse = new Ellipse(Vector2D.Half, 0.1f, 0.1f, Color.Red);
			new Command(zoomAmount => { ellipse.Radius += zoomAmount * 0.02f; }).Add(
				new MouseZoomTrigger());
		}

		[Test, CloseAfterFirstFrame]
		public void EmptyMouseZoomTriggerShouldDoNothing()
		{
			Assert.DoesNotThrow(() =>new MouseZoomTrigger(""));
		}

		[Test, CloseAfterFirstFrame]
		public void UsingParametersForMouseZoomTriggerShouldThrowException()
		{
			Assert.Throws<MouseZoomTrigger.MouseZoomTriggerHasNoParameters>(
				() => new MouseZoomTrigger("DeltaEngine"));
		}

		[Test, CloseAfterFirstFrame]
		public void MouseWheelZoomUp()
		{
			var mouse = Resolve<Mouse>() as MockMouse;
			if (mouse == null)
				return; //ncrunch: no coverage
			bool isZoomed = false;
			new Command((float zoomAmount) => isZoomed = true).Add(new MouseZoomTrigger());
			mouse.ScrollUp();
			AdvanceTimeAndUpdateEntities();
			Assert.IsTrue(isZoomed);
		}

		[Test, CloseAfterFirstFrame]
		public void MouseWheelZoomDown()
		{
			var mouse = Resolve<Mouse>() as MockMouse;
			if (mouse == null)
				return; //ncrunch: no coverage
			bool isZoomed = false;
			new Command((float zoomAmount) => isZoomed = true).Add(new MouseZoomTrigger());
			mouse.ScrollDown();
			AdvanceTimeAndUpdateEntities();
			Assert.IsTrue(isZoomed);
		}

		[Test, CloseAfterFirstFrame]
		public void MouseWheelZoomUsingCommandName()
		{
			var mouse = Resolve<Mouse>() as MockMouse;
			if (mouse == null)
				return; //ncrunch: no coverage
			bool isZoomed = false;
			new Command(Command.Zoom, (float zoomAmount) => isZoomed = true);
			mouse.ScrollUp();
			AdvanceTimeAndUpdateEntities();
			Assert.IsTrue(isZoomed);
		}
	}
}