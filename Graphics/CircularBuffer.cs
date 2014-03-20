using System;
using System.Collections.Generic;
using DeltaEngine.Content;
using DeltaEngine.Core;
using DeltaEngine.Graphics.Vertices;

namespace DeltaEngine.Graphics
{
	/// <summary>
	/// Provides a way to render lots of small batches inside a much larger circular vertex buffer.
	/// Each buffer uses one shader with a specific vertex format plus a blend mode and draw mode.
	/// http://blogs.msdn.com/b/shawnhar/archive/2010/07/07/setdataoptions-nooverwrite-versus-discard.aspx
	/// </summary>
	public abstract class CircularBuffer : IDisposable
	{
		protected CircularBuffer(Device device, ShaderWithFormat shader, BlendMode blendMode,
			VerticesMode drawMode)
		{
			this.device = device;
			this.shader = shader;
			this.blendMode = blendMode;
			this.drawMode = drawMode;
			vertexSize = shader.Format.Stride;
			indexSize = sizeof(short);
			maxNumberOfVertices = DefaultMaxNumberOfVertices;
			maxNumberOfIndices = DefaultMaxNumberOfIndices;
			Initialize();
		}

		protected readonly Device device;
		public readonly ShaderWithFormat shader;
		public readonly BlendMode blendMode;
		protected readonly VerticesMode drawMode;
		protected readonly int vertexSize;
		protected readonly int indexSize;
		protected int maxNumberOfVertices;
		protected int maxNumberOfIndices;
		private const int DefaultMaxNumberOfVertices = 4 * 256;
		private const int DefaultMaxNumberOfIndices = 6 * 256;

		private void Initialize()
		{
			if (isCreated)
				DisposeNextFrame();
			isCreated = true;
			CreateNative();
		}

		protected bool isCreated;
		protected abstract void DisposeNextFrame();
		protected abstract void CreateNative();

		public void Dispose()
		{
			if (isCreated)
				DisposeNative();
			isCreated = false;
		}

		protected abstract void DisposeNative();

		protected bool UsesIndexBuffer
		{
			get { return drawMode != VerticesMode.Lines; }
		}

		protected bool UsesTexturing
		{
			get { return drawMode != VerticesMode.Lines && textureChunks[0].Texture != null; }
		}

		/// <summary>
		/// Adds a short list of vertices (a quad or precomputed vertices for Particles), indices can be
		/// null for lines or when indices should be computed automatically for quads (0,1,2,0,2,3).
		/// </summary>
		public void Add<VertexType>(Image texture, VertexType[] vertices, short[] indices = null,
			int numberOfVerticesUsed = 0, int numberOfIndicesUsed = 0) where VertexType : struct, Vertex
		{
			int numberOfVertices = numberOfVerticesUsed;
			if (numberOfVerticesUsed == 0)
				numberOfVertices = vertices.Length;
			int numberOfIndices = numberOfIndicesUsed;
			if (numberOfIndicesUsed == 0)
				numberOfIndices = indices != null ? indices.Length : 0;
			if (drawMode == VerticesMode.Triangles && numberOfIndices == 0)
				numberOfIndices = numberOfVertices == 3 ? 3 : numberOfVertices * 6 / 4;
			CheckTotalDataSize(numberOfVertices, numberOfIndices);
			var chunk = GetOrCreateLastTextureChunk(texture);
			if (chunk.NumberOfVertices == 0 && vertices.Length > 0 && shader.Format != vertices[0].Format)
				throw new ShaderVertexFormatDoesNotMatchVertex(shader.Format, default(VertexType).Format);
			AddDataNative(chunk, vertices, indices, numberOfVertices, numberOfIndices);
			UpdateOffsets(chunk, numberOfVertices, numberOfIndices);
		}

		private void CheckTotalDataSize(int newVerticesCount, int newIndicesCount)
		{
			int numberOfNeededVertices = totalVerticesCount + newVerticesCount;
			int numberOfNeededIndices = totalIndicesCount + newIndicesCount;
			if (numberOfNeededVertices <= maxNumberOfVertices &&
				numberOfNeededIndices <= maxNumberOfIndices)
				return;
			if (newVerticesCount > TotalMaximumVerticesLimit)
				throw new TooManyVerticesForCircularBuffer(newVerticesCount);
			if (textureChunks.Count > 0)
				DrawEverythingWhenBufferIsFull();
			numberOfNeededVertices = verticesAddedSinceBeginningOfFrame + newVerticesCount;
			BufferIsFullResetToBeginning();
			if ((numberOfNeededVertices > maxNumberOfVertices ||
				numberOfNeededIndices > maxNumberOfIndices) &&
				maxNumberOfVertices < TotalMaximumVerticesLimit)
				ResizeBuffers(numberOfNeededVertices, numberOfNeededIndices);
		}

		public const int TotalMaximumVerticesLimit = 65536;

		public class TooManyVerticesForCircularBuffer : Exception
		{
			public TooManyVerticesForCircularBuffer(int newVerticesCount)
				: base("Vertices " + newVerticesCount + ", Maximum: " + TotalMaximumVerticesLimit) {}
		}

		protected void DrawEverythingWhenBufferIsFull()
		{
			if (Is3D)
				device.Set3DMode();
			else
				device.Set2DMode();
			DrawAllTextureChunks();
		}

		/// <summary>
		/// Some frameworks will overwrite this to set the buffer to discard mode, which is otherwise
		/// in NoOverwrite mode for better GPU performance when adding data at the end of the buffer.
		/// </summary>
		protected virtual void BufferIsFullResetToBeginning()
		{
			totalVerticesCount = 0;
			totalIndicesCount = 0;
			totalVertexOffsetInBytes = 0;
			totalIndexOffsetInBytes = 0;
			verticesAddedSinceBeginningOfFrame = 0;
			textureChunks.Clear();
		}

		protected int totalVerticesCount;
		protected int totalIndicesCount;
		protected int totalVertexOffsetInBytes;
		protected int totalIndexOffsetInBytes;
		private readonly List<Chunk> textureChunks = new List<Chunk>();
		private int verticesAddedSinceBeginningOfFrame;

		/// <summary>
		/// Keeps the offsets for each texture chunk to be rendered in this circular buffer this frame.
		/// </summary>
		protected class Chunk
		{
			public Chunk(Image texture, int firstVertexOffsetInBytes, int firstIndexOffsetInBytes)
			{
				Texture = texture;
				FirstVertexOffsetInBytes = firstVertexOffsetInBytes;
				FirstIndexOffsetInBytes = firstIndexOffsetInBytes;
			}

			public readonly Image Texture;
			public readonly int FirstVertexOffsetInBytes;
			public int NumberOfVertices;
			public readonly int FirstIndexOffsetInBytes;
			public int NumberOfIndices;
		}
		
		private void ResizeBuffers(int newVerticesNeeded, int newIndicesNeeded)
		{
			do
				maxNumberOfVertices *= 2;
			while (maxNumberOfVertices < newVerticesNeeded);
			do
				maxNumberOfIndices *= 2;
			while (maxNumberOfIndices < newIndicesNeeded);
			if (maxNumberOfIndices > maxNumberOfVertices * 3)
				maxNumberOfIndices = maxNumberOfVertices * 3;
			Initialize();
		}

		/// <summary>
		/// We are only interested in the last chunk. Either it uses the same texture we want to use
		/// now or not, then a new chunk is needed at the end. All data comes in presorted by
		/// SpriteBatchRenderer, FontTextRenderer or ParticleRenderer.
		/// </summary>
		private Chunk GetOrCreateLastTextureChunk(Image texture)
		{
			if (textureChunks.Count != 0 && textureChunks[textureChunks.Count - 1].Texture == texture)
				return textureChunks[textureChunks.Count - 1];
			var newChunk = new Chunk(texture, totalVertexOffsetInBytes, totalIndexOffsetInBytes);
			textureChunks.Add(newChunk);
			return newChunk;
		}

		internal class ShaderVertexFormatDoesNotMatchVertex : Exception
		{
			public ShaderVertexFormatDoesNotMatchVertex(VertexFormat materialFormat,
				VertexFormat vertexFormat)
				: base("Shader Format=" + materialFormat + ", Vertex Format=" + vertexFormat) {}
		}

		protected abstract void AddDataNative<VertexType>(Chunk textureChunk, VertexType[] vertexData,
			short[] indices, int numberOfVertices, int numberOfIndices) where VertexType : struct, Vertex;

		private void UpdateOffsets(Chunk chunk, int newVerticesCount, int newIndicesCount)
		{
			chunk.NumberOfVertices += newVerticesCount;
			chunk.NumberOfIndices += newIndicesCount;
			totalVerticesCount += newVerticesCount;
			totalIndicesCount += newIndicesCount;
			totalVertexOffsetInBytes = totalVerticesCount * vertexSize;
			totalIndexOffsetInBytes = totalIndicesCount * indexSize;
			verticesAddedSinceBeginningOfFrame += newVerticesCount;
		}

		public int NumberOfActiveVertices
		{
			get { return verticesAddedSinceBeginningOfFrame; }
		}
		public bool Is3D
		{
			get { return shader.Format.Is3D; }
		}

		protected short[] ComputeIndices(int vertexOffset, int numberOfVertices)
		{
			if (numberOfVertices == 4)
			{
				cachedQuadIndices[0] = (short)vertexOffset;
				cachedQuadIndices[1] = (short)(vertexOffset + 1);
				cachedQuadIndices[2] = (short)(vertexOffset + 2);
				cachedQuadIndices[3] = (short)vertexOffset;
				cachedQuadIndices[4] = (short)(vertexOffset + 2);
				cachedQuadIndices[5] = (short)(vertexOffset + 3);
				return cachedQuadIndices;
			}
			if (numberOfVertices == 3)
				return new[] { (short)vertexOffset, (short)(vertexOffset + 1), (short)(vertexOffset + 2) };
			var newIndices = new short[(numberOfVertices / 4) * 6];
			for (int i = 0; i < numberOfVertices / 4; i++)
			{
				newIndices[i * 6 + 0] = (short)(vertexOffset + i * 4);
				newIndices[i * 6 + 0] = (short)(vertexOffset + i * 4 + 1);
				newIndices[i * 6 + 2] = (short)(vertexOffset + i * 4 + 2);
				newIndices[i * 6 + 3] = (short)(vertexOffset + i * 4);
				newIndices[i * 6 + 4] = (short)(vertexOffset + i * 4 + 2);
				newIndices[i * 6 + 5] = (short)(vertexOffset + i * 4 + 3);
			}
			return newIndices;
		}

		protected readonly short[] cachedQuadIndices = { 0, 1, 2, 0, 2, 3 };

		protected short[] RemapIndices(short[] indices, int numberOfIndices)
		{
			var newIndices = new short[numberOfIndices];
			for (int i = 0; i < numberOfIndices; i++)
				newIndices[i] = (short)(indices[i] + totalVerticesCount);
			return newIndices;
		}

		/// <summary>
		/// After all draw calls have been collected for this blend mode and shader they all drawn here.
		/// </summary>
		public virtual void DrawAllTextureChunks()
		{
			device.SetBlendMode(blendMode);
			BindShader();
			foreach (var chunk in textureChunks)
				DrawChunk(chunk);
			ClearChunksForNextFrame();
		}

		private void BindShader()
		{
			shader.Bind();
			shader.SetModelViewProjection(device.ModelViewProjectionMatrix);
		}

		protected abstract void DrawChunk(Chunk chunk);

		/// <summary>
		/// Initially the chunks where remembered between frames and compared, but this made things
		/// slower. All vertices will be added again next frame, which is fast, even for MBs of data.
		/// </summary>
		private void ClearChunksForNextFrame()
		{
			textureChunks.Clear();
			verticesAddedSinceBeginningOfFrame = 0;
		}

		public abstract void DisposeUnusedBuffersFromPreviousFrame();
	}
}