using System;
using System.Linq;
using DeltaEngine.Extensions;

namespace DeltaEngine.Graphics.Vertices
{
	/// <summary>
	/// The format for a vertex, e.g. it contains position, color and texture.
	/// </summary>
	public class VertexFormat : IEquatable<VertexFormat>
	{
		private VertexFormat() {} //ncrunch: no coverage

		public VertexFormat(VertexElement[] elements)
		{
			Elements = elements;
			foreach (var vertexElement in elements)
				ComputeElementOffset(vertexElement);
		}

		public VertexElement[] Elements { get; private set; }

		private void ComputeElementOffset(VertexElement vertexElement)
		{
			vertexElement.Offset = Stride;
			Stride += vertexElement.Size;
		}

		public int Stride { get; private set; }

		public bool Is3D
		{
			get { return Elements.Any(t => t.ElementType == VertexElementType.Position3D); }
		}
		public bool HasColor
		{
			get { return Elements.Any(t => t.ElementType == VertexElementType.Color); }
		}
		public bool HasUV
		{
			get { return Elements.Any(t => t.ElementType == VertexElementType.TextureUV); }
		}
		public bool HasNormal
		{
			get { return Elements.Any(t => t.ElementType == VertexElementType.Normal); }
		}
		public bool HasLightmap
		{
			get { return Elements.Any(t => t.ElementType == VertexElementType.LightMapUV); }
		}

		public VertexElement GetElementFromType(VertexElementType type)
		{
			return Elements.FirstOrDefault(vertexElement => vertexElement.ElementType == type);
		}

		public static bool operator ==(VertexFormat f1, VertexFormat f2)
		{
			return ReferenceEquals(f1, f2) || (object)f1 != null && f1.Equals(f2);
		}

		public bool Equals(VertexFormat other)
		{
			if ((object)other == null || Elements.Length != other.Elements.Length)
				return false;
			for (int i = 0; i < Elements.Length; ++i)
				if (!Elements[i].Equals(other.Elements[i]))
					return false;
			return true;
		}

		public static bool operator !=(VertexFormat f1, VertexFormat f2)
		{
			return (object)f1 == null || !f1.Equals(f2);
		}

		public override bool Equals(object other)
		{
			return other is VertexFormat && Equals((VertexFormat)other);
		}

		//ncrunch: no coverage start
		public override int GetHashCode()
		{
			return Elements.GetHashCode() ^ Stride.GetHashCode();
		} //ncrunch: no coverage end

		public override string ToString()
		{
			return "VertexFormat: " + Elements.ToText() + ", Stride=" + Stride;
		}

		public static readonly VertexFormat Position2DUV =
			new VertexFormat(new[]
			{
				new VertexElement(VertexElementType.Position2D),
				new VertexElement(VertexElementType.TextureUV)
			});

		public static readonly VertexFormat Position2DColor =
			new VertexFormat(new[]
			{
				new VertexElement(VertexElementType.Position2D),
				new VertexElement(VertexElementType.Color)
			});

		public static readonly VertexFormat Position2DColorUV =
			new VertexFormat(new[]
			{
				new VertexElement(VertexElementType.Position2D),
				new VertexElement(VertexElementType.Color),
				new VertexElement(VertexElementType.TextureUV)
			});

		public static readonly VertexFormat Position3DUV =
			new VertexFormat(new[]
			{
				new VertexElement(VertexElementType.Position3D),
				new VertexElement(VertexElementType.TextureUV)
			});

		public static readonly VertexFormat Position3DColor =
			new VertexFormat(new[]
			{
				new VertexElement(VertexElementType.Position3D),
				new VertexElement(VertexElementType.Color)
			});

		public static readonly VertexFormat Position3DColorUV =
			new VertexFormat(new[]
			{
				new VertexElement(VertexElementType.Position3D),
				new VertexElement(VertexElementType.Color),
				new VertexElement(VertexElementType.TextureUV)
			});

		public static readonly VertexFormat Position3DNormalUV =
			new VertexFormat(new[]
			{
				new VertexElement(VertexElementType.Position3D),
				new VertexElement(VertexElementType.Normal),
				new VertexElement(VertexElementType.TextureUV)
			});

		public static readonly VertexFormat Position3DUVLightMap =
			new VertexFormat(new[]
			{
				new VertexElement(VertexElementType.Position3D),
				new VertexElement(VertexElementType.TextureUV),
				new VertexElement(VertexElementType.LightMapUV)
			});

		public static readonly VertexFormat Position3DSkinned =
			new VertexFormat(new[]
			{
				new VertexElement(VertexElementType.Position3D),
				new VertexElement(VertexElementType.SkinIndices),
				new VertexElement(VertexElementType.SkinWeights)
			});

		public static readonly VertexFormat Position3DColorSkinned =
			new VertexFormat(new[]
			{
				new VertexElement(VertexElementType.Position3D),
				new VertexElement(VertexElementType.Color), 
				new VertexElement(VertexElementType.SkinIndices),
				new VertexElement(VertexElementType.SkinWeights)
			});

		public static readonly VertexFormat Position3DUVSkinned =
			new VertexFormat(new[]
			{
				new VertexElement(VertexElementType.Position3D),
				new VertexElement(VertexElementType.TextureUV),
				new VertexElement(VertexElementType.SkinIndices),
				new VertexElement(VertexElementType.SkinWeights)
			});

		public static readonly VertexFormat Position3DNormalUVSkinned =
			new VertexFormat(new[]
			{
				new VertexElement(VertexElementType.Position3D),
				new VertexElement(VertexElementType.Normal),
				new VertexElement(VertexElementType.TextureUV),
				new VertexElement(VertexElementType.SkinIndices),
				new VertexElement(VertexElementType.SkinWeights)
			});
	}
}