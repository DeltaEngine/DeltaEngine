using System.Collections.Generic;
using System.Linq;
using DeltaEngine.Commands;
using DeltaEngine.Core;
using DeltaEngine.Datatypes;
using DeltaEngine.Extensions;
using DeltaEngine.Graphics;
using DeltaEngine.Input;
using DeltaEngine.Platforms;
using DeltaEngine.Rendering3D.Cameras;
using DeltaEngine.Rendering3D.Shapes;
using NUnit.Framework;

namespace DeltaEngine.Rendering3D.Tests.Cameras
{
	public class PathCameraTests : TestWithMocksOrVisually
	{
		[Test, CloseAfterFirstFrame]
		public void ThrowExceptionIfNoTrackPositionIsSpecified()
		{
			Assert.Throws<PathCamera.NoTrackSpecified>(() => CreatePathCamera(null));
			Assert.Throws<PathCamera.NoTrackSpecified>(() => CreatePathCamera(new Matrix[0]));
			Assert.Throws<PathCamera.NoTrackSpecified>(() => CreatePathCamera(new[] { Matrix.Identity }));
		}

		private PathCamera CreatePathCamera(Matrix[] cameraTrack)
		{
			var camera = new PathCamera(Resolve<Device>(), Resolve<Window>(), new CameraPath(cameraTrack));
			Assert.IsTrue(camera.IsPauseable);
			return camera;
		}

		[Test]
		public void CameraCanBeMovedOnLinearTrackWithScrollWheel()
		{
			new Grid3D(new Size(GridDimension));
			PathCamera camera = CreatePathCamera(GetLinearViewMatrixTrack(GridDimension));
			Command.Register(Command.Zoom, new MouseZoomTrigger());
			new Command(Command.Zoom, zoomAmount => camera.CurrentFrame += (int)(zoomAmount * 10));
		}

		private const int GridDimension = 100;

		private static Matrix[] GetLinearViewMatrixTrack(int length)
		{
			var track = new List<Matrix>();
			for (int i = 0; i < length; i++)
			{
				var position = new Vector3D(0, -(length / 2) + i, 5);
				var target = new Vector3D(0, -(length / 2) + i + 1, 5);
				track.Add(Matrix.CreateLookAt(position, target, Vector3D.UnitZ));
			}
			return track.ToArray();
		}

		[Test]
		public void AutomatedCameraMoveOnLinearTrackOnMiddleClick()
		{
			var linearTrack = GetLinearViewMatrixTrack(GridDimension);
			MoveCameraAutomatedOnTrack(linearTrack);
		}

		private void MoveCameraAutomatedOnTrack(Matrix[] track)
		{
			new Grid3D(new Size(GridDimension));
			PathCamera camera = CreatePathCamera(track);
			new Command(Command.MiddleClick, () => { camera.IsMoving = !camera.IsMoving; });
			new Command(Command.RightClick, () => camera.CurrentFrame = 0);
		}

		[Test]
		public void AutomatedCameraMoveOnCircularTrackOnMiddleClick()
		{
			var circlePoints = GetCirclePoints(10f, 5f);
			DrawPoints(circlePoints);
			var circularCameraTrack = VectorsToMatricesWithLookAtCenter(circlePoints);
			MoveCameraAutomatedOnTrack(circularCameraTrack.ToArray());
		}

		private static IEnumerable<Matrix> VectorsToMatricesWithLookAtCenter(
			IEnumerable<Vector3D> points)
		{
			return
				points.Select(point => Matrix.CreateLookAt(point, Vector3D.Zero, Vector3D.UnitZ)).ToList();
		}

		private static List<Vector3D> GetCirclePoints(float radius, float distanceToGround)
		{
			var vectorList = new List<Vector3D>();
			var pointsCount = GetPointsCount(radius);
			var theta = -360.0f / (pointsCount - 1);
			for (int i = 0; i < pointsCount; i++)
				vectorList.Add(CalculateCirclePoint(radius, distanceToGround, i, theta));
			return vectorList;
		}

		private static Vector3D CalculateCirclePoint(float radius, float distanceToGround,
			int pointCount, float theta)
		{
			float x = radius * MathExtensions.Cos(pointCount * theta);
			float y = radius * MathExtensions.Sin(pointCount * theta);
			return new Vector3D(x, y, distanceToGround);
		}

		private static int GetPointsCount(float radius)
		{
			const int PointsPerRadiusUnit = 96;
			return (int)(PointsPerRadiusUnit * radius);
		}

		private static void DrawPoints(IList<Vector3D> circlePoints)
		{
			for (int i = 0; i < circlePoints.Count; i++)
			{
				var start = circlePoints[i];
				Vector3D end = i + 1 < circlePoints.Count ? circlePoints[i + 1] : circlePoints[0];
				new Line3D(start, end, Color.Yellow);
			}
		}

		[Test, CloseAfterFirstFrame]
		public void CurrentFrameDoesNotChangeWhenNotMoving()
		{
			PathCamera camera = CreatePathCamera(GetLinearViewMatrixTrack(GridDimension));
			AdvanceTimeAndUpdateEntities();
			Assert.AreEqual(0, camera.CurrentFrame);
		}

		[Test, CloseAfterFirstFrame]
		public void CurrentFrameChangesWhenMoving()
		{
			PathCamera camera = CreatePathCamera(GetLinearViewMatrixTrack(GridDimension));
			camera.IsMoving = true;
			AdvanceTimeAndUpdateEntities();
			Assert.IsTrue(camera.CurrentFrame > 0);
		}

		[Test, CloseAfterFirstFrame]
		public void SetCurrentFrame()
		{
			PathCamera camera = CreatePathCamera(GetLinearViewMatrixTrack(GridDimension));
			camera.CurrentFrame = 1;
			Assert.AreEqual(1, camera.CurrentFrame);
		}

		[Test, CloseAfterFirstFrame]
		public void SetCurrentFramePastEnd()
		{
			PathCamera camera = CreatePathCamera(GetLinearViewMatrixTrack(GridDimension));
			camera.CurrentFrame = 1000;
			Assert.AreEqual(99, camera.CurrentFrame);
		}

		[Test, CloseAfterFirstFrame]
		public void GetCurrentViewMatrix()
		{
			var matrices = GetLinearViewMatrixTrack(GridDimension);
			PathCamera camera = CreatePathCamera(matrices);
			camera.CurrentFrame = 5;
			Assert.AreEqual(matrices[5], camera.GetCurrentViewMatrix());
		}

		[Test, CloseAfterFirstFrame]
		public void CreatePathCameraWithDefaultPath()
		{
			Assert.DoesNotThrow(() => { new PathCamera(Resolve<Device>(), Resolve<Window>()); });
		}
	}
}