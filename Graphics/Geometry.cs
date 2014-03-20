using System;
using System.IO;
using DeltaEngine.Content;
using DeltaEngine.Core;
using DeltaEngine.Datatypes;
using DeltaEngine.Graphics.Vertices;

namespace DeltaEngine.Graphics
{
	/// <summary>
	/// Base class for GPU geometry data.
	/// </summary>
	public abstract class Geometry : ContentData
	{
		protected Geometry(string contentName)
			: base(contentName) {}

		protected Geometry(GeometryCreationData creationData)
			: base("<GenerateGeometry>")
		{
			Format = creationData.Format;
			vertices = new byte[creationData.NumberOfVertices * Format.Stride];
			indices = new short[creationData.NumberOfIndices];
		}

		public VertexFormat Format { get; private set; }
		private byte[] vertices;
		private short[] indices;
		public int NumberOfVertices
		{
			get { return vertices.Length / Format.Stride; }
		}
		public int NumberOfIndices
		{
			get { return indices.Length; }
		}

		protected override void LoadData(Stream fileData)
		{
			if (fileData.Length == 0)
				throw new EmptyGeometryFileGiven();
			var loadedGeometry = new BinaryReader(fileData).Create() as GeometryData;
			Format = loadedGeometry.Format;
			vertices = loadedGeometry.VerticesData;
			indices = loadedGeometry.Indices;
			SetNativeData(vertices, indices);
		}

		public class EmptyGeometryFileGiven : Exception {}

		public class GeometryData
		{
			public String Name;
			public VertexFormat Format;
			public int NumberOfVertices;
			public byte[] VerticesData;
			public short[] Indices;
		}

		public abstract void Draw();

		public void SetData(Vertex[] setVertices, short[] setIndices)
		{
			SetData(BinaryDataExtensions.ToByteArray(setVertices), setIndices);
		}

		public void SetData(byte[] verticesData, short[] setIndices)
		{
			if (verticesData.Length != (NumberOfVertices * Format.Stride))
				throw new InvalidNumberOfVertices(verticesData.Length / Format.Stride, NumberOfVertices);
			if (setIndices.Length != NumberOfIndices)
				throw new InvalidNumberOfIndices(setIndices.Length, NumberOfIndices);
			vertices = verticesData;
			indices = setIndices;
			SetNativeData(vertices, indices);
		}

		public class InvalidNumberOfVertices : Exception
		{
			public InvalidNumberOfVertices(int actualVertices, int expectedVertices)
				: base("actualVertices=" + actualVertices + ", expectedVertices=" + expectedVertices) { }
		}

		public class InvalidNumberOfIndices : Exception
		{
			public InvalidNumberOfIndices(int actualIndices, int expectedIndices)
				: base("actualIndices=" + actualIndices + ", expectedIndices=" + expectedIndices) { }
		}

		public Matrix[] TransformsOfBones { get; set; }
		public bool HasAnimationData { get { return TransformsOfBones != null; } }

		protected abstract void SetNativeData(byte[] vertexData, short[] indices);

		//ncrunch: no coverage start
		public void LoadFromFile(Stream fileData)
		{
			var reader = new BinaryReader(fileData);
			string shortName = reader.ReadString();
			var dataVersion = reader.ReadBytes(4);
			var boolean = reader.ReadBoolean();
			if (boolean)
				reader.ReadString();
			boolean = reader.ReadBoolean();
			var type = reader.ReadString();
			if (type == null)
				throw new NullReferenceException();

			boolean = reader.ReadBoolean();
			var count = reader.ReadByte();
			var typeOfByte = reader.ReadByte();
			if (typeOfByte == 255)
				throw new NullReferenceException();
			type = reader.ReadString();
			var list = new VertexElement[count];
			for (int i = 0; i < count; i++)
			{
				var vertexElementType = reader.ReadInt32();
				var size = reader.ReadInt32();
				var vertexCount = reader.ReadInt32();
				var vertexElementOffset = reader.ReadInt32();
				list[i] = new VertexElement((VertexElementType)vertexElementType, size, vertexCount);
			}
			Format = new VertexFormat(list);
			var formatStride = reader.ReadInt32();

			int verticesLength = reader.ReadInt32(); //Int32 numberofvertices
			boolean = reader.ReadBoolean();
			if (boolean)
			{
				verticesLength = ReadNumberMostlyBelow255(reader);
				vertices = reader.ReadBytes(verticesLength);
			}

			reader.ReadBoolean(); //Indices
			var indicesLength = ReadNumberMostlyBelow255(reader);
			typeOfByte = reader.ReadByte();
			reader.ReadString();
			indices = new short[indicesLength];
			for (int i = 0; i < indicesLength; i++)
				indices[i] = reader.ReadInt16();
			SetNativeData(vertices, indices);
		}

		private static int ReadNumberMostlyBelow255(BinaryReader reader)
		{
			int number = reader.ReadByte();
			if (number == 255)
				number = reader.ReadInt32();
			return number;
		}
	}
}