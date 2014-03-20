using System.IO;
using DeltaEngine.Content;
using DeltaEngine.Datatypes;
using DeltaEngine.Graphics;

namespace DeltaEngine.Tests.Content
{
	public class FakeShader : Shader
	{
		//ncrunch: no coverage start
		internal FakeShader(ShaderWithFormatCreationData creationData)
			: base(creationData) {}

		internal FakeShader(ShaderCreationData creationData)
			: base(creationData) {}

		protected override void DisposeData() {}
		protected override void LoadData(Stream fileData) {}
		protected override void FillShaderCode() {}
		protected override void CreateShader() {}

		public override void SetModelViewProjection(Matrix model, Matrix view, Matrix projection) {}
		public override void SetModelViewProjection(Matrix matrix) {}
		public override void SetJointMatrices(Matrix[] jointMatrices) {}
		public override void SetDiffuseTexture(Image texture) {}
		public override void SetLightmapTexture(Image texture) {}
		public override void SetSunLight(SunLight light) {}
		public override void Bind() {}
		public override void BindVertexDeclaration() {}
		public override void ApplyFogSettings(FogSettings fogSettings) {}
	}
}