using DeltaEngine.Content;
using DeltaEngine.Graphics.Vertices;

namespace DeltaEngine.Graphics
{
	/// <summary>
	/// Creates a shader directly from vertex and fragment shader code for OpenGL frameworks plus
	/// HLSL code for DirectX frameworks. If you only provide shader code for a specific framework, 
	/// it breaks multiplatform compatibility. Use this only for testing and use content normally. 
	/// </summary>
	public class ShaderWithFormatCreationData : ShaderCreationData
	{
		protected ShaderWithFormatCreationData() {}

		public ShaderWithFormatCreationData(ShaderFlags flags, string glVertexCode,
			string glFragmentCode, string dx11Code, string dx9Code, VertexFormat format)
			: base(flags)
		{
			VertexCode = glVertexCode;
			FragmentCode = glFragmentCode;
			DX11Code = dx11Code;
			DX9Code = dx9Code;
			Format = format;
		}

		public string VertexCode { get; private set; }
		public string FragmentCode { get; private set; }
		public string DX11Code { get; private set; }
		public string DX9Code { get; private set; }
		public VertexFormat Format { get; private set; }
	}
}