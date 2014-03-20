using DeltaEngine.Commands;
using DeltaEngine.Platforms;
using DeltaEngine.Datatypes;
using DeltaEngine.Input;
using DeltaEngine.Rendering3D;
using DeltaEngine.Rendering3D.Cameras;
using DeltaEngine.Rendering3D.Shapes;
using DeltaEngine.ScreenSpaces;
using NUnit.Framework;
using DeltaEngine.Input.Mocks;

namespace CreepyTowers.Tests
{
	class GameCameraTests: TestWithMocksOrVisually
	{
		[SetUp]
		public void CreateGameCamera()
		{
			Command.Register("ViewPanning", new Trigger[] { new MouseDragTrigger(MouseButton.Middle) });
			Command.Register("ViewZooming", new Trigger[] { new MouseZoomTrigger() });
			Command.Register("TurnViewRight", new Trigger[]{new KeyTrigger(Key.E, State.Pressed)});
			Command.Register("TurnViewLeft", new Trigger[]{new KeyTrigger(Key.Q, State.Pressed)});
			gameCamera = new GameCamera(0.1f, 2.0f);
			new Grid3D(new Size(10.0f, 10.0f));
			gameCamera.AllowedMovementRect = new Rectangle(-4.0f, -4.0f, 8.0f, 8.0f);
		}

		private GameCamera gameCamera;

		[Test]
		public void RenderCubeUsingGameCamera()
		{
			var cubeMesh = new BoxMesh(Vector3D.One, Color.DarkGreen);
			new Model(new ModelData(cubeMesh), new Vector3D(0.0f, 0.0f, 0.0f));
			new Line3D(2 * Vector3D.UnitX, -3 * Vector3D.UnitY, Color.Red);
		}

		[Test, CloseAfterFirstFrame]
		public void MouseDragWillDragCamera()
		{
			if(!IsMockResolver)
				return; //ncrunch: no coverage
			MockMouse mouse = (MockMouse)Resolve<Mouse>();
			var originalPosition = gameCamera.Position;
			bool dragInvoked = false;
			new Command("ViewPanning", () => dragInvoked = true);
			mouse.SetNativePosition(ScreenSpace.Current.Viewport.TopLeft + new Vector2D(0.1f, 0.1f));
			mouse.SetButtonState(MouseButton.Middle, State.Pressing);
			AdvanceTimeAndUpdateEntities();
			mouse.SetNativePosition(ScreenSpace.Current.Viewport.TopRight + new Vector2D(-0.1f, 0.1f));
			mouse.SetButtonState(MouseButton.Middle, State.Pressed);
			AdvanceTimeAndUpdateEntities();
			Assert.IsTrue(dragInvoked);
			Assert.AreNotEqual(originalPosition, gameCamera.Position);
			Assert.AreEqual(135.0f, (gameCamera.Position - originalPosition).Angle(Vector3D.UnitY));
		}

		[Test, CloseAfterFirstFrame]
		public void MouseZoomWillZoomCamera()
		{
			if (!IsMockResolver)
				return; //ncrunch: no coverage
			MockMouse mouse = (MockMouse)Resolve<Mouse>();
			var originalZoom = gameCamera.ZoomLevel;
			mouse.ScrollUp();
			AdvanceTimeAndUpdateEntities();
			Assert.AreNotEqual(originalZoom, gameCamera.ZoomLevel);
		}

		[Test, CloseAfterFirstFrame]
		public void CannotZoomCloserThanMaximumValue()
		{
			if (!IsMockResolver)
				return; //ncrunch: no coverage
			MockMouse mouse = (MockMouse)Resolve<Mouse>();
			gameCamera.ZoomLevel = gameCamera.MaxZoom;
			var originalZoom = gameCamera.ZoomLevel;
			mouse.ScrollUp();
			AdvanceTimeAndUpdateEntities();
			Assert.AreEqual(originalZoom, gameCamera.ZoomLevel);
		}

		[Test, CloseAfterFirstFrame]
		public void CannotZoomFurtherThanMinimumValue()
		{
			if (!IsMockResolver)
				return; //ncrunch: no coverage
			MockMouse mouse = (MockMouse)Resolve<Mouse>();
			gameCamera.ZoomLevel = gameCamera.MinZoom;
			var originalZoom = gameCamera.ZoomLevel;
			mouse.ScrollDown();
			AdvanceTimeAndUpdateEntities();
			Assert.AreEqual(originalZoom, gameCamera.ZoomLevel);
		}

		[Test, CloseAfterFirstFrame]
		public void DisposeWillCorrectlyDisposeCamera()
		{
			gameCamera.Dispose();
			//LookAtCamera is the default Camera
			Assert.AreEqual(typeof(OrthoCamera), Camera.Current.GetType());
		}

		[Test,CloseAfterFirstFrame]
		public void CameraCannotMoveOutOfItsRectangle()
		{
			gameCamera.AllowedMovementRect = Rectangle.Zero;
			var originalPosition = gameCamera.Position;
			if (!IsMockResolver)
				return; //ncrunch: no coverage
			MockMouse mouse = (MockMouse)Resolve<Mouse>();
			mouse.SetNativePosition(ScreenSpace.Current.Viewport.TopLeft + new Vector2D(0.1f, 0.1f));
			mouse.SetButtonState(MouseButton.Middle, State.Pressing);
			AdvanceTimeAndUpdateEntities();
			mouse.SetNativePosition(ScreenSpace.Current.Viewport.TopRight + new Vector2D(-0.1f, 0.1f));
			mouse.SetButtonState(MouseButton.Middle, State.Pressed);
			AdvanceTimeAndUpdateEntities();
			Assert.AreEqual(originalPosition, gameCamera.Position);
		}
	}
}
