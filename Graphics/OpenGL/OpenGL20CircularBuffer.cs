using System;
using System.Collections.Generic;
using DeltaEngine.Core;
using DeltaEngine.Graphics.Vertices;

namespace DeltaEngine.Graphics.OpenGL
{
	public class OpenGL20CircularBuffer : CircularBuffer
	{
		private OpenGLDevice glDevice;
		private int nativeVertexBuffer;
		private int nativeIndexBuffer;
		private readonly List<int> buffersToDisposeNextFrame = new List<int>();

		public OpenGL20CircularBuffer(Device device, ShaderWithFormat shader, BlendMode blendMode, VerticesMode drawMode = VerticesMode.Triangles)
			: base(device, shader, blendMode, drawMode) {}

		protected override void CreateNative()
		{
			glDevice = (OpenGLDevice)device;
			nativeVertexBuffer = glDevice.CreateVertexBuffer(maxNumberOfVertices * vertexSize, OpenGL20BufferMode.Stream);
			if (nativeVertexBuffer == OpenGLDevice.InvalidHandle)
				throw new UnableToCreateOpenGLGeometry();
			if (UsesIndexBuffer)
				nativeIndexBuffer = glDevice.CreateIndexBuffer(maxNumberOfIndices * indexSize, OpenGL20BufferMode.Stream);
		}

		protected override void DisposeNextFrame()
		{
			buffersToDisposeNextFrame.Add(nativeVertexBuffer);
			if (UsesIndexBuffer)
				buffersToDisposeNextFrame.Add(nativeIndexBuffer);
		}

		protected override void AddDataNative<VertexType>(Chunk textureChunk, VertexType[] vertexData, short[] indices, int numberOfVertices, int numberOfIndices)
		{
			glDevice.BindVertexBuffer(nativeVertexBuffer);
			glDevice.LoadVertexData(totalVertexOffsetInBytes, vertexData, vertexSize * numberOfVertices);
			if (!UsesIndexBuffer)
				return;
			if (indices == null)
				indices = ComputeIndices(textureChunk.NumberOfVertices, numberOfVertices);
			else if (totalIndicesCount > 0)
				indices = RemapIndices(indices, numberOfIndices);
			glDevice.BindIndexBuffer(nativeIndexBuffer);
			glDevice.LoadIndices(totalIndexOffsetInBytes, indices, indexSize * numberOfIndices);
		}

		public override void DisposeUnusedBuffersFromPreviousFrame()
		{
			if (buffersToDisposeNextFrame.Count <= 0)
				return;
			foreach (int buffer in buffersToDisposeNextFrame)
				glDevice.DeleteBuffer(buffer);
			buffersToDisposeNextFrame.Clear();
		}

		public override void DrawAllTextureChunks()
		{
			glDevice.BindVertexBuffer(nativeVertexBuffer);
			if (UsesIndexBuffer)
				glDevice.BindIndexBuffer(nativeIndexBuffer);
			base.DrawAllTextureChunks();
		}

		protected override void DrawChunk(Chunk chunk)
		{
			if (Is3D && blendMode == BlendMode.Additive)
				device.DisableDepthTest();
			if (UsesIndexBuffer)
			{
				if (chunk.Texture != null)
					shader.SetDiffuseTexture(chunk.Texture);
				glDevice.DrawTriangles(chunk.FirstIndexOffsetInBytes, chunk.NumberOfIndices);
			}
			else
				glDevice.DrawLines(chunk.FirstVertexOffsetInBytes / vertexSize, chunk.NumberOfVertices);
		}

		protected override void DisposeNative()
		{
			glDevice.DeleteBuffer(nativeVertexBuffer);
			if (UsesIndexBuffer)
				glDevice.DeleteBuffer(nativeIndexBuffer);
		}

		private class UnableToCreateOpenGLGeometry : Exception {}
	}
}