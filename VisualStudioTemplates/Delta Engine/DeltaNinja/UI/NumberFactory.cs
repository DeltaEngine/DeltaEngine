using System.Globalization;
using DeltaEngine.Content;
using DeltaEngine.Datatypes;
using DeltaEngine.Scenes;

namespace $safeprojectname$.UI
{
	internal class NumberFactory
	{
		public NumberFactory()
		{
			for (int digit = 0; digit < 10; digit++)
				materials[digit] = new Material(ShaderFlags.Position2DColoredTextured,
					digit.ToString(CultureInfo.InvariantCulture));
			materials[10] = new Material(ShaderFlags.Position2DColoredTextured, "Empty");
		}

		private readonly Material[] materials = new Material[11];

		public Number CreateNumber(Scene scene, float left, float top, float height, Alignment align,
			int digitCount, Color color, GameRenderLayer layer = GameRenderLayer.Hud)
		{
			return new Number(scene, materials, left, top, height, align, digitCount, color, layer);
		}
	}
}