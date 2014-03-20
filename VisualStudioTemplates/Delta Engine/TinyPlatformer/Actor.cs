using System;
using DeltaEngine.Datatypes;
using DeltaEngine.Entities;
using DeltaEngine.Rendering2D.Shapes;

namespace $safeprojectname$
{
	/// <summary>
	/// An object in the game: player, monster or treasure. 
	/// Player responds to keyboard input; Monster moves back and forth on a platform;
	/// Treasure remains static.
	/// </summary>
	public class Actor : FilledRect, Updateable
	{
		public Actor(Map map, Vector2D position, string type)
			: base(new Rectangle(0.0f, 0.0f, Map.BlockSize, Map.BlockSize), GetColor(type))
		{
			this.map = map;
			this.position = position;
			startPosition = position;
			this.type = type;
			MaxVelocityX = Map.DefaultMaxVelocityX * Map.Meter;
		}

		private readonly Map map;
		private Vector2D position;
		private readonly Vector2D startPosition;
		internal readonly string type;

		public float MaxVelocityX
		{
			get { return maxVelocityX; }
			set
			{
				maxVelocityX = value;
				maxAcceleration = maxVelocityX / AccelerationFactor;
				maxFriction = maxVelocityX / FrictionFactor;
			}
		}

		private float maxVelocityX;
		internal float maxAcceleration;
		internal float maxFriction;
		internal const float AccelerationFactor = 0.5f;
		internal const float FrictionFactor = 0.1667f;

		internal static Color GetColor(string type)
		{
			if (type == "player")
				return Color.Green;
			if (type == "monster")
				return Color.Red;
			if (type == "treasure")
				return Color.Yellow;
			return Color.Pink;
		}

		public void Update()
		{
			runningDelta += Math.Min(1.0f, Time.Delta);
			while (runningDelta > TimeStep && IsActive)
			{
				runningDelta -= TimeStep;
				UpdateOneTimeStep();
			}
			TopLeft = new Vector2D(Map.ScreenGap.Width + position.X * XPixelAdjust,
				Map.ScreenGap.Height + position.Y * YPixelAdjust);
		}

		private const float TimeStep = 1 / 60.0f;
		private float runningDelta;
		private const float XPixelAdjust = 0.8f / (64.0f * TileSize);
		private const int TileSize = 32;
		private const float YPixelAdjust = 0.6f / (48.0f * TileSize);

		private void UpdateOneTimeStep()
		{
			var wasMovingLeft = velocity.X < 0;
			var wasMovingRight = velocity.X > 0;
			UpdateVelocityAndPosition();
			ClampFrictionEffect(wasMovingLeft, wasMovingRight);
			UpdateOutOfTileOffsets();
			ProcessVerticalMovement();
			ProcessHorizontalMovement();
			ProcessMonster();
			ProcessTreasure();
			isFalling = !(IsCell(CellDown) || ((outOfTileOffsetX != 0) && IsCell(CellDiag)));
		}

		private Vector2D velocity;
		private int outOfTileOffsetX;
		private bool isFalling;

		private void UpdateVelocityAndPosition()
		{
			SetAcceleration();
			position.X += TimeStep * velocity.X;
			position.Y += TimeStep * velocity.Y;
			velocity.X += TimeStep * acceleration.X;
			velocity.Y += TimeStep * acceleration.Y;
			velocity.X = (velocity.X < -maxVelocityX)
				? -maxVelocityX : (velocity.X > maxVelocityX) ? maxVelocityX : velocity.X;
			velocity.Y = (velocity.Y < -MaxVelocityY)
				? -MaxVelocityY : (velocity.Y > MaxVelocityY) ? MaxVelocityY : velocity.Y;
		}

		private Vector2D acceleration;
		private const float MaxVelocityY = 60 * Map.Meter;

		private void SetAcceleration()
		{
			acceleration = new Vector2D(0.0f, Gravity);
			AdjustHorizontalAcceleration();
			AdjustVerticalAcceleration();
		}

		private const float Gravity = 2 * 9.8f * Map.Meter;

		private void AdjustHorizontalAcceleration()
		{
			var localAcceleration = maxAcceleration * (isFalling ? 0.5f : 1.0f);
			var localFriction = maxFriction * (isFalling ? 0.5f : 1.0f);
			if (WantsToGoLeft)
				acceleration.X -= localAcceleration;
			else if (velocity.X < 0)
				acceleration.X += localFriction;
			if (WantsToGoRight)
				acceleration.X += localAcceleration;
			else if (velocity.X > 0)
				acceleration.X -= localFriction;
		}

		public bool WantsToGoLeft { get; set; }
		public bool WantsToGoRight { get; set; }

		private void AdjustVerticalAcceleration()
		{
			if (!WantsToJump || isJumping || isFalling)
				return;
			acceleration.Y -= Impulse;
			isJumping = true;
		}

		public bool WantsToJump { get; set; }
		private bool isJumping;
		private const float Impulse = 900 * Map.Meter;

		private void ClampFrictionEffect(bool wasMovingLeft, bool wasMovingRight)
		{
			if ((wasMovingLeft && (velocity.X > 0)) || (wasMovingRight && (velocity.X < 0)))
				velocity.X = 0.0f;
		}

		private void UpdateOutOfTileOffsets()
		{
			outOfTileOffsetX = (int)(position.X % TileSize);
			outOfTileOffsetY = (int)(position.Y % TileSize);
		}

		private int outOfTileOffsetY;

		private void ProcessVerticalMovement()
		{
			if (velocity.Y > 0)
			{
				if (HasCellDownOrDiagonalNoRight())
					VerticalStop();
			}
			else if (velocity.Y < 0)
				if (HasCellRightNoDownOrDiag())
					VerticalAdjust();
		}

		private bool HasCellDownOrDiagonalNoRight()
		{
			return (IsCell(CellDown) && !IsCell(Cell)) ||
				(IsCell(CellDiag) && !IsCell(CellRight) && (outOfTileOffsetX != 0));
		}

		private static bool IsCell(BlockType cell)
		{
			return cell != BlockType.None;
		}

		private BlockType CellDown
		{
			get { return map.Blocks[TileX, TileY + 1]; }
			set { map.Blocks[TileX, TileY + 1] = value; }
		}

		private BlockType Cell
		{
			get { return map.Blocks[TileX, TileY]; }
			set { map.Blocks[TileX, TileY] = value; }
		}

		private BlockType CellDiag
		{
			get { return map.Blocks[TileX + 1, TileY + 1]; }
			set { map.Blocks[TileX + 1, TileY + 1] = value; }
		}

		private BlockType CellRight
		{
			get { return map.Blocks[TileX + 1, TileY]; }
			set { map.Blocks[TileX + 1, TileY] = value; }
		}

		private void VerticalStop()
		{
			position.Y = TileY * TileSize;
			velocity.Y = 0.0f;
			isFalling = false;
			isJumping = false;
			outOfTileOffsetY = 0;
		}

		private int TileY
		{
			get { return (int)Math.Floor(position.Y / TileSize); }
		}

		private bool HasCellRightNoDownOrDiag()
		{
			return (IsCell(Cell) && !IsCell(CellDown)) ||
				(IsCell(CellRight) && !IsCell(CellDiag) && (outOfTileOffsetX != 0));
		}

		private void VerticalAdjust()
		{
			position.Y = (TileY + 1) * TileSize;
			velocity.Y = 0;
			Cell = CellDown;
			CellRight = CellDiag;
			outOfTileOffsetY = 0;
		}

		private void ProcessHorizontalMovement()
		{
			if (velocity.X > 0)
			{
				if (HasCellRightOrDiagNoDown())
					HorizontalStopOnRightEdge();
			}
			else if (velocity.X < 0)
				if (HasCellDownNoRightOrDiag())
					HorizontalStopOnLeftEdge();
		}

		private bool HasCellRightOrDiagNoDown()
		{
			return (IsCell(CellRight) && !IsCell(Cell)) ||
				(IsCell(CellDiag) && !IsCell(CellDown) && outOfTileOffsetY != 0);
		}

		private void HorizontalStopOnRightEdge()
		{
			position.X = TileX * TileSize;
			velocity.X = 0;
		}

		private int TileX
		{
			get { return (int)Math.Floor(position.X / TileSize); }
		}

		private bool HasCellDownNoRightOrDiag()
		{
			return (IsCell(Cell) && !IsCell(CellRight)) ||
				(IsCell(CellDown) && !IsCell(CellDiag) && outOfTileOffsetY != 0);
		}

		private void HorizontalStopOnLeftEdge()
		{
			position.X = (TileX + 1) * TileSize;
			velocity.X = 0;
		}

		private void ProcessMonster()
		{
			if (type != "monster")
				return;
			UpdateMonsterDesiredMovement();
			ProcessPlayerMonsterCollision();
		}

		private void UpdateMonsterDesiredMovement()
		{
			if (WantsToGoLeft && (IsCell(Cell) || !IsCell(CellDown)))
			{
				WantsToGoLeft = false;
				WantsToGoRight = true;
			}
			else if (WantsToGoRight && (IsCell(CellRight) || !IsCell(CellDiag)))
			{
				WantsToGoRight = false;
				WantsToGoLeft = true;
			}
		}

		private void ProcessPlayerMonsterCollision()
		{
			if (IsCollidingWithPlayer())
				if (map.Player.velocity.Y > 0.0f)
					Die();
				else
					map.Player.Respawn();
		}

		private bool IsCollidingWithPlayer()
		{
			return
				!(((position.X + TileSize - 1) < map.Player.position.X) ||
					((map.Player.position.X + TileSize - 1) < position.X) ||
					((position.Y + TileSize - 1) < map.Player.position.Y) ||
					((map.Player.position.Y + TileSize - 1) < position.Y));
		}

		private void Die()
		{
			map.AddToScore(type);
			IsActive = false;
		}

		private void Respawn()
		{
			position = startPosition;
			velocity = Vector2D.Zero;
		}

		private void ProcessTreasure()
		{
			if (type == "treasure" && IsCollidingWithPlayer())
				Die();
		}

		public bool IsPlayer
		{
			get { return type == "player"; }
		}
	}
}