using System;
using DeltaEngine.Content;
using DeltaEngine.Core;
using DeltaEngine.Datatypes;
using DeltaEngine.Entities;
using DeltaEngine.Extensions;
using DeltaEngine.Multimedia;
using DeltaEngine.Rendering2D;
using GameOfDeath.Items;

namespace $safeprojectname$
{
	/// <summary>
	/// Derived from the GameOfLife and handles the rabbit logic.
	/// </summary>
	public class RabbitGrid : GameOfLife
	{
		public RabbitGrid(int matrixWidth, int matrixHeight, Rectangle areaInViewport)
			: base(matrixWidth, matrixHeight)
		{
			rectInViewport = areaInViewport;
			rabbitSize =
				new Size(Math.Min(rectInViewport.Width / matrixWidth, rectInViewport.Height / matrixHeight));
			LoadContent();
			InitializeRabbits();
			RandomlySpawn();
		}

		private Rectangle rectInViewport;
		private Size rabbitSize;

		private void LoadContent()
		{
			deadRabbitMaterial = ContentLoader.Load<Material>("MaterialDeadRabbit");
			malletHitSound = ContentLoader.Load<Sound>("MalletHit");
			malletBloodMaterial = ContentLoader.Load<Material>("MaterialSplat");
			gameOverMaterial = ContentLoader.Load<Material>("MaterialGameOver");
		}

		private Material deadRabbitMaterial;
		private Sound malletHitSound;
		private Material malletBloodMaterial;
		private Material gameOverMaterial;

		private void InitializeRabbits()
		{
			Rabbits = new Rabbit[width,height];
			growCell = new float[width,height];
			for (int x = 0; x < width; x++)
				for (int y = 0; y < height; y++)
					Rabbits[x, y] = new Rabbit(CalculatePositionOfMatrixRabbit(x, y), rabbitSize);
		}

		public void RecalculateRabbitPositionsAndSizes(Rectangle newRect)
		{
			rabbitSize =
				new Size(Math.Min(rectInViewport.Width / width, rectInViewport.Height / height * 1.33f));
			for (int x = 0; x < width; x++)
				for (int y = 0; y < height; y++)
				{
					Rabbits[x, y].Center = CalculatePositionOfMatrixRabbit(x, y);
					Rabbits[x, y].OriginalSize = rabbitSize;
				}
		}

		public Rabbit[,] Rabbits { get; private set; }
		private float[,] growCell;

		public Vector2D CalculatePositionOfMatrixRabbit(int x, int y)
		{
			return new Vector2D(rectInViewport.Left + x * rectInViewport.Width / width,
				rectInViewport.Top + y * rectInViewport.Height / height);
		}

		protected override void BaseUpdate()
		{
			if (!IsGameOver() && Time.CheckEvery(GetTimeStep()))
				RandomlySpawn();
			//base.BaseUpdate();
			CheckForPayday();
			GrowRabbits();
		}

		private void CheckForPayday()
		{
			if (Time.CheckEvery(PaydayInterval) && !IsGameOver() && MoneyEarned != null)
				MoneyEarned(Payday);
		}

		private const float PaydayInterval = 1.0f;
		private const int Payday = 1;
		public event Action<int> MoneyEarned;

		private void GrowRabbits()
		{
			for (int x = 0; x < width; x++)
				for (int y = 0; y < height; y++)
					if (base[x, y])
						GrowRabbit(x, y);
		}

		private void GrowRabbit(int x, int y)
		{
			if (Rabbits[x, y].IsDead)
				Rabbits[x, y].SetHealth(GetCurrentNewRabbitHealth());

			if (growCell[x, y] < 1.0f)
				growCell[x, y] += GrowSpeed * Time.Delta;

			Rabbits[x, y].IsVisible = true;
			Rabbits[x, y].Scale = growCell[x, y];
		}

		private static float GetCurrentNewRabbitHealth()
		{
			return InitialRabbitHealth + GlobalTime.Current.Milliseconds / IncreaseRabbitHealthEveryMs;
		}

		private const float InitialRabbitHealth = 1;
		private const float IncreaseRabbitHealthEveryMs = 2.0f * 1000;
		private const float GrowSpeed = 0.66f;

		private static float GetTimeStep()
		{
			return MathExtensions.Max(1.5f - 0.5f * GlobalTime.Current.Milliseconds / 60000.0f, 0.1f);
		}

		private void RandomlySpawn()
		{
			int x = Randomizer.Current.Get(0, width);
			int y = Randomizer.Current.Get(0, height);
			base[x, y] = true;
			if (x + 1 < width && Randomizer.Current.Get(0, 2) == 1)
				base[x + 1, y] = true;

			if (y + 1 < height && Randomizer.Current.Get(0, 2) == 1)
				base[x, y + 1] = true;
		}

		public void DoDamage(Vector2D positionHit, float sizeOfImpact, float damage)
		{
			for (int x = 0; x < width; x++)
				for (int y = 0; y < height; y++)
					if (CalculatePositionOfMatrixRabbit(x, y).DistanceTo(positionHit) < sizeOfImpact &&
						base[x, y])
						DamageRabbit(x, y, damage);
		}

		private void DamageRabbit(int x, int y, float damage)
		{
			ShowMalletHit(x, y, damage);
			Rabbits[x, y].DoDamage(damage);
			if (Rabbits[x, y].IsDead)
				KillRabbit(x, y);
		}

		private void ShowMalletHit(int x, int y, float damage)
		{
			if (damage != Mallet.DefaultDamage)
				return;
			malletHitSound.Play(0.5f);
			CreateMalletBlood(Rabbits[x, y].Center);
		}

		private void CreateMalletBlood(Vector2D center)
		{
			var drawArea = Rectangle.FromCenter(center, MalletBloodSize);
			new FadeSprite(malletBloodMaterial, drawArea, 1.0f);
		}

		private static readonly Size MalletBloodSize = new Size(0.09f);

		private void KillRabbit(int x, int y)
		{
			Rabbits[x, y].IsVisible = false;
			Rabbits[x, y].RabbitHealthBar.IsVisible = false;
			base[x, y] = false;
			growCell[x, y] = 0.0f;
			new FadeSprite(deadRabbitMaterial, Rabbits[x, y].DrawArea, 5.0f);
			AddToScoreAndMoney();
		}

		private void AddToScoreAndMoney()
		{
			if (RabbitKilled != null)
				RabbitKilled();

			if (MoneyEarned != null)
				MoneyEarned(MoneyPerDeadRabbit);
		}

		public event Action RabbitKilled;
		private const int MoneyPerDeadRabbit = 2;

		public bool IsGameOver()
		{
			if (!gameOver)
				CheckPopulation();
			return gameOver;
		}

		private bool gameOver;

		private void CheckPopulation()
		{
			if (!IsOverPopulated())
				return;
			gameOver = true;
			new Sprite(gameOverMaterial,
				Rectangle.FromCenter(Vector2D.Half,
					gameOverMaterial.DiffuseMap.PixelSize / UserInterface.QuadraticFullscreenSize))
			{
				RenderLayer = (int)RenderLayers.IntroLogo
			};
			if (GameOver != null)
				GameOver();
		}

		public event Action GameOver;

		public bool IsOverPopulated()
		{
			return NumberOfActiveRabbits > width * height * OverPopulationPercentage;
		}

		private const float OverPopulationPercentage = 0.75f;

		private int NumberOfActiveRabbits
		{
			get
			{
				int activeRabbits = 0;
				for (int x = 0; x < width; x++)
					for (int y = 0; y < height; y++)
						if (base[x, y])
							activeRabbits++;
				return activeRabbits;
			}
		}
	}
}