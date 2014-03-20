using System;
using System.Collections.Generic;
using System.Globalization;
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
	public class VisualFogTests : TestWithMocksOrVisually
	{
		[SetUp]
		public void InitializeCameraAndFogSettings()
		{
			var camera = Camera.Use<LookAtCamera>();
			camera.Position = Vector3D.One * 1.25f;
			new FogSettings(Color.Blue, 3, 12);
		}

		[TearDown]
		public void DisposeCamera()
		{
			Camera.Current.Dispose();
		}

		[Test, ApproveFirstFrameScreenshot]
		public void ShowFogOnColoredVertices()
		{
			CreateGroundWithColoredVertices(3, 3, Color.White);
		}

		private static void CreateGroundWithColoredVertices(float width, float depth, Color vertexColor)
		{
			var vertices = new Vertex[]
			{
				new VertexPosition3DColor(GetGroundVertexPosition(0, width, depth), vertexColor),
				new VertexPosition3DColor(GetGroundVertexPosition(1, width, depth), vertexColor),
				new VertexPosition3DColor(GetGroundVertexPosition(2, width, depth), vertexColor),
				new VertexPosition3DColor(GetGroundVertexPosition(3, width, depth), vertexColor),
			};
			new GroundPlane(vertices, new Material(ShaderFlags.ColoredFog, ""));
		}

		private static Vector3D GetGroundVertexPosition(int vertexIndex, float width, float depth,
			float height = 0.0f)
		{
			float halfWidth = width / 2.0f;
			float halfDepth = depth / 2.0f;
			switch (vertexIndex)
			{
			case 0:
				return new Vector3D(-halfWidth, halfDepth, height);
			case 1:
				return new Vector3D(-halfWidth, -halfDepth, height);
			case 2:
				return new Vector3D(halfWidth, -halfDepth, height);
			case 3:
				return new Vector3D(halfWidth, halfDepth, height);
			default:
				//ncrunch: no coverage start
				throw new WrongVertexIndex(vertexIndex);
			}
		}

		private class WrongVertexIndex : Exception
		{
			public WrongVertexIndex(int vertexIndex)
				: base(vertexIndex.ToString(CultureInfo.InvariantCulture)) {}
		} //ncrunch: no coverage end

		private class GroundPlane : DrawableEntity
		{
			public GroundPlane(Vertex[] vertices, Material material)
			{
				this.material = material;
				CreateGeometry(vertices);
				OnDraw<DrawGroundPlane>();
			}

			private readonly Material material;

			private void CreateGeometry(Vertex[] vertices)
			{
				short[] indices = { 0, 1, 3, 3, 1, 2 };
				var creationData = new GeometryCreationData(vertices[0].Format, vertices.Length,
					indices.Length);
				geometry = ContentLoader.Create<Geometry>(creationData);
				geometry.SetData(vertices, indices);
			}

			private Geometry geometry;

			private class DrawGroundPlane : DrawBehavior
			{
				public DrawGroundPlane(Drawing drawing)
				{
					this.drawing = drawing;
				}

				private readonly Drawing drawing;

				public void Draw(List<DrawableEntity> visibleEntities)
				{
					foreach (var triangle in visibleEntities.OfType<GroundPlane>())
						drawing.AddGeometry(triangle.geometry, triangle.material, Matrix.Identity);
				}
			}
		}

		[Test, ApproveFirstFrameScreenshot]
		public void ShowFogOnTexturedVertices()
		{
			CreateGroundWithTexturedVertices(10, 10, "DeltaEngineLogo");
		}

		private static void CreateGroundWithTexturedVertices(float width, float depth,
			string diffuseTextureName)
		{
			var vertices = new Vertex[]
			{
				new VertexPosition3DUV(GetGroundVertexPosition(0, width, depth), new Vector2D(0, 0)), 
				new VertexPosition3DUV(GetGroundVertexPosition(1, width, depth), new Vector2D(0, 1)),
				new VertexPosition3DUV(GetGroundVertexPosition(2, width, depth), new Vector2D(1, 1)),
				new VertexPosition3DUV(GetGroundVertexPosition(3, width, depth), new Vector2D(1, 0)),
			};
			new GroundPlane(vertices, new Material(ShaderFlags.TexturedFog, diffuseTextureName));
		}

		[Test, ApproveFirstFrameScreenshot]
		public void ShowFogOnColoredTexturedVertices()
		{
			CreateGroundWithColoredTexturedVertices(10, 10, Color.Green, "DeltaEngineLogo");
		}

		private static void CreateGroundWithColoredTexturedVertices(float width, float depth,
			Color vertexColor, string diffuseTextureName)
		{
			var vertices = new Vertex[]
			{
				new VertexPosition3DColorUV(GetGroundVertexPosition(0, width, depth), vertexColor,
					new Vector2D(0, 0)), 
				new VertexPosition3DColorUV(GetGroundVertexPosition(1, width, depth), vertexColor,
					new Vector2D(0, 1)),
				new VertexPosition3DColorUV(GetGroundVertexPosition(2, width, depth), vertexColor,
					new Vector2D(1, 1)),
				new VertexPosition3DColorUV(GetGroundVertexPosition(3, width, depth), vertexColor,
					new Vector2D(1, 0)),
			};
			new GroundPlane(vertices, new Material(ShaderFlags.ColoredTexturedFog, diffuseTextureName));
		}

		[Test, ApproveFirstFrameScreenshot]
		public void ShowFogOnTexturedLightmapVertices()
		{
			ShowExistingModelWithFog("LightmapBoxMaya");
		}

		private static void ShowExistingModelWithFog(string modelName)
		{
			var modelData = ContentLoader.Load<ModelData>(modelName);
			var meshesWithFog = new List<Mesh>();
			foreach (Mesh mesh in modelData.Meshes)
				meshesWithFog.Add(GetMeshWithFogShaderMaterial(mesh));
			new Model(new ModelData(meshesWithFog.ToArray()), Vector3D.Zero);
		}

		private static Mesh GetMeshWithFogShaderMaterial(Mesh mesh)
		{
			var materialWithFog = new Material(mesh.Material.Shader.Flags | ShaderFlags.Fog, "")
			{
				DiffuseMap = mesh.Material.DiffuseMap,
				LightMap = mesh.Material.LightMap
			};
			return new Mesh(mesh.Geometry, materialWithFog)
			{
				LocalTransform = mesh.LocalTransform,
				Animation = mesh.Animation
			};
		}

		[Test, ApproveFirstFrameScreenshot]
		public void ShowFogOnSkinnedTexturedVertices()
		{
			ShowExistingModelWithFog("AnimatedMeshMax");
			CreateGroundWithColoredVertices(3, 3, Color.Green);
		}
	}
}