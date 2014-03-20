using DeltaEngine.Content;
using DeltaEngine.Datatypes;
using DeltaEngine.Graphics.Vertices;
using NUnit.Framework;

namespace DeltaEngine.Graphics.Tests
{
	public class VertexFormatTests
	{
		[Test]
		public void VertexSizeInBytes()
		{
			Assert.AreEqual(16, VertexPosition2DUV.SizeInBytes);
			Assert.AreEqual(12, VertexPosition2DColor.SizeInBytes);
			Assert.AreEqual(20, VertexPosition2DColorUV.SizeInBytes);
			Assert.AreEqual(20, VertexPosition3DUV.SizeInBytes);
			Assert.AreEqual(16, VertexPosition3DColor.SizeInBytes);
			Assert.AreEqual(24, VertexPosition3DColorUV.SizeInBytes);
		}

		[Test]
		public void VertexPositionColorTextured2D()
		{
			var vertex = new VertexPosition2DColorUV(Vector2D.Zero, Color.Red, Vector2D.One);
			Assert.AreEqual(vertex.Position, Vector2D.Zero);
			Assert.AreEqual(vertex.Color, Color.Red);
			Assert.AreEqual(vertex.UV, Vector2D.One);
		}

		[Test]
		public void LerpPositionColorTextured2D()
		{
			var vertex = new VertexPosition2DColorUV(Vector2D.UnitX, Color.White, Vector2D.One);
			var vertex2 = new VertexPosition2DColorUV(Vector2D.UnitY, Color.Black, Vector2D.Zero);
			var lerpedVertex = vertex.Lerp(vertex2, 0.5f);
			Assert.AreEqual(lerpedVertex.Position, Vector2D.Half);
			Assert.AreEqual(lerpedVertex.Color, new Color(127, 127, 127));
			Assert.AreEqual(lerpedVertex.UV, Vector2D.One);
		}

		[Test]
		public void LerpPositionTextured2D()
		{
			var vertex = new VertexPosition2DUV(Vector2D.UnitX, Vector2D.One);
			var vertex2 = new VertexPosition2DUV(Vector2D.UnitY, Vector2D.Zero);
			var lerpedVertex = vertex.Lerp(vertex2, 0.5f);
			Assert.AreEqual(lerpedVertex.Position, Vector2D.Half);
			Assert.AreEqual(lerpedVertex.UV, Vector2D.One);
		}

		[Test]
		public void VertexPositionColorTextured3D()
		{
			Assert.AreEqual(VertexPosition3DColorUV.SizeInBytes, 24);
			var vertex = new VertexPosition3DColorUV(Vector3D.UnitX, Color.Red, Vector2D.One);
			Assert.AreEqual(vertex.Format, VertexFormat.Position3DColorUV);
			Assert.AreEqual(vertex.Position, Vector3D.UnitX);
			Assert.AreEqual(vertex.Color, Color.Red);
			Assert.AreEqual(vertex.UV, Vector2D.One);
		}

		[Test]
		public void LerpPositionColorTextured3D()
		{
			var vertex = new VertexPosition3DColorUV(Vector3D.UnitX, Color.White, Vector2D.One);
			var vertex2 = new VertexPosition3DColorUV(Vector2D.UnitY, Color.Black, Vector2D.Zero);
			var lerpedVertex = vertex.Lerp(vertex2, 0.5f);
			Assert.AreEqual(lerpedVertex.Position, new Vector3D(0.5f, 0.5f, 0f));
			Assert.AreEqual(lerpedVertex.Color, new Color(127, 127, 127));
			Assert.AreEqual(lerpedVertex.UV, Vector2D.One);
		}

		[Test]
		public void VertexElementPosition3D()
		{
			var element = new VertexElement(VertexElementType.Position3D);
			Assert.AreEqual(VertexElementType.Position3D, element.ElementType);
			Assert.AreEqual(3, element.ComponentCount);
			Assert.AreEqual(12, element.Size);
		}

		[Test]
		public void VertexPositionTextured3D()
		{
			Assert.AreEqual(VertexPosition3DUV.SizeInBytes, 20);
			var vertex = new VertexPosition3DUV(Vector3D.UnitX, Vector2D.One);
			Assert.AreEqual(vertex.Format, VertexFormat.Position3DUV);
			Assert.AreEqual(vertex.Position, Vector3D.UnitX);
			Assert.AreEqual(vertex.UV, Vector2D.One);
		}

		[Test]
		public void LerpPositionTextured3D()
		{
			var vertex = new VertexPosition3DUV(Vector3D.UnitX, Vector2D.One);
			var vertex2 = new VertexPosition3DUV(Vector2D.UnitY, Vector2D.Zero);
			var lerpedVertex = vertex.Lerp(vertex2, 0.5f);
			Assert.AreEqual(lerpedVertex.Position, new Vector3D(0.5f, 0.5f, 0f));
			Assert.AreEqual(lerpedVertex.UV, Vector2D.One);
		}

		[Test]
		public void LerpPositionColor3D()
		{
			var vertex = new VertexPosition3DColor(Vector3D.UnitX, Color.White);
			var vertex2 = new VertexPosition3DColor(Vector2D.UnitY, Color.Black);
			var lerpedVertex = vertex.Lerp(vertex2, 0.5f);
			Assert.AreEqual(lerpedVertex.Position, new Vector3D(0.5f, 0.5f, 0f));
			Assert.AreEqual(lerpedVertex.Color, new Color(127, 127, 127));
		}

		[Test]
		public void VertexElementTextureUV()
		{
			var element = new VertexElement(VertexElementType.TextureUV);
			Assert.AreEqual(VertexElementType.TextureUV, element.ElementType);
			Assert.AreEqual(2, element.ComponentCount);
			Assert.AreEqual(8, element.Size);
		}

		[Test]
		public void VertexElementColor()
		{
			var element = new VertexElement(VertexElementType.Color);
			Assert.AreEqual(VertexElementType.Color, element.ElementType);
			Assert.AreEqual(4, element.ComponentCount);
			Assert.AreEqual(4, element.Size);
		}

		[Test]
		public void VertexFormatPosition3DTextureUVColor()
		{
			var elements = new[] {
				new VertexElement(VertexElementType.Position3D),
				new VertexElement(VertexElementType.TextureUV),
				new VertexElement(VertexElementType.Color)};
			var format = new VertexFormat(elements);
			Assert.AreEqual(24, format.Stride);
			Assert.AreEqual(0, elements[0].Offset);
			Assert.AreEqual(12, elements[1].Offset);
			Assert.AreEqual(20, elements[2].Offset);
		}

		[Test]
		public void VertexFormatVertexPosition3DColorSkinned()
		{
			var format = VertexFormat.Position3DColorSkinned;
			Assert.AreEqual(32, format.Stride);
			Assert.AreEqual(0, format.Elements[0].Offset);
			Assert.AreEqual(12, format.Elements[1].Offset);
			Assert.AreEqual(16, format.Elements[2].Offset);
			Assert.AreEqual(24, format.Elements[3].Offset);
		}

		[Test]
		public void CheckVertexPosition3DColorSkinned()
		{
			Assert.AreEqual(32, VertexPosition3DColorSkinned.SizeInBytes);
			var skinning = new SkinningData(0, 0, 0.0f, 0.0f);
			var vertex = new VertexPosition3DColorSkinned(Vector3D.UnitX, Color.White, skinning);
			Assert.AreEqual(vertex.Format, VertexFormat.Position3DColorSkinned);
			Assert.AreEqual(Vector3D.UnitX, vertex.Position);
			Assert.AreEqual(Color.White, vertex.Color);
			Assert.AreEqual(skinning, vertex.Skinning);
			Assert.AreEqual(32, VertexPosition3DColorSkinned.SizeInBytes);
		}

		[Test]
		public void LerpVertexPosition3DColorSkinned()
		{
			var vertex = new VertexPosition3DColorSkinned(Vector3D.UnitX, Color.White,
				new SkinningData(0, 1, 1.0f, 1.0f));
			var vertex2 = new VertexPosition3DColorSkinned(Vector2D.UnitY, Color.Black,
				new SkinningData(0, 1, 0.0f, 0.0f));
			var lerpedVertex = vertex.Lerp(vertex2, 0.5f);
			Assert.AreEqual(lerpedVertex.Position, new Vector3D(0.5f, 0.5f, 0f));
			Assert.AreEqual(lerpedVertex.Color, new Color(127, 127, 127));
			Assert.AreEqual(lerpedVertex.Skinning, new SkinningData(0, 1, 0.5f, 0.5f));
		}

		[Test]
		public void VertexFormatVertexPosition3DUVSkinned()
		{
			var format = VertexFormat.Position3DUVSkinned;
			Assert.AreEqual(36, format.Stride);
			Assert.AreEqual(0, format.Elements[0].Offset);
			Assert.AreEqual(12, format.Elements[1].Offset);
			Assert.AreEqual(20, format.Elements[2].Offset);
			Assert.AreEqual(28, format.Elements[3].Offset);
		}

		[Test]
		public void VertexPosition3DUVSkinned()
		{
			var skinning = new SkinningData(0, 0, 0.0f, 0.0f);
			var vertex = new VertexPosition3DUVSkinned(Vector3D.UnitX, Vector2D.One, skinning);
			Assert.AreEqual(vertex.Format, VertexFormat.Position3DUVSkinned);
			Assert.AreEqual(Vector3D.UnitX, vertex.Position);
			Assert.AreEqual(Vector2D.One, vertex.UV);
			Assert.AreEqual(skinning, vertex.Skinning);
		}

		[Test]
		public void LerpVertexPosition3DUVSkinned()
		{
			var vertex = new VertexPosition3DUVSkinned(Vector3D.UnitX, Vector2D.Zero,
				new SkinningData(0, 1, 1.0f, 1.0f));
			var vertex2 = new VertexPosition3DUVSkinned(Vector2D.UnitY, Vector2D.One,
				new SkinningData(0, 1, 0.0f, 0.0f));
			var lerpedVertex = vertex.Lerp(vertex2, 0.5f);
			Assert.AreEqual(lerpedVertex.Position, new Vector3D(0.5f, 0.5f, 0f));
			Assert.AreEqual(lerpedVertex.UV, Vector2D.Half);
			Assert.AreEqual(lerpedVertex.Skinning, new SkinningData(0, 1, 0.5f, 0.5f));
		}

		[Test]
		public void VertexFormatGetVertexElement()
		{
			var elements = new[] {
				new VertexElement(VertexElementType.Position3D),
				new VertexElement(VertexElementType.TextureUV) };
			var format = new VertexFormat(elements);
			Assert.IsNull(format.GetElementFromType(VertexElementType.Color));
			Assert.IsNotNull(format.GetElementFromType(VertexElementType.TextureUV));
		}

		[Test]
		public void AreEqual()
		{
			var elements = new[] {
				new VertexElement(VertexElementType.Position3D),
				new VertexElement(VertexElementType.TextureUV) };
			var format = new VertexFormat(elements);
			Assert.IsTrue(VertexFormat.Position3DUV.Equals(format));
			Assert.IsTrue(VertexFormat.Position3DUV.Equals((object)format));
			Assert.AreEqual(VertexFormat.Position3DUV, format);
			Assert.IsTrue(VertexFormat.Position3DUV == format);
			Assert.IsTrue(VertexFormat.Position2DUV.Equals(VertexFormat.Position2DUV));
			Assert.IsFalse(VertexFormat.Position2DUV == VertexFormat.Position2DColor);
			Assert.AreEqual(VertexFormat.Position2DUV, VertexFormat.Position2DUV);
			Assert.AreNotEqual(VertexFormat.Position2DUV, VertexFormat.Position2DColor);
			VertexFormat unassignedFormat = null;
			Assert.IsTrue(unassignedFormat == null);
		}

		[Test]
		public void FormatToString()
		{
			var elements = new[] {
				new VertexElement(VertexElementType.Position3D),
				new VertexElement(VertexElementType.TextureUV) };
			var format = new VertexFormat(elements);
			Assert.AreEqual("VertexFormat: Position3D*3, TextureUV*2, Stride=20", format.ToString());
		}

		[Test]
		public void HasProperties()
		{
			var elements = new[] {
				new VertexElement(VertexElementType.Position3D),
				new VertexElement(VertexElementType.TextureUV) };
			var format = new VertexFormat(elements);
			Assert.IsTrue(format.HasUV);
			Assert.IsTrue(format.Is3D);
			Assert.IsFalse(format.HasColor);
			Assert.IsFalse(format.HasNormal);
			Assert.IsFalse(format.HasLightmap);
		}

		[Test]
		public void LerpVertexPosition3DNormalUV()
		{
			var v1 = new VertexPosition3DNormalUV(Vector3D.Zero, Vector3D.UnitZ, Vector2D.Zero);
			var v2 = new VertexPosition3DNormalUV(Vector2D.One, Vector3D.UnitX, Vector2D.One);
			var lerp = v1.Lerp(v2, 0.5f);
			Assert.AreEqual(new Vector3D(0.5f, 0.5f, 0f), lerp.Position);
			Assert.AreEqual(new Vector3D(0.5f, 0f, 0.5f), lerp.Normal);
			Assert.AreEqual(Vector2D.Zero, lerp.UV);
		}

		[Test]
		public void VertexPosition3DNormalUVSizeInBytes()
		{
			Assert.AreEqual(32, VertexPosition3DNormalUV.SizeInBytes);
		}

		[Test]
		public void VertexPosition3DFormat()
		{
			var v1 = new VertexPosition3DNormalUV(Vector3D.Zero, Vector3D.UnitZ, Vector2D.Zero);
			Assert.AreEqual(VertexFormat.Position3DNormalUV, v1.Format);
		}

		[Test]
		public void CheckPosition3DUVLightmapFormat()
		{
			Assert.AreEqual(VertexFormat.Position3DUVLightMap.Stride, 28);
		}

		[Test]
		public void LerpShouldReturnARenderingDataObject()
		{
			var data = new RenderingData();
			var newRenderingDataObject = data.Lerp(data, 0.5f);
			Assert.IsNotNull(newRenderingDataObject);
		}
	}
}