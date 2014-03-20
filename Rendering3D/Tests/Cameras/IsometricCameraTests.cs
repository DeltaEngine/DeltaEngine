using System;
using DeltaEngine.Commands;
using DeltaEngine.Core;
using DeltaEngine.Datatypes;
using DeltaEngine.Graphics;
using DeltaEngine.Input;
using DeltaEngine.Platforms;
using DeltaEngine.Rendering3D.Cameras;
using DeltaEngine.Rendering3D.Shapes;
using NUnit.Framework;

namespace DeltaEngine.Rendering3D.Tests.Cameras
{
	public class IsometricCameraTests : TestWithMocksOrVisually
	{
		[SetUp]
		public void SetUp()
		{
			camera = Camera.Use<IsometricCamera>(Vector3D.UnitY);
			CreateGridFacingCamera();
		}

		private IsometricCamera camera;

		private static void CreateGridFacingCamera()
		{
			const int Distance = 50;
			const int Min = -1000;
			const int Max = 1000;
			const int Step = 100;
			for (float i = Min; i <= Max; i += Step)
			{
				new Line3D(new Vector3D(Min, Distance, i), new Vector3D(Max, Distance, i), Color.White);
				new Line3D(new Vector3D(i, Distance, Min), new Vector3D(i, Distance, Max), Color.White);
			}
		}

		[Test]
		public void MoveIsometricCamera()
		{
			RegisterCommand(Key.J, () => camera.MoveLeft(MoveDistance));
			RegisterCommand(Key.L, () => camera.MoveRight(MoveDistance));
			RegisterCommand(Key.I, () => camera.MoveUp(MoveDistance));
			RegisterCommand(Key.K, () => camera.MoveDown(MoveDistance));
			RegisterCommand(Key.U, () => camera.Zoom(ZoomAmount));
			RegisterCommand(Key.O, () => camera.Zoom(1 / ZoomAmount));
		}

		private const int MoveDistance = 5;
		private const float ZoomAmount = 1.05f;

		private static void RegisterCommand(Key key, Action action)
		{
			new Command(action).Add(new KeyTrigger(key, State.Pressed));
		}

		[Test, CloseAfterFirstFrame]
		public void SettingPositionMovesTarget()
		{
			camera.Position = new Vector3D(10.0f, 10.0f, 10.0f);
			Assert.AreEqual(new Vector3D(10.0f, 11.0f, 10.0f), camera.Target);
		}

		[Test, CloseAfterFirstFrame]
		public void SettingTargetMovesPosition()
		{
			camera.Target = new Vector3D(10.0f, 10.0f, 10.0f);
			Assert.AreEqual(new Vector3D(10.0f, 9.0f, 10.0f), camera.Position);
		}

		[Test, CloseAfterFirstFrame]
		public void MoveLeft()
		{
			camera.MoveLeft(1.0f);
			Assert.AreEqual(new Vector3D(-1.0f, -1.0f, 0.0f), camera.Position);
		}

		[Test, CloseAfterFirstFrame]
		public void MoveRight()
		{
			camera.MoveRight(1.0f);
			Assert.AreEqual(new Vector3D(1.0f, -1.0f, 0.0f), camera.Position);
		}

		[Test, CloseAfterFirstFrame]
		public void MoveUp()
		{
			camera.MoveUp(1.0f);
			Assert.AreEqual(new Vector3D(0.0f, -1.0f, 1.0f), camera.Position);
		}

		[Test, CloseAfterFirstFrame]
		public void MoveDown()
		{
			camera.MoveDown(1.0f);
			Assert.AreEqual(new Vector3D(0.0f, -1.0f, -1.0f), camera.Position);
		}

		[Test, CloseAfterFirstFrame]
		public void Zoom()
		{
			camera.Zoom(4.0f);
			Assert.AreEqual(0.25f, camera.ZoomScale);
		}

		[Test, CloseAfterFirstFrame]
		public void CreateWithLookDirection()
		{
			var lookDirection = Vector3D.UnitY;
			camera = new IsometricCamera(Resolve<Device>(), Resolve<Window>(), lookDirection);
			Assert.AreEqual(-lookDirection, camera.Position);
			Assert.AreEqual(Vector3D.Zero, camera.Target);
		}
	}
}