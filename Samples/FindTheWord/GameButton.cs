using DeltaEngine.Content;
using DeltaEngine.Datatypes;
using DeltaEngine.Scenes.Controls;

namespace FindTheWord
{
	public class GameButton : Button
	{
		public GameButton(string image, Rectangle drawArea)
			: base(new Theme { Button = new Material(ShaderFlags.Position2DTextured, image) }, drawArea) {}
	}
}