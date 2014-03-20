using DeltaEngine.Content;
using DeltaEngine.Datatypes;
using DeltaEngine.Rendering2D;

namespace DeltaEngine.Tutorials.Entities04ChangingBehavior
{
	public class Earth : Sprite
	{
		public Earth(Vector2D position)
			: base(ContentLoader.Load<Material>("Earth"), position)
		{
			var data = new SimplePhysics.Data
			{
				Gravity = new Vector2D(0.0f, 0.1f),
				Bounced = () => Color = Color.GetRandomColor()
			};
			Add(data);
			Start<SimplePhysics.BounceIfAtScreenEdge>();
			Start<SimplePhysics.Move>();
		}
	}
}