using DeltaEngine.Content;
using NUnit.Framework;

namespace DeltaEngine.Tests.Content
{
	public class ShaderTests
	{
		[Test]
		public void ShadersAreEqualWhenTheFlagsAreTheSame()
		{
			Shader coloredShader = CreateShaderByFlags(ShaderFlags.Colored);
			Assert.AreEqual(coloredShader, CreateShaderByFlags(ShaderFlags.Colored));
			Assert.AreNotEqual(coloredShader, CreateShaderByFlags(ShaderFlags.Textured));
		}

		private static Shader CreateShaderByFlags(ShaderFlags flags)
		{
			var creationData = new ShaderCreationData(flags);
			return new FakeShader(creationData);
		}
	}
}