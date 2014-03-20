using System;
using DeltaEngine.Content;
using DeltaEngine.Graphics.Vertices;

namespace DeltaEngine.Graphics
{
	/// <summary>
	/// Adds graphics specific features to a shader object like VertexFormat and the shader code.
	/// </summary>
	public abstract class ShaderWithFormat : Shader
	{
		protected ShaderWithFormat(ShaderCreationData data)
			: base(data) {}

		protected ShaderWithFormat(ShaderWithFormatCreationData data) //ncrunch: no coverage
			: base(data) { } //ncrunch: no coverage

		protected override void FillShaderCode()
		{
			if (!(Data is ShaderWithFormatCreationData))
				Data = GetFormatCreationDataFromFlags();
			var data = Data as ShaderWithFormatCreationData;
			if (data.Format == null || data.Format.Elements.Length == 0)
				throw new InvalidVertexFormat();
			if (string.IsNullOrEmpty(data.VertexCode) || string.IsNullOrEmpty(data.FragmentCode) ||
				string.IsNullOrEmpty(data.DX9Code) || string.IsNullOrEmpty(data.DX11Code))
				throw new NoShaderCodeSpecifiedForOpenGLAndDirectX();
			Format = data.Format;
			OpenGLVertexCode = data.VertexCode;
			OpenGLFragmentCode = data.FragmentCode;
			DX11Code = data.DX11Code;
			DX9Code = data.DX9Code;
		}

		private ShaderWithFormatCreationData GetFormatCreationDataFromFlags()
		{
			switch (Data.Flags)
			{
			case ShaderFlags.Position2DTextured:
				return new ShaderWithFormatCreationData(Data.Flags,
					ShaderCodeOpenGL.PositionUVOpenGLVertexCode, ShaderCodeOpenGL.PositionUVOpenGLFragmentCode,
					ShaderCodeDX11.PositionUVDX11, ShaderCodeDX9.Position2DUVDX9, VertexFormat.Position2DUV);
			case ShaderFlags.Position2DColored:
				return new ShaderWithFormatCreationData(Data.Flags,
					ShaderCodeOpenGL.PositionColorOpenGLVertexCode,
					ShaderCodeOpenGL.PositionColorOpenGLFragmentCode, ShaderCodeDX11.PositionColorDX11,
					ShaderCodeDX9.Position2DColorDX9, VertexFormat.Position2DColor);
			case ShaderFlags.Position2DColoredTextured:
				return new ShaderWithFormatCreationData(Data.Flags,
					ShaderCodeOpenGL.PositionColorUVOpenGLVertexCode,
					ShaderCodeOpenGL.PositionColorUVOpenGLFragmentCode, ShaderCodeDX11.PositionColorUVDX11,
					ShaderCodeDX9.Position2DColorUVDX9, VertexFormat.Position2DColorUV);
			case ShaderFlags.Textured:
				return new ShaderWithFormatCreationData(Data.Flags,
					ShaderCodeOpenGL.PositionUVOpenGLVertexCode, ShaderCodeOpenGL.PositionUVOpenGLFragmentCode,
					ShaderCodeDX11.PositionUVDX11, ShaderCodeDX9.Position3DUVDX9, VertexFormat.Position3DUV);
			case ShaderFlags.Colored:
				return new ShaderWithFormatCreationData(Data.Flags,
					ShaderCodeOpenGL.PositionColorOpenGLVertexCode,
					ShaderCodeOpenGL.PositionColorOpenGLFragmentCode, ShaderCodeDX11.PositionColorDX11,
					ShaderCodeDX9.Position3DColorDX9, VertexFormat.Position3DColor);
			case ShaderFlags.ColoredTextured:
				return new ShaderWithFormatCreationData(Data.Flags,
					ShaderCodeOpenGL.PositionColorUVOpenGLVertexCode,
					ShaderCodeOpenGL.PositionColorUVOpenGLFragmentCode, ShaderCodeDX11.PositionColorUVDX11,
					ShaderCodeDX9.Position3DColorUVDX9, VertexFormat.Position3DColorUV);
			case ShaderFlags.TexturedLightMap:
				return new ShaderWithFormatCreationData(Data.Flags,
					ShaderCodeOpenGL.PositionUVLightmapVertexCode,
					ShaderCodeOpenGL.PositionUVLightmapFragmentCode, ShaderCodeDX11.UVLightmapHLSLCode,
					ShaderCodeDX9.DX9Position3DLightMap, VertexFormat.Position3DUVLightMap);
			case ShaderFlags.TexturedSkinned:
				return new ShaderWithFormatCreationData(Data.Flags,
					ShaderCodeOpenGL.PositionUVSkinnedOpenGLVertexCode,
					ShaderCodeOpenGL.PositionUVSkinnedOpenGLFragmentCode,
					ShaderCodeDX11.PositionUVSkinnedDX11, ShaderCodeDX9.PositionUVSkinnedDX9,
					VertexFormat.Position3DUVSkinned);
			case ShaderFlags.ColoredFog:
				return new ShaderWithFormatCreationData(Data.Flags,
					ShaderCodeOpenGL.PositionColorFogOpenGLVertexCode,
					ShaderCodeOpenGL.PositionColorFogOpenGLFragmentCode,
					ShaderCodeDX11.PositionColorFogDX11,
					ShaderCodeDX9.PositionColorFogDX9, VertexFormat.Position3DColor);
			case ShaderFlags.TexturedFog:
				return new ShaderWithFormatCreationData(Data.Flags,
					ShaderCodeOpenGL.PositionUVFogOpenGLVertexCode,
					ShaderCodeOpenGL.PositionUVFogOpenGLFragmentCode,
					ShaderCodeDX11.PositionUVFogDX11,
					ShaderCodeDX9.PositionUVFogDX9, VertexFormat.Position3DUV);
			case ShaderFlags.ColoredTexturedFog:
				return new ShaderWithFormatCreationData(Data.Flags,
					ShaderCodeOpenGL.PositionColorUVFogOpenGLVertexCode,
					ShaderCodeOpenGL.PositionColorUVFogOpenGLFragmentCode,
					ShaderCodeDX11.PositionColorUVFogDX11, ShaderCodeDX9.PositionColorUVFogDX9,
					VertexFormat.Position3DColorUV);
			case ShaderFlags.TexturedLightMapFog:
				return new ShaderWithFormatCreationData(Data.Flags,
					ShaderCodeOpenGL.PositionUVLightmapFogVertexCode,
					ShaderCodeOpenGL.PositionUVLightmapFogFragmentCode,
					ShaderCodeDX11.PositionUVLightmapFogDX11, ShaderCodeDX9.PositionUVLightmapFogDX9,
					VertexFormat.Position3DUVLightMap);
			case ShaderFlags.LitTextured:
				return new ShaderWithFormatCreationData(Data.Flags,
					ShaderCodeOpenGL.PositionUVNormalOpenGLVertexCode,
					ShaderCodeOpenGL.PositionUVNormalOpenGLFragmentCode,
					ShaderCodeDX11.PositionUVNormal, ShaderCodeDX9.PositionUVNormal,
					VertexFormat.Position3DNormalUV);					
			case ShaderFlags.TexturedSkinnedFog:
				return new ShaderWithFormatCreationData(Data.Flags,
					ShaderCodeOpenGL.PositionUVSkinnedFogOpenGLVertexCode,
					ShaderCodeOpenGL.PositionUVSkinnedFogOpenGLFragmentCode,
					ShaderCodeDX11.PositionUVSkinnedFogDX11, ShaderCodeDX9.PositionUVSkinnedFogDX9,
					VertexFormat.Position3DUVSkinned);
			default:
				throw new ShaderFlagsNotSupported(Data.Flags);
			}
		}

		public class ShaderFlagsNotSupported : Exception
		{
			public ShaderFlagsNotSupported(ShaderFlags flags)
				: base(flags.ToString()) {}
		}

		internal class InvalidVertexFormat : Exception {}

		internal class NoShaderCodeSpecifiedForOpenGLAndDirectX : Exception {}

		public VertexFormat Format { get; private set; }
		protected string OpenGLVertexCode { get; private set; }
		protected string OpenGLFragmentCode { get; private set; }
		protected string DX11Code { get; private set; }
		protected string DX9Code { get; private set; }
	}
}