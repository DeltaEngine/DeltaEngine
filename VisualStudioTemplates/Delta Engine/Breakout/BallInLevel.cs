using DeltaEngine.Rendering2D;

namespace $safeprojectname$
{
	/// <summary>
	/// Extends the ball to live and communicate with the level
	/// </summary>
	public class BallInLevel : Ball
	{
		public BallInLevel(Paddle paddle, Level level)
			: base(paddle)
		{
			Level = level;
		}

		public Level Level { get; private set; }

		protected override void UpdateInFlight(float timeDelta)
		{
			base.UpdateInFlight(timeDelta);
			HandleBrickCollisions();
			StartNewLevelIfAllBricksAreDestroyed();
		}

		private void HandleBrickCollisions()
		{
			HandleBrickCollision(Level.GetBrickAt(Position.X - Radius, Position.Y), Direction.Left);
			HandleBrickCollision(Level.GetBrickAt(Position.X, Position.Y - Radius), Direction.Top);
			HandleBrickCollision(Level.GetBrickAt(Position.X + Radius, Position.Y), Direction.Right);
			HandleBrickCollision(Level.GetBrickAt(Position.X, Position.Y + Radius), Direction.Bottom);
		}

		private void StartNewLevelIfAllBricksAreDestroyed()
		{
			if (Level.BricksLeft > 0)
				return;

			Level.InitializeNextLevel();
			ResetBall();
		}

		private void HandleBrickCollision(Sprite brick, Direction collisionSide)
		{
			if (brick == null)
				return;

			Level.Explode(brick, Position);
			ReflectVelocity(collisionSide);
		}

		public override void ResetBall()
		{
			Level.LifeLost();
			base.ResetBall();
		}
	}
}