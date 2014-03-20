using System.Collections.Generic;
using System.Linq;
using DeltaEngine.Content;
using DeltaEngine.Datatypes;
using DeltaEngine.Entities;
using DeltaEngine.Graphics;
using DeltaEngine.Graphics.Vertices;
using DeltaEngine.Platforms;
using DeltaEngine.Rendering3D.Cameras;
using NUnit.Framework;

namespace DeltaEngine.Rendering3D.Tests
{
	public class VisualCullingTests : TestWithMocksOrVisually
	{
		[SetUp]
		public void InitializeCameraAndGraphicsDevice()
		{
			var camera = Camera.Use<LookAtCamera>();
			camera.Position = new Vector3D(0, -2, 2);
			graphicsDevice = Resolve<Device>();
		}

		private Device graphicsDevice;

		[Test, ApproveFirstFrameScreenshot]
		public void FlippedPlaneIsNotVisibleWhenCullingIsEnabled()
		{
			CreateNormalGroundPlane(-1, 0);
			CreateFlippedGroundPlane(1, 0);
			graphicsDevice.CullingMode = Culling.Enabled;
		}

		[Test, ApproveFirstFrameScreenshot]
		public void BothPlanesAreVisibleWhenCullingIsDisabled()
		{
			CreateNormalGroundPlane(-1, 0);
			CreateFlippedGroundPlane(1, 0);
			graphicsDevice.CullingMode = Culling.Disabled;
		}

		private static void CreateNormalGroundPlane(float x, float y)
		{
			new GroundPlane1X1(x, y, new Material(ShaderFlags.Colored, ""));
		}

		private static void CreateFlippedGroundPlane(float x, float y)
		{
			new GroundPlane1X1(x, y, new Material(ShaderFlags.Colored, ""))
			{
				Transform = Matrix.CreateRotationX(180)
			};
		}

		private class GroundPlane1X1 : DrawableEntity
		{
			public GroundPlane1X1(float x, float y, Material material)
			{
				this.material = material;
				CreatePlaneGeometry(x, y);
				OnDraw<DrawGroundPlane1X1>();
				Transform = Matrix.Identity;
			}
			
			private readonly Material material;

			private void CreatePlaneGeometry(float x, float y)
			{
				Vertex[] vertices = GetPlaneVertices(x, y);
				short[] indices = GetPlaneIndices();
				var creationData = new GeometryCreationData(VertexFormat.Position3DColor, vertices.Length,
					indices.Length);
				geometry = ContentLoader.Create<Geometry>(creationData);
				geometry.SetData(vertices, indices);
			}
			
			private Geometry geometry;

			private static Vertex[] GetPlaneVertices(float x, float y)
			{
				const float HalfWidth = 0.5f;
				const float HalfDepth = 0.5f;
				const float Height = 0.0f;
				Color color = Color.White;
				return new Vertex[]
				{
					new VertexPosition3DColor(new Vector3D(x - HalfWidth, y + HalfDepth, Height), color),
					new VertexPosition3DColor(new Vector3D(x + HalfWidth, y + HalfDepth, Height), color),
					new VertexPosition3DColor(new Vector3D(x + HalfWidth, y - HalfDepth, Height), color),
					new VertexPosition3DColor(new Vector3D(x - HalfWidth, y - HalfDepth, Height), color),
				};
			}

			private static short[] GetPlaneIndices()
			{
				return new short[] { 0, 1, 3, 3, 1, 2 };
			}

			public Matrix Transform { get; set; }

			private class DrawGroundPlane1X1 : DrawBehavior
			{
				public DrawGroundPlane1X1(Drawing drawing)
				{
					this.drawing = drawing;
				}

				private readonly Drawing drawing;

				public void Draw(List<DrawableEntity> visibleEntities)
				{
					foreach (var plane in visibleEntities.OfType<GroundPlane1X1>())
						drawing.AddGeometry(plane.geometry, plane.material, plane.Transform);
				}
			}
		}
	}
}
