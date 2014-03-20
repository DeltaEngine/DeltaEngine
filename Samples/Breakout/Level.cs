using System;
using DeltaEngine.Content;
using DeltaEngine.Datatypes;
using DeltaEngine.Multimedia;
using DeltaEngine.Rendering2D;
using DeltaEngine.Rendering3D.Particles;

namespace Breakout
{
	/// <summary>
	/// All the bricks for each level are initialized and updated here.
	/// </summary>
	public class Level : IDisposable
	{
		public Level(Score score)
		{
			brickMaterial = new Material(ShaderFlags.Position2DColoredTextured, "Brick");
			var explosionMaterial = new Material(ShaderFlags.Position2DColoredTextured, "Explosion");
			explosionSound = ContentLoader.Load<Sound>("BrickExplosion");
			lostBallSound = ContentLoader.Load<Sound>("LostBall");
			explosionData = new ParticleEmitterData
			{
				Color = new RangeGraph<Color>(Color.White, Color.TransparentWhite),
				Size = new RangeGraph<Size>(ExplosionSize, ExplosionSize * 2),
				ParticleMaterial = explosionMaterial,
				MaximumNumberOfParticles = 1,
				LifeTime = 0.6f,
				SpawnInterval = -1
			};
			this.score = score;
			Initialize();
		}

		private readonly Material brickMaterial;
		private readonly ParticleEmitterData explosionData;
		private readonly Sound explosionSound;
		private readonly Sound lostBallSound;
		private readonly Score score;
		private Sprite border;

		private void Initialize()
		{
			rows = score.Level + 1;
			columns = score.Level + 1;
			bricks = new Sprite[rows,columns];
			brickWidth = 1.0f / rows;
			brickHeight = 0.5f / columns;
			CreateBricks();
		}

		private float brickWidth;
		private float brickHeight;
		protected int rows;
		protected int columns;
		protected Sprite[,] bricks;

		private void CreateBricks()
		{
			for (int x = 0; x < rows; x++)
				for (int y = 0; y < columns; y++)
				{
					bricks[x, y] = new Sprite(brickMaterial, GetBounds(x, y));
					bricks[x, y].Set(GetBrickColor(x, y));
					bricks[x, y].RenderLayer = 5;
				}
			border = new Sprite(ContentLoader.Load<Material>("BorderMaterial"), Rectangle.One);
		}

		private Rectangle GetBounds(int x, int y)
		{
			return new Rectangle(x * brickWidth, y * brickHeight, brickWidth, brickHeight);
		}

		private Color GetBrickColor(int x, int y)
		{
			if (score.Level <= 1)
				return GetLevelOneBrickColor(x, y);

			if (score.Level == 2)
				return GetLevelTwoBrickColor(y);

			if (score.Level == 3)
				return GetLevelThreeBrickColor(x, y);

			if (score.Level == 4)
				return GetLevelFourBrickColor(x, y);

			return GetLevelFiveOrAboveBrickColor(x, y);
		}

		private static Color GetLevelOneBrickColor(int x, int y)
		{
			return x + y == 1 ? Color.Green : Color.Orange;
		}

		private static Color GetLevelTwoBrickColor(int y)
		{
			if (y == 0)
				return new Color(0.25f, 0.25f, 0.25f);

			return y == 1 ? Color.Red : Color.Gold;
		}

		private static Color GetLevelThreeBrickColor(int x, int y)
		{
			return LevelThreeColors[(x * 4 + y) % LevelThreeColors.Length];
		}

		private static readonly Color[] LevelThreeColors = new[]
		{ Color.Yellow, Color.Teal, Color.Green, Color.LightBlue, Color.Teal };

		private static Color GetLevelFourBrickColor(int x, int y)
		{
			return new Color(x * 0.2f + 0.1f, 0.2f, (x + y / 2) * 0.15f + 0.2f);
		}

		private static Color GetLevelFiveOrAboveBrickColor(int x, int y)
		{
			return new Color(0.9f - x * 0.15f, 0.5f, (x + y / 2) * 0.1f + 0.2f);
		}

		public void InitializeNextLevel()
		{
			Dispose();
			score.NextLevel();
			Initialize();
		}

		public void Dispose()
		{
			for (int x = 0; x < rows; x++)
				for (int y = 0; y < columns; y++)
					bricks[x, y].Dispose();
			border.IsActive = false;
		}

		public int BricksLeft
		{
			get
			{
				var bricksAlive = 0;
				for (int x = 0; x < rows; x++)
					for (int y = 0; y < columns; y++)
						if (bricks[x, y].IsVisible)
							bricksAlive++;

				return bricksAlive;
			}
		}

		public Sprite GetBrickAt(float x, float y)
		{
			var brickIndexX = (int)(x / brickWidth);
			var brickIndexY = (int)(y / brickHeight);
			if (brickIndexX < 0 || brickIndexX >= rows || brickIndexY < 0 || brickIndexY >= columns ||
				bricks[brickIndexX, brickIndexY].IsVisible != true)
				return null;

			return bricks[brickIndexX, brickIndexY];
		}

		public void Explode(Sprite brick, Vector2D collision)
		{
			score.IncreasePoints();
			brick.IsVisible = false;
			CreateExplosion(collision);
			explosionSound.Play();
		}

		private void CreateExplosion(Vector2D collision)
		{
			var explosion = new ParticleEmitter(explosionData, collision) { RenderLayer = 16 };
			explosion.SpawnAndDispose();
		}

		private static Size ExplosionSize
		{
			get { return new Size(0.1f, 0.1f); }
		}

		public void LifeLost()
		{
			score.LifeLost();
			lostBallSound.Play();
		}
	}
}