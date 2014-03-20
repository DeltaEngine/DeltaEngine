using System;
using System.Collections.Generic;
using DeltaEngine.Commands;
using DeltaEngine.Content;
using DeltaEngine.Core;
using DeltaEngine.Datatypes;
using DeltaEngine.Entities;
using DeltaEngine.Extensions;
using DeltaEngine.Input;
using DeltaEngine.Multimedia;
using DeltaEngine.Rendering2D;

namespace $safeprojectname$
{
	/// <summary>
	/// The player interacts with the ball with his paddle and navigates it to destroy bricks.
	/// </summary>
	public class Ball : Sprite
	{
		public Ball(Paddle paddle)
			: base(new Material(ShaderFlags.Position2DColoredTextured,"Ball"), Rectangle.Zero)
		{
			this.paddle = paddle;
			fireBallSound = ContentLoader.Load<Sound>("PaddleBallStart");
			collisionSound = ContentLoader.Load<Sound>("BallCollision");
			UpdateOnPaddle();
			RegisterFireBallCommand();
			Start<RunBall>();
			RenderLayer = 5;
		}

		private readonly Paddle paddle;
		private readonly Sound fireBallSound;
		private readonly Sound collisionSound;

		private void UpdateOnPaddle()
		{
			isOnPaddle = true;
			Position = new Vector2D(paddle.Position.X, paddle.Position.Y - Radius);
		}

		protected bool isOnPaddle;

		private void RegisterFireBallCommand()
		{
			var command = new Command(FireBallFromPaddle);
			command.Add(new KeyTrigger(Key.Space));
			command.Add(new MouseButtonTrigger());
			command.Add(new GamePadButtonTrigger(GamePadButton.A));
		}

		private void FireBallFromPaddle()
		{
			if (!isOnPaddle || !IsVisible)
				return;

			isOnPaddle = false;
			float randomXSpeed = Randomizer.Current.Get(-0.15f, 0.15f);
			velocity = new Vector2D(randomXSpeed.Abs() < 0.01f ? 0.01f : randomXSpeed, StartBallSpeedY);
			fireBallSound.Play();
		}

		protected static Vector2D velocity;
		private const float StartBallSpeedY = -1f;

		public virtual void ResetBall()
		{
			UpdateOnPaddle();
			velocity = Vector2D.Zero;
		}

		public class RunBall : UpdateBehavior
		{
			public override void Update(IEnumerable<Entity> entities)
			{
				foreach (var entity in entities)
				{
					var ball = (Ball)entity;
					if (ball.isOnPaddle)
						ball.UpdateOnPaddle();
					else
						ball.UpdateInFlight(Time.Delta);

					const float Aspect = 1;
					ball.DrawArea = Rectangle.FromCenter(ball.Position, new Size(Height / Aspect, Height));
				}
			}
		}

		public Vector2D Position { get; protected set; }
		public static readonly Size BallSize = new Size(Height);
		private const float Height = Radius * 2.0f;
		internal const float Radius = 0.02f;

		protected virtual void UpdateInFlight(float timeDelta)
		{
			Position += velocity * timeDelta;
			HandleBorderCollisions();
			HandlePaddleCollision();
		}

		private void HandleBorderCollisions()
		{
			if (Position.X < Radius)
				HandleBorderCollision(Direction.Left);
			else if (Position.X > 1.0f - Radius)
				HandleBorderCollision(Direction.Right);

			if (Position.Y < Radius)
				HandleBorderCollision(Direction.Top);
			else if (Position.Y > 1.0f - Radius)
				HandleBorderCollision(Direction.Bottom);
		}

		protected enum Direction
		{
			Left,
			Top,
			Right,
			Bottom,
		}

		protected void ReflectVelocity(Direction collisionSide)
		{
			switch (collisionSide)
			{
				case Direction.Left:
					velocity.X = Math.Abs(velocity.X);
					break;
				case Direction.Top:
					velocity.Y = Math.Abs(velocity.Y);
					break;
				case Direction.Right:
					velocity.X = -Math.Abs(velocity.X);
					break;
				case Direction.Bottom:
					velocity.Y = -Math.Abs(velocity.Y);
					break;
			}
		}

		private void HandleBorderCollision(Direction collisionAtBorder)
		{
			ReflectVelocity(collisionAtBorder);
			if (collisionAtBorder == Direction.Bottom)
				ResetBall();
			else
				collisionSound.Play();
		}

		private void HandlePaddleCollision()
		{
			if (IsInAreaOfPaddle())
				SetNewVelocityAfterPaddleCollision();
		}

		private bool IsInAreaOfPaddle()
		{
			if (Position.Y + Radius > paddle.Position.Y && velocity.Y > 0)
				return Position.X + Radius > paddle.Position.X - Paddle.HalfWidth &&
					Position.X - Radius < paddle.Position.X + Paddle.HalfWidth;
			return false;
		}

		private void SetNewVelocityAfterPaddleCollision()
		{
			velocity.X += (Position.X - paddle.Position.X) * SpeedXIncrease;
			velocity.Y = -Math.Abs(velocity.Y) * SpeedYIncrease;
			float speed = velocity.Length;
			if (speed > MaximumScalarSpeed)
				velocity *= MaximumScalarSpeed / speed; //ncrunch: no coverage
			collisionSound.Play();
		}

		private const float SpeedYIncrease = 1.015f;
		private const float SpeedXIncrease = 2.5f;
		private const float MaximumScalarSpeed = 1.2f;

		public override void Dispose()
		{
			paddle.Dispose();
			base.Dispose();
		}
	}
}