using System;
using System.IO;
using DeltaEngine.Datatypes;

namespace DeltaEngine.Graphics.Vertices
{
	/// <summary>
	/// An element of a rendering vertex (like the position, the color, the normal, the uv, etc.)
	/// </summary>
	public class VertexElement : IEquatable<VertexElement>
	{
		private VertexElement() {} //ncrunch: no coverage

		public VertexElement(VertexElementType elementType)
		{
			ElementType = elementType;
			InitializeDataFromType();
		}

		//ncrunch: no coverage start
		public VertexElement(VertexElementType elementType, int size, int count)
		{
			ElementType = elementType;
			Size = size;
			ComponentCount = count;
		} //ncrunch: no coverage end

		public VertexElementType ElementType { get; private set; }

		private void InitializeDataFromType()
		{
			switch (ElementType)
			{
			case VertexElementType.Position3D:
			case VertexElementType.Normal:
				ComponentCount = 3;
				Size = 12;
				break;
			case VertexElementType.Position2D:
			case VertexElementType.TextureUV:
			case VertexElementType.LightMapUV:
				ComponentCount = 2;
				Size = 8;
				break;
			case VertexElementType.Color:
				ComponentCount = 4;
				Size = 4;
				break;
			case VertexElementType.SkinIndices:
			case VertexElementType.SkinWeights:
				ComponentCount = 2;
				Size = 8;
				break;
			default:
				throw new ElementTypeNotYetSupported(ElementType);
			}
		}

		public class ElementTypeNotYetSupported : Exception
		{
			public ElementTypeNotYetSupported(VertexElementType elementType)
				: base(elementType.ToString()) {}
		}

		public int Size { get; private set; }
		public int ComponentCount { get; private set; }

		public bool Equals(VertexElement other)
		{
			return ElementType == other.ElementType && ComponentCount == other.ComponentCount &&
				Size == other.Size;
		}

		public override string ToString()
		{
			return ElementType + "*" + ComponentCount;
		}

		public void SaveData(BinaryWriter writer, Vector3D position)
		{
			writer.Write(position.X);
			writer.Write(position.Y);
			writer.Write(position.Z);
		}

		public void SaveData(BinaryWriter writer, Vector2D position)
		{
			writer.Write(position.X);
			writer.Write(position.Y);
		}

		public void SaveData(BinaryWriter writer, Color color)
		{
			writer.Write(color.R);
			writer.Write(color.G);
			writer.Write(color.B);
			writer.Write(color.A);
		}

		public int Offset { get; internal set; }
	}
}