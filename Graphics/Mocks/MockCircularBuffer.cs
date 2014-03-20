using DeltaEngine.Core;
using DeltaEngine.Graphics.Vertices;

namespace DeltaEngine.Graphics.Mocks
{
	public class MockCircularBuffer : CircularBuffer
	{
		public MockCircularBuffer(Device device, ShaderWithFormat shader, BlendMode blendMode,
			VerticesMode drawMode)
			: base(device, shader, blendMode, drawMode) {}

		protected override void DisposeNextFrame() {}
		public override void DisposeUnusedBuffersFromPreviousFrame() { }

		protected override void CreateNative()
		{
			IsCreated = true;
		}

		public bool IsCreated { get; private set; }

		protected override void DisposeNative()
		{
			IsCreated = false;
		}

		protected override void AddDataNative<VertexType>(Chunk textureChunk, VertexType[] vertexData,
			short[] indices, int numberOfVertices, int numberOfIndices)
		{
			if (indices == null)
				indices = ComputeIndices(textureChunk.NumberOfVertices, numberOfVertices);
			else if (totalIndicesCount > 0)
				indices = RemapIndices(indices, numberOfIndices);
			CachedIndices = indices;
		}

		public short[] CachedIndices { get; private set; }

		protected override void DrawChunk(Chunk chunk)
		{
			HasDrawn = true;
		}

		public int VertexOffset
		{
			get { return totalVertexOffsetInBytes; }
		}

		public int IndexOffset
		{
			get { return totalIndexOffsetInBytes; }
		}

		public bool HasDrawn { get; private set; }

		public int MaxNumberOfVertices
		{
			get { return maxNumberOfVertices; }
		}

		public new bool UsesIndexBuffer { get { return base.UsesIndexBuffer; } }

		public new bool UsesTexturing { get { return base.UsesTexturing; } }
	}
}