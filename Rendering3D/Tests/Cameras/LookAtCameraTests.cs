using System.Collections.Generic;
using System.Linq;
using DeltaEngine.Commands;
using DeltaEngine.Datatypes;
using DeltaEngine.Entities;
using DeltaEngine.Input;
using DeltaEngine.Platforms;
using DeltaEngine.Rendering3D.Cameras;
using DeltaEngine.Rendering3D.Shapes;
using NUnit.Framework;

namespace DeltaEngine.Rendering3D.Tests.Cameras
{
	public class LookAtCameraTests : TestWithMocksOrVisually
	{
		[Test, ApproveFirstFrameScreenshot]
		public void RenderGrid()
		{
			CreateLookAtCamera(new Vector3D(0.0f, -5.0f, 5.0f), Vector3D.Zero);
			new Grid3D(new Size(9));
		}

		private static LookAtCamera CreateLookAtCamera(Vector3D position, Vector3D target)
		{
			var camera = Camera.Use<LookAtCamera>();
			camera.Position = position;
			camera.Target = target;
			return camera;
		}

		[Test, CloseAfterFirstFrame]
		public void PositionToTargetDistance()
		{
			var camera = CreateLookAtCamera(Vector3D.UnitZ * 5.0f, Vector3D.Zero);
			Assert.AreEqual(5.0f, camera.Position.Length - camera.Target.Length);
		}

		[Test, CloseAfterFirstFrame]
		public void RotateCamera90DegreesAroundZAxis()
		{
			var camera = CreateLookAtCamera(Vector3D.UnitY, Vector3D.Zero);
			Assert.AreEqual(Vector3D.Zero, camera.YawPitchRoll);
			camera.YawPitchRoll = new Vector3D(90.0f, 0.0f, 0.0f);
			Assert.IsTrue(camera.Position.IsNearlyEqual(-Vector3D.UnitX));
			Assert.AreEqual(Vector3D.Zero, camera.Target);
		}

		[Test, CloseAfterFirstFrame]
		public void LookAtEntity3D()
		{
			var entity = new Entity3D(Vector3D.One * 5.0f, Quaternion.Identity);
			var camera = CreateLookAtCamera(Vector3D.Zero, entity);
			Assert.AreEqual(camera.Target, entity.Position);
		}

		private static LookAtCamera CreateLookAtCamera(Vector3D position, Entity3D target)
		{
			return CreateLookAtCamera(position, target.Position);
		}

		[Test, CloseAfterFirstFrame]
		public void ZoomTowardTheTarget()
		{
			var camera = CreateLookAtCamera(Vector3D.UnitX * 2.0f, Vector3D.Zero);
			camera.Zoom(1.0f);
			Assert.AreEqual(Vector3D.UnitX, camera.Position);
		}

		[Test, CloseAfterFirstFrame]
		public void ZoomOutwardTheTarget()
		{
			var camera = CreateLookAtCamera(Vector3D.UnitX, Vector3D.Zero);
			camera.Zoom(-1.0f);
			Assert.AreEqual(Vector3D.UnitX * 2.0f, camera.Position);
		}

		[Test, CloseAfterFirstFrame]
		public void OverZoomTowardTheTarget()
		{
			var camera = CreateLookAtCamera(Vector3D.UnitX * 3.0f, Vector3D.Zero);
			camera.Zoom(100.0f);
			Assert.IsTrue(camera.Position.IsNearlyEqual(Vector3D.Zero));
		}

		[Test, CloseAfterFirstFrame]
		public void WorldToScreenPoint()
		{
			var camera = CreateLookAtCamera(Vector3D.One, Vector3D.Zero);
			var point = camera.WorldToScreenPoint(Vector3D.Zero);
			Assert.AreEqual(Vector2D.Half, point);
		}

		[Test, CloseAfterFirstFrame]
		public void CenterOfScreenWithLookAtCameraPointsToTarget()
		{
			VerifyScreenCenterIsTarget(new Vector3D(3.0f, 3.0f, 3.0f), new Vector3D(1.0f, 1.0f, 2.0f));
			VerifyScreenCenterIsTarget(new Vector3D(1.0f, 4.0f, 1.5f), new Vector3D(-2.9f, 0.0f, 2.5f));
			VerifyScreenCenterIsTarget(new Vector3D(-1.0f, -4.0f, 2.5f), new Vector3D(2.9f, -1.0f, 3.5f));
		}

		private static void VerifyScreenCenterIsTarget(Vector3D position, Vector3D target)
		{
			var camera = CreateLookAtCamera(position, target);
			var floor = new Plane(Vector3D.UnitY, target);
			Ray ray = camera.ScreenPointToRay(Vector2D.Half);
			Assert.IsTrue(target.IsNearlyEqual(floor.Intersect(ray).Value));
		}

		[Test]
		public void ActivateShakeEffect()
		{
			var camera = CreateLookAtCamera(new Vector3D(0.0f, -5.0f, 5.0f), Vector3D.Zero);
			new Grid3D(new Size(10));
			camera.Start<CameraShakingUpdater>();
			new Command(() => { CameraShakingUpdater.isShaking = true; }).Add(new KeyTrigger(Key.Space));
		}

		//ncrunch: no coverage start
		public class CameraShakingUpdater : UpdateBehavior
		{
			public override void Update(IEnumerable<Entity> entities)
			{
				time += Time.Delta;
				if (time > 0.5f)
				{
					isShaking = false;
					time = 0.0f;
				}
				if (!isShaking)
					return;
				foreach (var entity in entities.OfType<Camera>())
					MoveCameraPosition(entity);
				goToLeft = !goToLeft;
			}

			private float time;
			public static bool isShaking;
			private bool goToLeft;

			private void MoveCameraPosition(Camera entity)
			{
				var delta = goToLeft ? -0.1f * Vector3D.UnitX : 0.1f * Vector3D.UnitX;
				entity.Position += delta;
				if (entity is LookAtCamera)
					((LookAtCamera)entity).Target += delta;
			}
		}
	}
}