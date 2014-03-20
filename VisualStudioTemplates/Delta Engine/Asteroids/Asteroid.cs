using System.Collections.Generic;
using System.Linq;
using DeltaEngine.Content;
using DeltaEngine.Core;
using DeltaEngine.Datatypes;
using DeltaEngine.Entities;
using DeltaEngine.Rendering2D;
using DeltaEngine.ScreenSpaces;

namespace $safeprojectname$
{
	/// <summary>
	/// representing an asteroid that can be destroyed, causing it to split into two objects of half the
	/// size at first. Those of half size can split again and those of third part the size shall vanish.
	/// </summary>
	public class Asteroid : Sprite
	{
		public Asteroid(InteractionLogic interactionLogic, int sizeModifier = 1)
			: this(CreateDrawArea(Randomizer.Current, sizeModifier), interactionLogic, sizeModifier) {}

		public Asteroid(Vector2D position, InteractionLogic interactionLogic, int sizeModifier = 1)
			: this(Rectangle.FromCenter(position, new Size(0.1f / sizeModifier)), interactionLogic,
				sizeModifier) {}

		private Asteroid(Rectangle drawArea, InteractionLogic interactionLogic, int sizeModifier)
			: base(new Material(ShaderFlags.Position2DColoredTextured, "Asteroid"), drawArea)
		{
			var randomizer = Randomizer.Current;
			this.interactionLogic = interactionLogic;
			this.sizeModifier = sizeModifier;
			RenderLayer = (int)AsteroidsRenderLayer.Asteroids;
			var data = new SimplePhysics.Data
			{
				Gravity = Vector2D.Zero,
				Velocity = GetInitialVelocity(randomizer),
				RotationSpeed = randomizer.Get(.1f, 50)
			};
			Add(data);
			Start<SimplePhysics.Move>();
			Start<MoveCrossingScreenEdges>();
			Start<SimplePhysics.Rotate>();
		}

		private static Rectangle CreateDrawArea(Randomizer randomizer, int sizeModifier)
		{
			var randomPosition =
				new Vector2D(
					randomizer.Get(-100, 100) > 0 ? ScreenSpace.Current.Left - .1f : ScreenSpace.Current.Right,
					randomizer.Get(-100, 100) > 0 ? ScreenSpace.Current.Top - .1f : ScreenSpace.Current.Bottom);
			var modifiedSize = new Size(.1f / sizeModifier);
			return new Rectangle(randomPosition, modifiedSize);
		}

		private Vector2D GetInitialVelocity(Randomizer randomizer)
		{
			var randomizedAxisValue = randomizer.Get(.03f, .15f);
			var randomXAimingForCenter = (Center.X > 0.5f) ? -randomizedAxisValue : randomizedAxisValue;
			var randomYAimingForCenter = (Center.Y > 0.5f) ? -randomizedAxisValue : randomizedAxisValue;
			return new Vector2D(randomXAimingForCenter, randomYAimingForCenter);
		}

		public readonly int sizeModifier;
		private readonly InteractionLogic interactionLogic;

		public void Fracture()
		{
			if (sizeModifier < 3)
				interactionLogic.CreateAsteroidsAtPosition(DrawArea.Center, sizeModifier + 1);
			interactionLogic.IncrementScore(1);
			IsActive = false;
		}

		public class MoveCrossingScreenEdges : UpdateBehavior
		{
			public MoveCrossingScreenEdges()
				: base(Priority.High) {}

			public override void Update(IEnumerable<Entity> entities)
			{
				foreach (var entity in entities.OfType<Asteroid>())
				{
					var physics = entity.Get<SimplePhysics.Data>();
					entity.SetToOppositeEdgeIfColliding(ScreenSpace.Current.Viewport);
					entity.DrawArea = new Rectangle(entity.TopLeft + physics.Velocity * Time.Delta,
						entity.Size);
				}
			}
		}

		public void SetToOppositeEdgeIfColliding(Rectangle borders)
		{
			var velocity = Get<SimplePhysics.Data>().Velocity;
			if (DrawArea.Width >= borders.Width || DrawArea.Height >= borders.Height)
				return; //ncrunch: no coverage, this does not actually happen
			if (DrawArea.Right < borders.Left && velocity.X < 0)
				SetWithoutInterpolation(new Rectangle(borders.Right, DrawArea.Top, DrawArea.Width,
					DrawArea.Height));
			if (DrawArea.Left > borders.Right && velocity.X > 0)
				SetWithoutInterpolation(new Rectangle(borders.Left - DrawArea.Width, DrawArea.Top,
					DrawArea.Width, DrawArea.Height));
			if (DrawArea.Bottom < borders.Top && velocity.Y < 0)
				SetWithoutInterpolation(new Rectangle(DrawArea.Left, borders.Bottom, DrawArea.Width,
					DrawArea.Height));
			if (DrawArea.Top > borders.Bottom && velocity.Y > 0)
				SetWithoutInterpolation(new Rectangle(DrawArea.Left, borders.Top - DrawArea.Height,
					DrawArea.Width, DrawArea.Height));
		}
	}
}