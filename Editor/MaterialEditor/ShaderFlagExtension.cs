using System.Collections.Generic;
using DeltaEngine.Content;

namespace DeltaEngine.Editor.MaterialEditor
{
	public class ShaderFlagExtension
	{
		public static IEnumerable<ShaderFlags> GetSupportedShaders()
		{
			return new[]
			{
				ShaderFlags.Position2DTextured, ShaderFlags.Position2DColored,
				ShaderFlags.Position2DColoredTextured, ShaderFlags.Textured, ShaderFlags.Colored,
				ShaderFlags.ColoredTextured, ShaderFlags.TexturedLightMap, ShaderFlags.LitTextured, 
				ShaderFlags.TexturedSkinned
			};
		}
	}
}