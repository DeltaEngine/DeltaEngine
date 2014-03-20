using DeltaEngine.Content;
using DeltaEngine.Datatypes;
using DeltaEngine.Extensions;
using DeltaEngine.Graphics;
using DeltaEngine.Graphics.Mocks;
using DeltaEngine.Graphics.Vertices;
using DeltaEngine.Platforms;
using DeltaEngine.Rendering3D.Cameras;
using NUnit.Framework;

namespace DeltaEngine.Rendering3D.Tests
{
	public class PlaneQuadTests : TestWithMocksOrVisually
	{
		[Test, ApproveFirstFrameScreenshot]
		public void DrawRedPlane()
		{
			SetUpCamera();
			new Model(new ModelData(CreatePlaneQuad()), Vector3D.Zero);
		}

		private static void SetUpCamera(float cameraPosition = 3.0f)
		{
			var camera = Camera.Use<LookAtCamera>();
			camera.Position = new Vector3D(-cameraPosition, -cameraPosition, cameraPosition);
		}

		private static PlaneQuad CreatePlaneQuad(Color? color = null)
		{
			var material = new Material(ShaderFlags.ColoredTextured, "DeltaEngineLogo");
			material.DefaultColor = color ?? Color.Red;
			return new PlaneQuad(Size, material);
		}

		private static readonly Size Size = new Size(2);

		[Test, ApproveFirstFrameScreenshot]
		public void DrawUncoloredPlane()
		{
			SetUpCamera();
			new Model(new ModelData(CreateUncoloredPlaneQuad()), Vector3D.Zero);
		}

		private static PlaneQuad CreateUncoloredPlaneQuad()
		{
			var material = new Material(ShaderFlags.Textured, "DeltaEngineLogo");
			return new PlaneQuad(Size, material);
		}

		[Test, ApproveFirstFrameScreenshot]
		public void DrawFourPlanesWithoutCulling()
		{
			Resolve<Device>().CullingMode = Culling.Disabled;
			SetUpCamera(-3);
			new Model(new ModelData(CreateUncoloredPlaneQuad()), Vector3D.Zero);
			new Model(new ModelData(CreatePlaneQuad(Color.Red)), Vector3D.UnitX * Size.Width);
			new Model(new ModelData(CreatePlaneQuad(Color.Green)), Vector3D.UnitY);
			new Model(new ModelData(CreatePlaneQuad(Color.Blue)), Vector3D.UnitZ * Size.Height);
		}

		[Test, CloseAfterFirstFrame]
		public void ChangeSize()
		{
			PlaneQuad quad = CreatePlaneQuad();
			Assert.AreEqual(Size, quad.Size);
			quad.Size = 2 * Size;
			Assert.AreEqual(2 * Size, quad.Size);
		}

		[Test, CloseAfterFirstFrame]
		public void WindingOfVerticesShouldBeCounterClockWise()
		{
			if (!StackTraceExtensions.IsStartedFromNCrunch())
				return; //ncrunch: no coverage
			MockGeometry geometry = CreateMockGeometry();
			var indices = new short[] { 0, 1, 2 };
			geometry.SetData(CreateVertexData(), indices);
			AssertVertices(geometry, indices);
			AssertGeometryIsCounterClockWise(geometry);
		}

		private static MockGeometry CreateMockGeometry()
		{
			var creationData = new GeometryCreationData(VertexFormat.Position3DColor, 3, 3);
			return ContentLoader.Create<MockGeometry>(creationData);
		}

		private Vertex[] CreateVertexData()
		{
			var color = Color.Black;
			var ccwVertices = new Vertex[]
			{
				new VertexPosition3DColor(vertex0, color),
				new VertexPosition3DColor(vertex1, color),
				new VertexPosition3DColor(vertex2, color)
			};
			return ccwVertices;
		}

		private readonly Vector3D vertex0 = new Vector3D(0, 1, 0);
		private readonly Vector3D vertex1 = new Vector3D(-1, 0, 0);
		private readonly Vector3D vertex2 = new Vector3D(1, 0, 0);

		private void AssertVertices(MockGeometry geometry, short[] indices)
		{
			Assert.AreEqual(vertex0, geometry.GetVertexPosition(indices[0]));
			Assert.AreEqual(vertex1, geometry.GetVertexPosition(indices[1]));
			Assert.AreEqual(vertex2, geometry.GetVertexPosition(indices[2]));
		}

		private static void AssertGeometryIsCounterClockWise(MockGeometry geometry)
		{
			Vector3D vertex0 = geometry.GetVertexPosition(0);
			Vector3D vertex1 = geometry.GetVertexPosition(1);
			Vector3D vertex2 = geometry.GetVertexPosition(2);
			Assert.IsTrue(vertex0.X >= vertex1.X);
			Assert.IsTrue(vertex1.X <= vertex2.X);
			Assert.IsTrue(vertex2.X >= vertex0.X);
			Assert.IsTrue(vertex0.Y >= vertex1.Y);
			Assert.IsTrue(vertex1.Y <= vertex2.Y);
			Assert.IsTrue(vertex2.Y <= vertex0.Y);
		}

		[Test, CloseAfterFirstFrame]
		public void VerticesShouldNotBeWindedClockWise()
		{
			if (!StackTraceExtensions.IsStartedFromNCrunch())
				return; //ncrunch: no coverage
			MockGeometry geometry = CreateMockGeometry();
			var indices = new short[] { 2, 1, 0 };
			geometry.SetData(CreateVertexData(), indices);
			AssertVertices(geometry, indices);
			AssertGeometryIsClockWise(geometry);
		}

		private static void AssertGeometryIsClockWise(MockGeometry geometry)
		{
			Vector3D vertex0 = geometry.GetVertexPosition(0);
			Vector3D vertex1 = geometry.GetVertexPosition(1);
			Vector3D vertex2 = geometry.GetVertexPosition(2);
			Assert.IsTrue(vertex0.X >= vertex1.X);
			Assert.IsTrue(vertex1.X <= vertex2.X);
			Assert.IsTrue(vertex2.X <= vertex0.X);
			Assert.IsTrue(vertex0.Y >= vertex1.Y);
			Assert.IsTrue(vertex1.Y <= vertex2.Y);
			Assert.IsTrue(vertex2.Y >= vertex0.Y);
		}
	}
}