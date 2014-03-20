using System;
using DeltaEngine.Content;
using DeltaEngine.Core;
using DeltaEngine.Graphics;

namespace DeltaEngine.Rendering2D
{
	public abstract class Batch
	{
		public Material Material { get; protected set; }
		public BlendMode BlendMode { get; protected set; }
		protected bool hasUV;
		protected bool hasColor;
		protected short[] indices;
		protected const int MinNumberOfQuads = 16;
		protected const int IndicesPerQuad = 6;
		protected const int VerticesPerQuad = 4;
		protected int indicesIndex;
		public int verticesIndex;

		public abstract void Draw(Drawing drawing);

		public void AddIndices()
		{
			indices[indicesIndex++] = (short)verticesIndex;
			indices[indicesIndex++] = (short)(verticesIndex + 1);
			indices[indicesIndex++] = (short)(verticesIndex + 2);
			indices[indicesIndex++] = (short)verticesIndex;
			indices[indicesIndex++] = (short)(verticesIndex + 2);
			indices[indicesIndex++] = (short)(verticesIndex + 3);
		}

		public void AddIndicesReversedWinding()
		{
			indices[indicesIndex++] = (short)verticesIndex;
			indices[indicesIndex++] = (short)(verticesIndex + 2);
			indices[indicesIndex++] = (short)(verticesIndex + 1);
			indices[indicesIndex++] = (short)verticesIndex;
			indices[indicesIndex++] = (short)(verticesIndex + 3);
			indices[indicesIndex++] = (short)(verticesIndex + 2);
		}

		public bool IsBufferFullAndResizeIfPossible(int numberOfQuadsToAdd = 1)
		{
			bool isBufferFull = IsBufferFull(indices.Length, numberOfQuadsToAdd);
			if (!isBufferFull || indices.Length >= MaxNumberOfIndices)
				return isBufferFull;
			GrowIndicesAndVerticesToSmallestPowerOfTwoThatFits(numberOfQuadsToAdd);
			return IsBufferFull(indices.Length, numberOfQuadsToAdd);
		}

		private const int MaxNumberOfIndices = short.MaxValue;

		private bool IsBufferFull(int length, int numberOfQuadsToAdd)
		{
			return length - indicesIndex < IndicesPerQuad * numberOfQuadsToAdd;
		}

		private void GrowIndicesAndVerticesToSmallestPowerOfTwoThatFits(int numberOfQuadsToAdd)
		{
			int newNumberOfIndices = indices.Length * 2;
			while (IsBufferFull(newNumberOfIndices, numberOfQuadsToAdd))
				newNumberOfIndices *= 2; //ncrunch: no coverage
			ResizeIndicesAndVertices(newNumberOfIndices);
		}

		private void ResizeIndicesAndVertices(int newNumberOfIndices)
		{
			if (newNumberOfIndices > MaxNumberOfIndices)
				newNumberOfIndices = MaxNumberOfIndices; //ncrunch: no coverage
			var newIndices = new short[newNumberOfIndices];
			Array.Copy(indices, newIndices, indices.Length);
			indices = newIndices;
			int newNumberOfVertices = newNumberOfIndices * VerticesPerQuad / IndicesPerQuad;
			if (!hasUV)
				ResizeColorVertices(newNumberOfVertices);
			else if (hasColor)
				ResizeColorUVVertices(newNumberOfVertices);
			else
				ResizeUVVertices(newNumberOfVertices);
		}

		protected abstract void ResizeColorVertices(int newNumberOfVertices);

		protected abstract void ResizeColorUVVertices(int newNumberOfVertices);

		protected abstract void ResizeUVVertices(int newNumberOfVertices);

		public void Reset()
		{
			verticesIndex = 0;
			indicesIndex = 0;
		}
	}
}