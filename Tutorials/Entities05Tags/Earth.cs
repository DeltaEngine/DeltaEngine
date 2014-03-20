using DeltaEngine.Content;
using DeltaEngine.Datatypes;
using DeltaEngine.Rendering2D;

namespace DeltaEngine.Tutorials.Entities05Tags
{
	public class Earth : Sprite
	{
		public Earth(Vector2D position)
			: base(ContentLoader.Load<Material>("Earth"), position) {}
	}
}