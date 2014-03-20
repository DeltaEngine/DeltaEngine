using System;
using System.Collections.Generic;
using DeltaEngine.Content;
using DeltaEngine.Core;
using DeltaEngine.Datatypes;
using DeltaEngine.Graphics.Vertices;

namespace DeltaEngine.Graphics
{
	/// <summary>
	/// Draws shapes, images and geometries. Framework independent, but needs a graphic device.
	/// </summary>
	public sealed class Drawing : IDisposable
	{
		public Drawing(Device device, Window window)
		{
			this.device = device;
			this.window = window;
		}

		private readonly Device device;
		private readonly Window window;

		public void Dispose()
		{
			foreach (var pair in buffersPerBlendMode)
				foreach (var buffer in pair.Value)
					buffer.Dispose();
			foreach (var buffer in lineBuffers)
				buffer.Dispose();
		}

		public void AddGeometry(Geometry geometry, Material material, Matrix transform)
		{
			foreach (var sorted in sortedShaderGeometries)
				if (sorted.shader == material.Shader)
				{
					sorted.AddGeometry(new RenderGeometryTask(geometry, material, transform));
					return;
				}
			sortedShaderGeometries.Add(
				new GeometryPerShader(new RenderGeometryTask(geometry, material, transform), device));
		}

		private class RenderGeometryTask
		{
			public RenderGeometryTask(Geometry geometry, Material material, Matrix transform)
			{
				Geometry = geometry;
				Material = material;
				Transform = transform;
			}

			public Geometry Geometry { get; private set; }
			public Material Material { get; private set; }
			public Matrix Transform { get; private set; }
		}

		private readonly List<GeometryPerShader> sortedShaderGeometries =
			new List<GeometryPerShader>();

		private class GeometryPerShader
		{
			public GeometryPerShader(RenderGeometryTask renderGeometry, Device device)
			{
				shader = renderGeometry.Material.Shader;
				this.device = device;
				AddGeometry(renderGeometry);
			}

			private readonly Device device;
			public readonly Shader shader;

			public void AddGeometry(RenderGeometryTask renderGeometry)
			{
				foreach (var sorted in sortedTextureGeometries)
					if (sorted.texture == renderGeometry.Material.DiffuseMap)
					{
						sorted.geometries.Add(renderGeometry);
						return;
					}
				sortedTextureGeometries.Add(new GeometryPerTexture(renderGeometry, device));
			}

			private readonly List<GeometryPerTexture> sortedTextureGeometries =
				new List<GeometryPerTexture>();

			private class GeometryPerTexture
			{
				public GeometryPerTexture(RenderGeometryTask renderGeometry, Device device)
				{
					this.device = device;
					texture = renderGeometry.Material.DiffuseMap;
					geometries.Add(renderGeometry);
				}

				private readonly Device device;
				public readonly Image texture;
				public readonly List<RenderGeometryTask> geometries = new List<RenderGeometryTask>();

				public void Draw()
				{
					//ncrunch: no coverage start
					foreach (var geometryTask in geometries)
					{
						var material = geometryTask.Material;
						if (material.LightMap != null)
							material.Shader.SetLightmapTexture(material.LightMap);
						material.Shader.SetModelViewProjection(geometryTask.Transform, device.CameraViewMatrix,
							device.CameraProjectionMatrix);
						if (geometryTask.Geometry.HasAnimationData)
							material.Shader.SetJointMatrices(geometryTask.Geometry.TransformsOfBones);
						if (material.Shader.Flags.HasFlag(ShaderFlags.Lit))
							material.Shader.SetSunLight(SunLight.Current);
						if (material.Shader.Flags.HasFlag(ShaderFlags.Fog))
							material.Shader.ApplyFogSettings(FogSettings.Current);
						geometryTask.Geometry.Draw();
					} //ncrunch: no coverage end
				}
			}

			public void Draw()
			{
				shader.Bind();
				foreach (var textureGeometries in sortedTextureGeometries)
				{
					if (textureGeometries.texture != null)
					{
						device.SetBlendMode(textureGeometries.texture.BlendMode);
						shader.SetDiffuseTexture(textureGeometries.texture);
					}
					textureGeometries.Draw();
				}
			}
		}

		/// <summary>
		/// Adds presorted material DrawableEntities calls. Rendering happens after all vertices have
		/// been added at the end of the frame in <see cref="DrawEverythingInCurrentLayer"/>.
		/// </summary>
		public void Add<T>(Material material, T[] vertices, short[] indices = null,
			int numberOfVerticesUsed = 0, int numberOfIndicesUsed = 0) where T : struct, Vertex
		{
			var mode = material.DiffuseMap == null ? BlendMode.Normal : material.DiffuseMap.BlendMode;
			Add(material, mode, vertices, indices, numberOfVerticesUsed, numberOfIndicesUsed);
		}

		public void Add<T>(Material material, BlendMode blendMode, T[] vertices,
			short[] indices = null, int numberOfVerticesUsed = 0, int numberOfIndicesUsed = 0)
			where T : struct, Vertex
		{
			GetDrawBuffer(material.Shader, blendMode).Add(material.DiffuseMap, vertices, indices,
				numberOfVerticesUsed, numberOfIndicesUsed);
		}

		private CircularBuffer GetDrawBuffer(Shader shader, BlendMode blendMode)
		{
			foreach (var pair in buffersPerBlendMode)
				if (pair.Key == blendMode)
				{
					foreach (var buffer in pair.Value)
						if (buffer.shader == shader)
							return buffer;
					var newBuffer = device.CreateCircularBuffer(shader as ShaderWithFormat, blendMode);
					pair.Value.Add(newBuffer);
					return newBuffer;
				}
			var initialBuffer = device.CreateCircularBuffer(shader as ShaderWithFormat, blendMode);
			buffersPerBlendMode.Add(blendMode, new List<CircularBuffer> { initialBuffer });
			return initialBuffer;
		}

		private readonly Dictionary<BlendMode, List<CircularBuffer>> buffersPerBlendMode =
			new Dictionary<BlendMode, List<CircularBuffer>>();

		public void FlushDrawBuffer(Material material, BlendMode blendMode)
		{
			var buffer = GetDrawBuffer(material.Shader, blendMode);
			buffer.DrawAllTextureChunks();
		}

		public void AddLines<T>(Material material, T[] vertices) where T : struct, Vertex
		{
			if (material.DiffuseMap != null)
				throw new LineMaterialShouldNotUseDiffuseMap(material);
			if (vertices.Length == 0)
				return;
			GetDrawBufferForLines(material.Shader).Add(null, vertices);
		}

		public class LineMaterialShouldNotUseDiffuseMap : Exception
		{
			public LineMaterialShouldNotUseDiffuseMap(Material material)
				: base(material.ToString()) {}
		}

		private CircularBuffer GetDrawBufferForLines(Shader shader)
		{
			foreach (var buffer in lineBuffers)
				if (buffer.shader == shader)
					return buffer;
			var newBuffer = device.CreateCircularBuffer(shader as ShaderWithFormat, BlendMode.Normal,
				VerticesMode.Lines);
			lineBuffers.Add(newBuffer);
			return newBuffer;
		}

		private readonly List<CircularBuffer> lineBuffers = new List<CircularBuffer>();

		internal void DrawEverythingInCurrentLayer()
		{
			if (Has3DData())
				Draw3DData();
			Draw2DData();
		}

		private bool Has3DData()
		{
			if (sortedShaderGeometries.Count > 0)
				return true;
			foreach (var lineBuffer in lineBuffers)
				if (lineBuffer.Is3D && lineBuffer.NumberOfActiveVertices > 0)
					return true;
			foreach (var pair in buffersPerBlendMode)
				foreach (var buffer in pair.Value)
					if (buffer.Is3D && buffer.NumberOfActiveVertices > 0)
						return true;
			return false;
		}

		private void Draw3DData()
		{
			device.Set3DMode();
			foreach (var lineBuffer in lineBuffers)
				if (lineBuffer.Is3D && lineBuffer.NumberOfActiveVertices > 0)
					DrawBufferAndIncreaseStatisticsNumbers(lineBuffer);
			foreach (var sortedGeometry in sortedShaderGeometries)
				sortedGeometry.Draw();
			sortedShaderGeometries.Clear();
			foreach (var pair in buffersPerBlendMode)
				foreach (var buffer in pair.Value)
					if (buffer.Is3D && buffer.NumberOfActiveVertices > 0)
						DrawBufferAndIncreaseStatisticsNumbers(buffer);
		}

		private void DrawBufferAndIncreaseStatisticsNumbers(CircularBuffer buffer)
		{
			NumberOfDynamicDrawCallsThisFrame++;
			NumberOfDynamicVerticesDrawnThisFrame += buffer.NumberOfActiveVertices;
			buffer.DisposeUnusedBuffersFromPreviousFrame();
			buffer.DrawAllTextureChunks();
		}

		private void Draw2DData()
		{
			device.Set2DMode();
			foreach (var buffer in lineBuffers)
				if (!buffer.Is3D && buffer.NumberOfActiveVertices > 0)
					DrawBufferAndIncreaseStatisticsNumbers(buffer);
			foreach (var pair in buffersPerBlendMode)
				foreach (var buffer in pair.Value)
					if (!buffer.Is3D && buffer.NumberOfActiveVertices > 0)
						DrawBufferAndIncreaseStatisticsNumbers(buffer);
		}

		public int NumberOfDynamicVerticesDrawnThisFrame { get; internal set; }
		public int NumberOfDynamicDrawCallsThisFrame { get; internal set; }

		public Size ViewportPixelSize
		{
			get { return window.ViewportPixelSize; }
		}
	}
}