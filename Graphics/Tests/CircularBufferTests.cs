using DeltaEngine.Content;
using DeltaEngine.Core;
using DeltaEngine.Graphics.Mocks;
using DeltaEngine.Graphics.Vertices;
using DeltaEngine.Platforms;
using NUnit.Framework;

namespace DeltaEngine.Graphics.Tests
{
	public class CircularBufferTests : TestWithMocksOrVisually
	{
		[SetUp]
		public void CreateBuffer()
		{
			buffer2D = new MockCircularBuffer(Resolve<Device>(),
				ContentLoader.Create<ShaderWithFormat>(
				new ShaderCreationData(ShaderFlags.Position2DTextured)), BlendMode.Normal,
				VerticesMode.Triangles);
			buffer3D = new MockCircularBuffer(Resolve<Device>(),
				ContentLoader.Create<ShaderWithFormat>(
				new ShaderCreationData(ShaderFlags.Textured)), BlendMode.Normal, VerticesMode.Triangles);
			image = ContentLoader.Load<Image>("DeltaEngineLogo");
			Assert.IsTrue(buffer2D.IsCreated);
		}

		private MockCircularBuffer buffer2D;
		private MockCircularBuffer buffer3D;
		private Image image;

		private readonly int vertexSize = VertexPosition3DColor.SizeInBytes;

		[Test, CloseAfterFirstFrame, Timeout(5000)]
		public void CreateAndDisposeBuffer()
		{
			buffer2D.Dispose();
			Assert.IsFalse(buffer2D.IsCreated);
		}

		[Test, CloseAfterFirstFrame, Timeout(5000)]
		public void BufferCanBe2DOr3D()
		{
			Assert.IsFalse(buffer2D.Is3D);
			Assert.IsTrue(buffer3D.Is3D);
		}

		[Test, CloseAfterFirstFrame]
		public void OffsetInitialization()
		{
			Assert.AreEqual(0, buffer2D.VertexOffset);
			Assert.AreEqual(0, buffer2D.IndexOffset);
		}

		[Test, CloseAfterFirstFrame]
		public void OffsetIncrement()
		{
			const int VerticesCount = 32;
			const int IndicesCount = 48;
			var vertices = new VertexPosition2DUV[VerticesCount];
			var indices = new short[IndicesCount];
			buffer2D.Add(null, vertices, indices);
			Assert.AreEqual(VerticesCount * vertexSize, buffer2D.VertexOffset);
			Assert.AreEqual(IndicesCount * sizeof(short), buffer2D.IndexOffset);
		}

		[Test, CloseAfterFirstFrame]
		public void OffsetSeveralIncrements()
		{
			const int VerticesCount = 32;
			const int IndicesCount = 48;
			var vertices = new VertexPosition2DUV[VerticesCount];
			var indices = new short[IndicesCount];
			for (int i = 1; i <= IncrementCount; i++)
			{
				buffer2D.Add(null, vertices, indices);
				Assert.AreEqual(i * VerticesCount * vertexSize, buffer2D.VertexOffset);
				Assert.AreEqual(i * IndicesCount * sizeof(short), buffer2D.IndexOffset);
			}
		}

		private const int IncrementCount = 4;

		[Test, CloseAfterFirstFrame]
		public void ExpectExceptionIfTryingToAddTooManyVertices()
		{
			const int NumberOfTooMuchVertices = CircularBuffer.TotalMaximumVerticesLimit + 1;
			var dummyVertices = new VertexPosition2DUV[1];
			Assert.Throws<CircularBuffer.TooManyVerticesForCircularBuffer>(
				() => buffer2D.Add(null, dummyVertices, new short[1], NumberOfTooMuchVertices, 1));
		}

		[Test]
		public void ForceLimitMaxNumberOfIndicesIfUsingTooMany()
		{
			buffer2D.Add(null, new VertexPosition2DUV[1], new short[1], 1,
				CircularBuffer.TotalMaximumVerticesLimit);
		}

		[Test, CloseAfterFirstFrame]
		public void DrawAndReset()
		{
			const int VerticesCount = 32;
			const int IndicesCount = 48;
			var vertices = new VertexPosition2DUV[VerticesCount];
			var indices = new short[IndicesCount];
			buffer2D.Add(null, vertices, indices);
			Assert.IsFalse(buffer2D.HasDrawn);
			buffer2D.DrawAllTextureChunks();
			Assert.IsTrue(buffer2D.HasDrawn);
			Assert.AreEqual(512, buffer2D.VertexOffset);
			Assert.AreEqual(96, buffer2D.IndexOffset);
		}

		[Test, CloseAfterFirstFrame]
		public void DataBiggerThanHalfOfTheBufferSize()
		{
			const int VerticesCount = 12288;
			const int IndicesCount = 16384;
			var vertices = new VertexPosition2DUV[VerticesCount];
			var indices = new short[IndicesCount];
			buffer2D.Add(null, vertices, indices);
			Assert.AreEqual(VerticesCount * vertexSize, buffer2D.VertexOffset);
			Assert.AreEqual(IndicesCount * sizeof(short), buffer2D.IndexOffset);
			Assert.IsFalse(buffer2D.HasDrawn);
		}

		[Test, CloseAfterFirstFrame]
		public void MakeBufferResize()
		{
			const int VerticesCount = 12288;
			const int IndicesCount = 16384;
			var vertices = new VertexPosition2DUV[VerticesCount];
			var indices = new short[IndicesCount];
			buffer2D.Add(null, vertices, indices);
			Assert.IsFalse(buffer2D.HasDrawn);
			buffer2D.Add(null, vertices, indices);
			Assert.IsTrue(buffer2D.HasDrawn);
		}

		[Test, CloseAfterFirstFrame]
		public void Make3DBufferResize()
		{
			const int VerticesCount = 12288;
			const int IndicesCount = 16384;
			var vertices = new VertexPosition3DUV[VerticesCount];
			var indices = new short[IndicesCount];
			buffer3D.Add(null, vertices, indices);
			Assert.IsFalse(buffer3D.HasDrawn);
			buffer3D.Add(null, vertices, indices);
			Assert.IsTrue(buffer3D.HasDrawn);
		}

		[Test, CloseAfterFirstFrame]
		public void LoadDataWithDifferentSize()
		{
			const int Data1VerticesCount = 100;
			const int Data1IndicesCount = 150;
			const int Data2VerticesCount = 400;
			const int Data2IndicesCount = 500;
			var vertices1 = new VertexPosition2DUV[Data1VerticesCount];
			var indices1 = new short[Data1IndicesCount];
			var vertices2 = new VertexPosition2DUV[Data2VerticesCount];
			var indices2 = new short[Data2IndicesCount];
			buffer2D.Add(null, vertices1, indices1);
			Assert.AreEqual(Data1VerticesCount * vertexSize, buffer2D.VertexOffset);
			Assert.AreEqual(Data1IndicesCount * sizeof(short), buffer2D.IndexOffset);
			buffer2D.Add(null, vertices2, indices2);
			Assert.AreEqual((Data1VerticesCount + Data2VerticesCount) * vertexSize, buffer2D.VertexOffset);
			Assert.AreEqual((Data1IndicesCount + Data2IndicesCount) * sizeof(short), buffer2D.IndexOffset);
		}

		[Test, CloseAfterFirstFrame]
		public void DataBiggerThanBufferSize()
		{
			var verticesCount = buffer2D.MaxNumberOfVertices * 3;
			var indicesCount = buffer2D.MaxNumberOfVertices * 4;
			var vertices = new VertexPosition2DUV[verticesCount];
			var indices = new short[indicesCount];
			buffer2D.Add(null, vertices, indices);
			Assert.AreEqual(verticesCount * vertexSize, buffer2D.VertexOffset);
			Assert.AreEqual(indicesCount * sizeof(short), buffer2D.IndexOffset);
		}

		[Test, CloseAfterFirstFrame]
		public void TrianglesBufferShouldUseIndexBuffer()
		{
			Assert.IsTrue(buffer2D.UsesIndexBuffer);
		}

		[Test, CloseAfterFirstFrame]
		public void LinesBufferShouldNotUseIndexBuffer()
		{
			var linesBuffer = new MockCircularBuffer(Resolve<Device>(),
				ContentLoader.Create<ShaderWithFormat>(
				new ShaderCreationData(ShaderFlags.Position2DTextured)), BlendMode.Normal,
				VerticesMode.Lines);
			Assert.IsFalse(linesBuffer.UsesIndexBuffer);
		}

		[Test, CloseAfterFirstFrame]
		public void UsingTexturing()
		{
			var vertices = new VertexPosition2DUV[4];
			buffer2D.Add(image, vertices);
			Assert.IsTrue(buffer2D.UsesTexturing);
		}

		[Test, CloseAfterFirstFrame]
		public void VertexFormatShouldMatchCircularBufferShaderVertexFormat()
		{
			var vertices = new VertexPosition2DColorUV[4];
			Assert.Throws<CircularBuffer.ShaderVertexFormatDoesNotMatchVertex>(
				() => buffer2D.Add(image, vertices));
		}

		[Test, CloseAfterFirstFrame]
		public void IndicesAreNotChangedWhenPassedAsArgument()
		{
			var vertices = new VertexPosition2DUV[4];
			buffer2D.Add(image, vertices, quadIndices);
			Assert.AreEqual(quadIndices, buffer2D.CachedIndices);
		}

		private readonly short[] quadIndices = { 0, 1, 2, 0, 2, 3 };

		[Test, CloseAfterFirstFrame]
		public void IndicesAreComputedWhenNotPassedAsArgument()
		{
			var vertices = new VertexPosition2DUV[4];
			buffer2D.Add(image, vertices);
			Assert.AreEqual(quadIndices, buffer2D.CachedIndices);
		}

		[Test, CloseAfterFirstFrame]
		public void IndicesAreRemappedWhenAddedVerticesAreNotAtTheBeginningOfTheBuffer()
		{
			var remappedIndices = new short[] { 4, 5, 6, 4, 6, 7 };
			var vertices = new VertexPosition2DUV[4];
			buffer2D.Add(image, vertices, quadIndices);
			Assert.AreEqual(quadIndices, buffer2D.CachedIndices);
			buffer2D.Add(image, vertices, quadIndices);
			Assert.AreEqual(remappedIndices, buffer2D.CachedIndices);
		}

		// ncrunch: no coverage start
		[Test, Category("Slow")]
		public void CircularBufferCannotHandleMoreThan65536Vertices()
		{
			Assert.AreEqual(1024, buffer3D.MaxNumberOfVertices);
			var vertices = new VertexPosition3DUV[65536];
			buffer3D.Add(image, vertices);
			buffer3D.Add(image, vertices);
			Assert.AreEqual(65536, buffer3D.MaxNumberOfVertices);
		}
	}
}