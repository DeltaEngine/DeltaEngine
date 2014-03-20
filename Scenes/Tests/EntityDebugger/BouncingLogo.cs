using DeltaEngine.Content;
using DeltaEngine.Core;
using DeltaEngine.Datatypes;
using DeltaEngine.Rendering2D;

namespace DeltaEngine.Scenes.Tests.EntityDebugger
{
	internal class BouncingLogo : Sprite
	{
		public BouncingLogo()
			: base(new Material(ShaderFlags.Position2DColoredTextured, "DeltaEngineLogo"), Vector2D.Half)
		{
			Color = Color.GetRandomColor();
			Add(new SimplePhysics.Data
			{
				RotationSpeed = random.Get(-50, 50),
				Velocity = new Vector2D(random.Get(-0.4f, 0.4f), random.Get(-0.4f, 0.4f)),
			});
			Start<SimplePhysics.BounceIfAtScreenEdge>();
			Start<SimplePhysics.Rotate>();
		}

		private readonly Randomizer random = Randomizer.Current;
	}
}