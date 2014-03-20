using System;
using DeltaEngine.Commands;
using DeltaEngine.Datatypes;
using DeltaEngine.Extensions;
using DeltaEngine.Input;
using DeltaEngine.Platforms;
using DeltaEngine.Rendering2D.Fonts;
using DeltaEngine.Rendering3D.Cameras;
using DeltaEngine.Rendering3D.Shapes;
using NUnit.Framework;

namespace DeltaEngine.Rendering3D.Tests.Cameras
{
	public class FreeCameraTests : TestWithMocksOrVisually
	{
		[SetUp]
		public void SetUpCamera()
		{
			camera = Camera.Use<FreeCamera>();
			camera.Position = new Vector3D(0.0f, 5.0f, 5.0f);
			text = new FontText(Font.Default, "Yaw: 0 Pitch: 0 Roll: 0",
				new Rectangle(0.0f, 0.3f, 1.0f, 0.1f));
		}

		private FreeCamera camera;
		private FontText text;

		[Test]
		public void MoveFreeCamera()
		{
			new Grid3D(new Size(100));
			RegisterCommand(Key.J, () => camera.MoveLeft(MoveDistance));
			RegisterCommand(Key.L, () => camera.MoveRight(MoveDistance));
			RegisterCommand(Key.I, () => camera.MoveUp(MoveDistance));
			RegisterCommand(Key.K, () => camera.MoveDown(MoveDistance));
			RegisterCommand(Key.U, () => camera.MoveForward(MoveDistance));
			RegisterCommand(Key.O, () => camera.MoveBackward(MoveDistance));
			RegisterCommand(Key.A, () => camera.AdjustYaw(RotationAngle));
			RegisterCommand(Key.D, () => camera.AdjustYaw(-RotationAngle));
			RegisterCommand(Key.W, () => camera.AdjustPitch(RotationAngle));
			RegisterCommand(Key.S, () => camera.AdjustPitch(-RotationAngle));
			RegisterCommand(Key.Q, () => camera.AdjustRoll(-RotationAngle));
			RegisterCommand(Key.E, () => camera.AdjustRoll(RotationAngle));
		}

		private const float MoveDistance = 0.1f;
		private const int RotationAngle = 1;

		private void RegisterCommand(Key key, Action action)
		{
			new Command(UpdateText).Add(new KeyTrigger(key, State.Pressed));
			new Command(action).Add(new KeyTrigger(key, State.Pressed));
		}

		//ncrunch: no coverage start
		private void UpdateText()
		{
			EulerAngles euler = camera.Rotation.ToEuler();
			text.Text = "Yaw: " + euler.Yaw.ToInvariantString("0") + " Pitch: " +
				euler.Pitch.ToInvariantString("0") + " Roll: " + euler.Roll.ToInvariantString("0");
		}

		//ncrunch: no coverage end

		[Test, CloseAfterFirstFrame]
		public void Move()
		{
			camera.Move(Vector3D.UnitX, 1.0f);
			Assert.AreEqual(new Vector3D(1.0f, 5.0f, 5.0f), camera.Position);
		}

		[Test, CloseAfterFirstFrame]
		public void AdjustPitch()
		{
			camera.AdjustPitch(45.0f);
			Assert.AreEqual(new EulerAngles(45.0f, 0.0f, 0.0f), camera.Rotation.ToEuler());
		}

		[Test, CloseAfterFirstFrame]
		public void AdjustYaw()
		{
			camera.AdjustYaw(-45.0f);
			Assert.AreEqual(new EulerAngles(0.0f, -45.0f, 0.0f), camera.Rotation.ToEuler());
		}

		[Test, CloseAfterFirstFrame]
		public void AdjustRoll()
		{
			camera.AdjustRoll(90.0f);
			Assert.AreEqual(new EulerAngles(0.0f, 0.0f, 90.0f), camera.Rotation.ToEuler());
		}

		[Test, CloseAfterFirstFrame]
		public void RotateThenMove()
		{
			camera.Rotate(Vector3D.UnitZ, 90.0f);
			camera.Move(Vector3D.UnitX, 1.0f);
			Assert.IsTrue(camera.Position.IsNearlyEqual(new Vector3D(0.0f, 6.0f, 5.0f)));
		}

		[Test, CloseAfterFirstFrame]
		public void MoveUp()
		{
			camera.MoveUp(10.0f);
			Assert.AreEqual(new Vector3D(0.0f, 15.0f, 5.0f), camera.Position);
		}

		[Test, CloseAfterFirstFrame]
		public void MoveDown()
		{
			camera.MoveDown(10.0f);
			Assert.AreEqual(new Vector3D(0.0f, -5.0f, 5.0f), camera.Position);
		}

		[Test, CloseAfterFirstFrame]
		public void MoveLeft()
		{
			camera.MoveLeft(10.0f);
			Assert.AreEqual(new Vector3D(-10.0f, 5.0f, 5.0f), camera.Position);
		}

		[Test, CloseAfterFirstFrame]
		public void MoveRight()
		{
			camera.MoveRight(10.0f);
			Assert.AreEqual(new Vector3D(10.0f, 5.0f, 5.0f), camera.Position);
		}

		[Test, CloseAfterFirstFrame]
		public void MoveForward()
		{
			camera.MoveForward(10.0f);
			Assert.AreEqual(new Vector3D(0.0f, 5.0f, -5.0f), camera.Position);
		}

		[Test, CloseAfterFirstFrame]
		public void MoveBackward()
		{
			camera.MoveBackward(10.0f);
			Assert.AreEqual(new Vector3D(0.0f, 5.0f, 15.0f), camera.Position);
		}
	}
}