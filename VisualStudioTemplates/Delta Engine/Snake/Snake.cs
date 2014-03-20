using System.Collections.Generic;
using DeltaEngine.Datatypes;
using DeltaEngine.Entities;
using DeltaEngine.Rendering2D;

namespace $safeprojectname$
{
	/// <summary>
	/// This class holds data about the snake body and checks for snake collisions with either 
	/// itself or with the borders and whether the snake must grow in size.
	/// </summary>
	public sealed class Snake : Entity2D
	{
		public Snake(int gridSize, Color color)
			: base(Rectangle.Zero)
		{
			Add(new Body(gridSize, color));
			Start<SnakeHandler>();
		}

		public override void Dispose()
		{
			var body = Get<Body>();
			foreach (var bodyPart in body.BodyParts)
				bodyPart.Dispose();
			Stop<SnakeHandler>();
			Get<Body>().BodyParts.Clear();
			Remove<Body>();
			base.Dispose();
		}

		internal class SnakeHandler : UpdateBehavior
		{
			public override void Update(IEnumerable<Entity> entities)
			{
				foreach (var entity in entities)
				{
					if (!Time.CheckEvery(0.15f))
						return;
					var body = entity.Get<Body>();
					if(body.BodyParts.Count < 2)
						return;
					body.MoveBody();
					body.CheckSnakeCollidesWithChunk();
					body.CheckSnakeCollisionWithBorderOrItself();
				}
			}
		}
	}
}