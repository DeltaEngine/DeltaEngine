using DeltaEngine.Commands;
using DeltaEngine.Content;
using DeltaEngine.Datatypes;
using DeltaEngine.Entities;
using DeltaEngine.Input;
using DeltaEngine.Rendering3D;
using DeltaEngine.Rendering3D.Cameras;
using DeltaEngine.Rendering3D.Shapes;

namespace DeltaEngine.Editor.ContentManager.Previewers
{
	public class ModelPreviewer : ContentPreview
	{
		protected override void Init()
		{
			Initialize3DCamera();
			SetupKeboardCommands();
		}

		private void Initialize3DCamera()
		{
			camera = Camera.Use<LookAtCamera>();
			InitializeCameraPosition();
		}

		private LookAtCamera camera;

		private void InitializeCameraPosition()
		{
			camera.Position = new Vector3D(-2.0f, -2.0f, 2.0f);
			camera.Target = new Vector3D(0, 0, 1);
		}

		private void SetupKeboardCommands()
		{
			const string GridCommand = "ShowHideGrid";
			Command.Register(GridCommand, new KeyTrigger(Key.G));
			new Command(GridCommand, ToggleGridVisibility);
			const string CameraResetCommand = "CameraReset";
			Command.Register(CameraResetCommand, new KeyTrigger(Key.R));
			new Command(CameraResetCommand, InitializeCameraPosition);
			const string CameraMoveUpCommand = "CameraMoveUp";
			Command.Register(CameraMoveUpCommand, new KeyTrigger(Key.Q, State.Pressed));
			new Command(CameraMoveUpCommand, MoveCameraUp);
			const string CameraMoveDownCommand = "CameraMoveDown";
			Command.Register(CameraMoveDownCommand, new KeyTrigger(Key.E, State.Pressed));
			new Command(CameraMoveDownCommand, MoveCameraDown);
		}

		private void ToggleGridVisibility()
		{
			groundGrid.IsVisible = !groundGrid.IsVisible;
		}

		private Grid3D groundGrid;

		private void MoveCameraUp()
		{
			float zMovement = Time.Delta * 2;
			camera.Position = ChangeZ(camera.Position, zMovement);
			camera.Target = ChangeZ(camera.Target, zMovement);
		}

		private static Vector3D ChangeZ(Vector3D position, float zMovementChange)
		{
			return new Vector3D(position.X, position.Y, position.Z + zMovementChange);
		}

		private void MoveCameraDown()
		{
			float zMovement = Time.Delta * -2;
			camera.Position = ChangeZ(camera.Position, zMovement);
			camera.Target = ChangeZ(camera.Target, zMovement);
		}

		protected override void Preview(string contentName)
		{
			groundGrid = new Grid3D(new Size(10));
			var modeldata = ContentLoader.Load<ModelData>(contentName);
			new Model(modeldata, new Vector3D(0, 0, 0));
		}
	}
}