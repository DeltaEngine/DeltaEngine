using System;
using System.IO;
using DeltaEngine.Content;
using DeltaEngine.Core;
using DeltaEngine.Graphics.Mocks;
using DeltaEngine.Graphics.Vertices;
using DeltaEngine.Platforms;
using NUnit.Framework;

namespace DeltaEngine.Graphics.Tests
{
	public class ShaderWithFormatTests : TestWithMocksOrVisually
	{
		[Test, CloseAfterFirstFrame]
		public void InvalidVertexFormat()
		{
			var invalidData = new ShaderWithFormatCreationData(ShaderFlags.None, "", "", "", "", null);
			Assert.Throws<ShaderWithFormat.InvalidVertexFormat>(
				() => ContentLoader.Create<Shader>(invalidData));
			var emptyFormat = new ShaderWithFormatCreationData(ShaderFlags.None, "", "", "", "",
				new VertexFormat(new VertexElement[0]));
			Assert.Throws<ShaderWithFormat.InvalidVertexFormat>(
				() => ContentLoader.Create<Shader>(emptyFormat));
		}

		[Test, CloseAfterFirstFrame]
		public void InvalidVertexAndPixelCode()
		{
			var data = new ShaderWithFormatCreationData(ShaderFlags.None, "", "", "", "",
				VertexFormat.Position2DColor);
			Assert.Throws<ShaderWithFormat.NoShaderCodeSpecifiedForOpenGLAndDirectX>(
				() => ContentLoader.Create<Shader>(data));
		}

		[Test, CloseAfterFirstFrame]
		public void AllowDynamicCreationViaCreationData()
		{
			var data = new ShaderWithFormatCreationData(ShaderFlags.None, "AnyData", "AnyData",
				"AnyData", "AnyData", VertexFormat.Position2DColor);
			var shader = ContentLoader.Create<NoDataShaderWithFormat>(data);
			Assert.DoesNotThrow(() => shader.ReloadCreationData(data));
		}

		private class NoDataShaderWithFormat : MockShader
		{
			public NoDataShaderWithFormat(ShaderWithFormatCreationData creationData, Device device)
				: base(creationData, device) { }

			public void ReloadCreationData(ShaderWithFormatCreationData creationData)
			{
				byte[] rawData = creationData.VertexCode == "NoData"
					? new byte[0] : BinaryDataExtensions.ToByteArrayWithTypeInformation(creationData);
				LoadData(new MemoryStream(rawData));
			}
		}

		[Test, CloseAfterFirstFrame]
		public void InvalidShaderFlagsThrowsException()
		{
			Assert.Throws<ShaderWithFormat.ShaderFlagsNotSupported>(
				() => new InvalidShaderWithFormat(Resolve<Device>()));
		}

		private class InvalidShaderWithFormat : MockShader
		{
			public InvalidShaderWithFormat(Device device)
				: base(new ShaderCreationData(InvalidShaderFlags), device) {} //ncrunch: no coverage

			private const ShaderFlags InvalidShaderFlags =
				ShaderFlags.TexturedLightMap | ShaderFlags.TexturedSkinned;
		}

		[Test, CloseAfterFirstFrame]
		public void CreationOfShaderWithInvalidShaderCodeMustFail()
		{
			var creationData = new ShaderWithFormatCreationData(ShaderFlags.Colored, "BadGLVertexCode",
				"BadGLFragmentCode", "BadDX11Code", "BadDX9Code", VertexFormat.Position3DColor);
			AssertShaderCreationException(creationData);
			creationData = new ShaderWithFormatCreationData(ShaderFlags.Colored, "AnyData",
				"BadGLFragmentCode", "BadDX11Code", "BadDX9Code", VertexFormat.Position3DColor);
			AssertShaderCreationException(creationData);
			creationData = new ShaderWithFormatCreationData(ShaderFlags.Colored, "AnyData", "AnyData",
				"BadDX11Code", "BadDX9Code", VertexFormat.Position3DColor);
			AssertShaderCreationException(creationData);
			creationData = new ShaderWithFormatCreationData(ShaderFlags.Colored, "AnyData", "AnyData",
				"AnyData", "BadDX9Code", VertexFormat.Position3DColor);
			AssertShaderCreationException(creationData);
		}

		private static void AssertShaderCreationException(ShaderWithFormatCreationData creationData)
		{
			try
			{
				ContentLoader.Create<ShaderWithFormat>(creationData);
			} //ncrunch: no coverage
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
				Assert.IsTrue(ex is Shader.ShaderCreationHasFailed, ex.ToString());
			}
		}

		[Test, CloseAfterFirstFrame]
		public void CreateTextureLightMapShader()
		{
			var shader = new TextureLightMapShaderWithFormat(Resolve<Device>());
			Assert.IsTrue(shader.Format.HasLightmap);
			Assert.IsTrue(shader.Format.HasUV);
		}

		private class TextureLightMapShaderWithFormat : MockShader
		{
			public TextureLightMapShaderWithFormat(Device device)
				: base(new ShaderCreationData(ShaderFlags.TexturedLightMap), device) { } //ncrunch: no coverage
		}

		[Test, CloseAfterFirstFrame]
		public void CreateColoredFogShader()
		{
			var shader = new ColoredFogShaderWithFormat(Resolve<Device>());
			Assert.IsTrue(shader.Format.HasColor);
			Assert.IsTrue(shader.Format.Is3D);
		}

		private class ColoredFogShaderWithFormat : MockShader
		{
			public ColoredFogShaderWithFormat(Device device)
				: base(new ShaderCreationData(ShaderFlags.ColoredFog), device) { } //ncrunch: no coverage
		}
	}
}