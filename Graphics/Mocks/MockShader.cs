using DeltaEngine.Content;
using DeltaEngine.Datatypes;

namespace DeltaEngine.Graphics.Mocks
{
	public class MockShader : ShaderWithFormat
	{
		public MockShader(ShaderWithFormatCreationData creationData, Device device)
			: this((ShaderCreationData)creationData, device) {}

		public MockShader(ShaderCreationData customShader, Device device)
			: base(customShader)
		{
			this.device = device;
			CallPublicImplementationMethodsToFixCoverage();
		}

		private readonly Device device;

		protected override void FillShaderCode()
		{
			base.FillShaderCode();
			TryCreateShader();
		}

		protected override void CreateShader()
		{
			const string BadKeyword = "Bad";
			if (OpenGLVertexCode.Contains(BadKeyword))
				ThrowShaderCreationException("OpenGLVertexCode");
			if (OpenGLFragmentCode.Contains(BadKeyword))
				ThrowShaderCreationException("OpenGLFragmentCode");
			if (DX11Code.Contains(BadKeyword))
				ThrowShaderCreationException("DX11Code");
			if (DX9Code.Contains(BadKeyword))
				ThrowShaderCreationException("DX9Code");
		}

		private static void ThrowShaderCreationException(string shaderCodeType)
		{
			throw new ShaderCreationHasFailed(shaderCodeType + " is no valid shader code");
		}

		private void CallPublicImplementationMethodsToFixCoverage()
		{
			SetModelViewProjection(Matrix.Identity);
			SetJointMatrices(new Matrix[0]);
			SetDiffuseTexture(null);
			SetLightmapTexture(null);
			Bind();
			BindVertexDeclaration();
			ApplyFogSettings(null);
		}

		protected override void DisposeData() {}

		public override void SetModelViewProjection(Matrix model, Matrix view, Matrix projection) {}
		public override void SetModelViewProjection(Matrix matrix) {}
		public override void SetJointMatrices(Matrix[] jointMatrices) {}
		public override void SetDiffuseTexture(Image texture) {}
		public override void SetLightmapTexture(Image texture) {}
		public override void SetSunLight(SunLight light) {}

		public override void Bind()
		{
			device.Shader = this;
		}

		public override void BindVertexDeclaration() {}
		public override void ApplyFogSettings(FogSettings fogSettings) {}
	}
}