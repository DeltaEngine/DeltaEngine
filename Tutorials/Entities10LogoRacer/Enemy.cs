using DeltaEngine.Content;
using DeltaEngine.Core;
using DeltaEngine.Datatypes;
using DeltaEngine.Rendering2D;

namespace DeltaEngine.Tutorials.Entities10LogoRacer
{
	public class Enemy : Sprite
	{
		public Enemy()
			: base(ContentLoader.Load<Material>("Earth"), Rectangle.FromCenter(
				new Vector2D(Randomizer.Current.Get(), 0.1f), new Size(0.1f * 1.35f, 0.1f)))
		{
			var data = new SimplePhysics.Data { Gravity = new Vector2D(0.0f, 0.1f), Duration = 10 };
			Add(data);
			Start<SimplePhysics.Move>();
			Start<SimplePhysics.KillAfterDurationReached>();
		}
	}
}